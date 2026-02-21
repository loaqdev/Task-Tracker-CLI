using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Task_Tracker_CLI.Models;

internal class TaskModel
{
    [JsonProperty(propertyName: "id")]
    public int Id { get; set; }

    [JsonProperty(propertyName: "description")]
    public string Description { get; set; } = string.Empty;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum TaskStatus
    {
        todo,
        inProgress,
        done
    }

    [JsonProperty(propertyName: "status")]
    public TaskStatus Status { get; set; } = TaskStatus.todo;

    [JsonProperty(propertyName: "creationDate")]
    public string CreatedAt { get; set; } = string.Empty;

    [JsonProperty(propertyName: "updationDate")]
    public string UpdatedAt { get; set; } = string.Empty;
}