# Task Tracker CLI

A small cross-platform console application to manage personal tasks (CLI).  
Add, update, delete and change task status. Tasks are persisted to `tasks.json`.

## Features
- Add / update / delete tasks
- Mark tasks as `todo`, `inProgress`, `done`
- List all tasks or filter by status
- Persistent storage in JSON using `Newtonsoft.Json`
- Simple interactive command-line interface

## Requirements
- .NET 10 SDK
- C# 14

## Quick start (development)
1. Clone the repository:
   - `git clone https://github.com/loaqdev/Task-Tracker-CLI.git`
2. Open project folder:
   - `cd "Task Tracker CLI"`
3. Build and run:
   - `dotnet build`
   - `dotnet run --project "Task Tracker CLI.csproj"`

By default the app reads/writes `tasks.json` from the app base directory.

## Commands
- `add "task description"` — add a new task  
- `update <id> "new description"` — update a task  
- `delete <id>` — delete a task  
- `mip <id>` — mark as in progress  
- `md <id>` — mark as done  
- `list` — show all tasks  
- `list todo|inProgress|done` — filter by status  
- `clear` — clear the screen  
- `exit` — exit the app

Examples:
- `add "Buy groceries"`
- `update 2 "Buy milk and bread"`
- `mip 2`
- `md 2`
- `list todo`

## Data location and permissions
By default the app stores `tasks.json` in the application base directory. If the published app runs from a protected folder (e.g. `Program Files`), writing may fail. Consider storing app data in user-specific folder:

```
string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData); 
string path = Path.Combine(appData, "TaskTrackerCLI", "tasks.json");
Directory.CreateDirectory(Path.GetDirectoryName(path)!);
```


## Troubleshooting Newtonsoft.Json errors
- Collect full exception text and stack trace from the published app.
- Ensure `Newtonsoft.Json.dll` is present in the publish folder.
- Disable trimming / single-file (see above) if reflection-based serialization fails.
- Option: migrate `FileIOService` to `System.Text.Json` to avoid trimming issues.

## Contributing & License
Pull requests welcome. Open issues for bugs or feature requests.
