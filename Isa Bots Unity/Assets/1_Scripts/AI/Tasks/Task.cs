using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TaskActivity
{
    move,
    mine,
}

[System.Serializable]
public class Task
{
    public TaskActivity TaskActivity;
    public Vector2Int Pos;

    public Task(TaskActivity TaskActivity, Vector2Int Pos)
    {
        this.TaskActivity = TaskActivity;
        this.Pos = Pos;
    }
}
