using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIPathFinding : BaseState
{
    private Tile startTile;
    private Tile targetTile;
    private AStar aStar = new AStar();
    private List<Tile> path;
    private int pathIndex = 0;

    //References 
    private Robot robot;

    public override void OnStart()
    {
        robot = GetComponent<Robot>();

        CalcTiles();
        CalcPath();

        if (path == null)
        {
            TaskFailed();
        }
    }

    public override void OnUPS()
    {
        StepTroughPath();
    }

    public override void OnExit()
    {

    }

    protected virtual void StepTroughPath()
    {
        if (pathIndex < path.Count)
        {
            robot.SetPos(path[pathIndex].Pos);
            pathIndex++;
        }
        else
        {
            TaskSuccess();
        }
    }

    private void CalcTiles()
    {
        startTile = robot.tileGrid.GetTile(robot.robot.Pos);
        targetTile = robot.tileGrid.GetTile(robot.currentTask.Pos);
    }

    private void CalcPath()
    {
        path = aStar.CalcPath(startTile, targetTile, robot.tileGrid, robot.currentTask.allowedMainTiles, robot.currentTask.allowedGroundTiles);
    }

    private void TaskSuccess()
    {
        ClearValues();
        owner.SwitchState(typeof(RobotWaitState));
    }

    private void TaskFailed()
    {
        Debug.Log("Task failed");
        ClearValues();
        robot.TaskFailed(robot.currentTask);
    }

    private void ClearValues()
    {
        startTile = null;
        targetTile = null;
        path = null;
        pathIndex = 0;
    }
}
