using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TaskActivity
{
    move,
    mine,
}

public class Task
{ 
    public TaskActivity TaskActivity { get; set; }
    public Vector2Int Pos { get; set; }

    public List<MainID> allowedMainTiles { get; } = new List<MainID>() { MainID.none };
    public List<GroundID> allowedGroundTiles { get; } = new List<GroundID>() { GroundID.dirt, GroundID.grass };

    public Task(TaskActivity TaskActivity, Vector2Int Pos)
    {
        this.TaskActivity = TaskActivity;
        this.Pos = Pos;
    }
}
