using Newtonsoft.Json;
using System.ComponentModel;
using System.Text.Json;
using Task_Tracker_CLI.Models;

namespace Task_Tracker_CLI.Services;

internal class FileIOService
{
    private readonly string PATH;

    public FileIOService(string path) => PATH = path;

    public void SaveToFile(List<TaskModel> tasksList)
    {
        var output = JsonConvert.SerializeObject(tasksList);
        File.WriteAllText(PATH, output);
    }

    public List<TaskModel> LoadFromFile()
    {
        var fileExists = File.Exists(PATH);
        if (!fileExists)
        {
            File.CreateText(PATH).Dispose();
            return new List<TaskModel>();
        }

        var fileText = File.ReadAllText(PATH);
        if (string.IsNullOrWhiteSpace(fileText))
            return new List<TaskModel>();

        var data = JsonConvert.DeserializeObject<List<TaskModel>>(fileText);

        return data ?? new List<TaskModel>();
    }
}
