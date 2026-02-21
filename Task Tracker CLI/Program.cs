using System.Globalization;
using Task_Tracker_CLI.Models;
using Task_Tracker_CLI.Services;

string PATH = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tasks.json");

FileIOService _fileIOService = new(PATH);

List<TaskModel> _tasks = _fileIOService.LoadFromFile();

Console.WriteLine("Welcome to your CLI Task Tracker!");
Console.WriteLine("Enter \"help\" to see list of commands.\n");


while (true)
{
    Console.Write(">task-cli ");
    var raw = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(raw)) continue;

    var (command, option) = SplitCommand(raw);

    switch (command)
    {
        case "help":
            ShowHelp();
            break;
        case "add":
            AddTask(option);
            break;
        case "update":
            UpdateTask(option);
            break;
        case "delete":
            DeleteTaskByArg(option);
            break;
        case "mip":
            MarkInProgressByArg(option);
            break;
        case "md":
            MarkDoneByArg(option);
            break;
        case "list":
            ListTasks(option);
            break;
        case "clear":
            Console.Clear();
            break;
        case "exit": return;
        default:
            Console.WriteLine("Unknown command. Type \"help\" for a list of commands.");
            break;
    }
}

// --- Helpers ---

(string command, string args) SplitCommand(string input)
{
    var firstSpace = input.IndexOf(' ');
    if (firstSpace == -1) return (input.Trim(), string.Empty);
    var cmd = input.Substring(0, firstSpace).Trim();
    var rest = input.Substring(firstSpace + 1).Trim();
    return (cmd, rest);
}

(string? first, string? second) SplitFirst(string s)
{
    if (string.IsNullOrWhiteSpace(s)) return (null, null);
    var trimmed = s.Trim();
    var idx = trimmed.IndexOf(' ');
    if (idx == -1) return (trimmed, null);
    return (trimmed.Substring(0, idx), trimmed.Substring(idx + 1).Trim());
}

string TrimQuotes(string s)
{
    if (string.IsNullOrEmpty(s)) return s;
    s = s.Trim();
    if ((s.StartsWith("\"") && s.EndsWith("\"")) || (s.StartsWith("'") && s.EndsWith("'")))
        return s.Substring(1, s.Length - 2);
    return s;
}

int? ParseId(string s)
{
    if (int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out int id))
        return id;
    return null;
}

void ListTasks(string parameters)
{
    if (_tasks.Count == 0)
    {
        Console.WriteLine("There are no tasks in the list.");
        return;
    }

    IEnumerable<TaskModel> toShow = _tasks;

    if (!string.IsNullOrWhiteSpace(parameters))
    {
        var p = parameters.Trim();
        // accept "todo", "inProgress", "done" (case-insensitive)
        if (Enum.TryParse<TaskModel.TaskStatus>(p, true, out var status))
            toShow = _tasks.Where(t => t.Status == status);
        else
        {
            Console.WriteLine("Unknown status filter. Use: todo, inProgress, done");
            return;
        }
    }

    Console.WriteLine("ID | Description | Status | Created | Updated");
    foreach (var task in toShow)
    {
        Console.WriteLine($"{task.Id} | {task.Description} | {task.Status} | {task.CreatedAt} | {task.UpdatedAt}");
    }
}

void MarkDoneByArg(string parameters) => MarkStatusByArg(parameters, TaskModel.TaskStatus.done, "done");

void MarkInProgressByArg(string parameters) => MarkStatusByArg(parameters, TaskModel.TaskStatus.inProgress, "in progress");

void MarkStatusByArg(string parameters, TaskModel.TaskStatus status, string statusText)
{
    var id = ParseId(parameters);
    if (!id.HasValue)
    {
        Console.WriteLine("Usage: <command> <taskId>");
        return;
    }

    var idx = _tasks.FindIndex(t => t.Id == id.Value);
    if (idx == -1)
    {
        Console.WriteLine($"Task with ID:{id.Value} does not exist.");
        return;
    }

    try
    {
        _tasks[idx].Status = status;
        _tasks[idx].UpdatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
        _fileIOService.SaveToFile(_tasks);
        Console.WriteLine($"Task with ID:{id.Value} was successfully marked as {statusText}.");
    }
    catch (Exception ex)
    {
        Console.WriteLine("Failed to change status, error message: " + ex.Message);
    }
}

void DeleteTaskByArg(string parameters)
{
    var id = ParseId(parameters);
    if (!id.HasValue)
    {
        Console.WriteLine("Usage: delete <taskId>");
        return;
    }

    var idx = _tasks.FindIndex(t => t.Id == id.Value);
    if (idx == -1)
    {
        Console.WriteLine($"Task with ID:{id.Value} does not exist.");
        return;
    }

    try
    {
        _tasks.RemoveAt(idx);
        _fileIOService.SaveToFile(_tasks);
        Console.WriteLine($"Task with ID:{id.Value} was successfully deleted.");
    }
    catch (Exception ex)
    {
        Console.WriteLine("Failed to delete task, error message: " + ex.Message);
    }
}

void UpdateTask(string parameters)
{
    // expected: <id> "new description"
    var parts = SplitFirst(parameters);
    if (parts.first == null)
    {
        Console.WriteLine("Usage: update <taskId> \"new description\"");
        return;
    }

    var id = ParseId(parts.first);
    if (!id.HasValue)
    {
        Console.WriteLine("Invalid task id.");
        return;
    }

    var newDesc = TrimQuotes(parts.second ?? "");
    if (string.IsNullOrWhiteSpace(newDesc))
    {
        Console.WriteLine("New description cannot be empty.");
        return;
    }

    var idx = _tasks.FindIndex(t => t.Id == id.Value);
    if (idx == -1)
    {
        Console.WriteLine($"Task with ID:{id.Value} does not exist.");
        return;
    }

    try
    {
        _tasks[idx].Description = newDesc;
        _tasks[idx].UpdatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
        _fileIOService.SaveToFile(_tasks);
        Console.WriteLine($"Task with ID: {id.Value} was successfully updated!");
    }
    catch (Exception ex)
    {
        Console.WriteLine("Failed to update task, error message: " + ex.Message);
    }
}

void AddTask(string parameters)
{
    var description = TrimQuotes(parameters);
    if (string.IsNullOrWhiteSpace(description))
    {
        Console.WriteLine("Cannot add empty task. Usage: add \"task description\"");
        return;
    }

    try
    {
        int newId = _tasks.Any() ? _tasks.Max(t => t.Id) + 1 : 1;
        var now = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);

        var task = new TaskModel
        {
            Id = newId,
            Description = description,
            Status = TaskModel.TaskStatus.todo,
            CreatedAt = now
        };

        _tasks.Add(task);
        _fileIOService.SaveToFile(_tasks);
        Console.WriteLine($"Task added successfully with ID: {newId}");
    }
    catch (Exception ex)
    {
        Console.WriteLine("Failed to add task, error message: " + ex.Message);
    }
}

void ShowHelp()
{
    Console.WriteLine(
        "Available commands:\n" +
        "add \"task description\" - Add a new task\n" +
        "update <taskId> \"new description\" - Update an existing task\n" +
        "delete <taskId> - Delete a task\n" +
        "mip <taskId> - Mark a task as in progress\n" +
        "md <taskId> - Mark a task as done\n" +
        "list [status] - List all tasks or filter by status (todo, inProgress, done)\n" +
        "clear - Clear the screen\n" +
        "exit - Exit the application\n"
    );
}