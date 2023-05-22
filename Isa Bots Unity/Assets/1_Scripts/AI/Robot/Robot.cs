using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : BaseClass
{
    public BaseState currentState;
    public Task currentTask { get; set; }
    public Vector2Int Pos { get; set; }

    //References
    public Robot robot { get; set; }
    public TileGrid tileGrid { get; set; }
    private Tasks tasks;
    private FSM robotState;

    public override void OnAwake()
    {
        robot = GetComponent<Robot>();
        tasks = FindObjectOfType<Tasks>();
        tileGrid = FindObjectOfType<TileGrid>();
    }

    public override void OnStart()
    {
        //FSM
        robotState = new FSM(GetComponents<BaseState>());
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
        gameObject.transform.position = new Vector3(newPos.x, -newPos.y, -5);
    }

    public void ChooseTask()
    {
        currentTask = null;
        currentTask = tasks.GetTask();

        if (currentTask != null)
        {
            tasks.RemoveTask(currentTask);

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
            robotState.SwitchState(typeof(RobotWaitState));
        }
    }

    public void TaskFailed(Task task)
    {
        tasks.AddTask(task);
        robotState.SwitchState(typeof(RobotWaitState));
    }

    private void UpdateFSM()
    {
        robotState.OnUPS();
        currentState = robotState.currentState;
    }
}