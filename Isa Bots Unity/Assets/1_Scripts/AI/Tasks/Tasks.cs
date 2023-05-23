using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tasks : BaseClass
{
    public List<Task> taskList = new List<Task>();

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