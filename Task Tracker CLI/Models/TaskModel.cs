using System;
using System.Collections.Generic;
using System.Text;

namespace Task_Tracker_CLI.Models;

internal class TaskModel
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Description { get; set; } = string.Empty;

    public enum TaskStatus
    {
        todo,
        inProgress,
        done
    }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

}