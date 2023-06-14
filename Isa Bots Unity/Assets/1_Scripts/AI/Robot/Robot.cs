using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : BaseClass
{
    public Vector2Int Pos;
    public Task currentTask;
    public int GoBackToHomeAfterNoTask;

    private int goBackToHomeAfterNoTaskCounter;

    //References
    public TileGrid tileGrid { get; set; }
    private Tasks tasks;
    private FSM robotState;

    public override void OnAwake()
    {
        tasks = FindObjectOfType<Tasks>();
        tileGrid = FindObjectOfType<TileGrid>();
    }

    public override void OnStart()
    {
        //FSM
        robotState = new FSM(GetComponents<BaseState>());
        BaseState[] robotStates = GetComponents<BaseState>();
        for (int i = 0; i < robotStates.Length; i++)
        {
            robotStates[i].OnAwake();
        }
        robotState.SwitchState(typeof(RobotWaitState));

        SetPos(new Vector2Int(1, 1));
    }

    public override void OnUPS()
    {
        UpdateFSM();
    }

    public void SetPos(Vector2Int newPos)
    {
        Pos = newPos;
    }

    public void ChooseTask()
    {
        currentTask = null;
        currentTask = tasks.GetPendingTask();

        if (currentTask != null)
        {
            goBackToHomeAfterNoTaskCounter = 0;
            tasks.ActivateTask(currentTask);

            if (currentTask.TaskActivity == TaskActivity.move)
            {
                robotState.SwitchState(typeof(RobotMoveState));
            }
            else if (currentTask.TaskActivity == TaskActivity.mine)
            {
                robotState.SwitchState(typeof(RobotMineState));
            }
        }
        else
        {
            if (goBackToHomeAfterNoTaskCounter >= GoBackToHomeAfterNoTask)
            {
                currentTask = new Task(TaskActivity.move, new Vector2Int(1, 1));
                robotState.SwitchState(typeof(RobotMoveState));
            }
            else
            {
                goBackToHomeAfterNoTaskCounter++;
                robotState.SwitchState(typeof(RobotWaitState));
            }
        }
    }

    public void TaskAccomplished(Task task)
    {
        tasks.TaskAccomplished(task);
        robotState.SwitchState(typeof(RobotWaitState));
    }

    public void TaskFailed(Task task)
    {
        tasks.TaskFailed(task);
        ChooseTask();
    }

    private void UpdateFSM()
    {
        robotState.OnUPS();
    }
}