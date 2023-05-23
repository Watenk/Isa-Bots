using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Tasks : BaseClass
{
    public int RetryFailedTasksTime;

    public List<Task> activeTaskList = new List<Task>();
    public List<Task> pendingTaskList = new List<Task>();
    public List<Task> failedTasksList = new List<Task>();

    private int retryFailedTasksTimer;

    public override void OnUPS()
    {
        if (retryFailedTasksTimer >= RetryFailedTasksTime)
        {
            if (failedTasksList.Count > 0)
            {
                pendingTaskList.AddRange(failedTasksList);
                failedTasksList.Clear();
            }
            retryFailedTasksTimer = 0;
        }
        else
        {
            retryFailedTasksTimer++;
        }
    }

    public Task GetPendingTask()
    {
        if (pendingTaskList.Count > 0) // if task is availible
        {
            Task currentTask = pendingTaskList[0];
            return currentTask;
        }
        return null;
    }

    public void AddTask(Task task)
    {
        pendingTaskList.Add(task);
    }

    public void ActivateTask(Task task)
    {
        activeTaskList.Add(task);
        pendingTaskList.Remove(task);
    }

    public void TaskAccomplished(Task task)
    {
        activeTaskList.Remove(task);
    }

    public void TaskFailed(Task task)
    {
        activeTaskList.Remove(task);
        failedTasksList.Add(task);
    }
}