using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tasks : BaseClass
{
    private List<Task> taskList = new List<Task>();

    public override void OnStart()
    {
        AddTask(new Task(TaskActivity.move, new Vector2Int(5, 5)));
        AddTask(new Task(TaskActivity.mine, new Vector2Int(11, 10)));
    }

    public Task GetTask()
    {
        if (taskList.Count > 0) // if task is availible
        {
            Task currentTask = taskList[0];
            return currentTask;
        }
        return null;
    }

    public void AddTask(Task task)
    {
        taskList.Add(task);
    }

    public void RemoveTask(Task task)
    {
        taskList.Remove(task);
    }
}