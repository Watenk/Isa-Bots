using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIPathFinding : BaseState
{
    public List<MainID> allowedMainTiles;
    public List<GroundID> allowedGroundTiles;

    protected Tile startTile;
    protected Tile targetTile;
    protected AStar aStar = new AStar();
    protected List<Tile> path;
    protected int pathIndex = 0;

    //References 
    protected Robot robot;

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
            LocationReached();
        }
    }

    protected virtual void CalcTiles()
    {
        startTile = robot.tileGrid.GetTile(robot.robot.Pos);
        targetTile = robot.tileGrid.GetTile(robot.currentTask.Pos);
    }

    protected virtual void CalcPath()
    {
        path = aStar.CalcPath(startTile, targetTile, robot.tileGrid, allowedMainTiles, allowedGroundTiles);
    }

    protected virtual void LocationReached()
    {
        ClearValues();
        owner.SwitchState(typeof(RobotWaitState));
    }

    protected void TaskFailed()
    {
        Debug.Log("Task failed");
        ClearValues();
        robot.TaskFailed(robot.currentTask);
    }

    protected void ClearValues()
    {
        startTile = null;
        targetTile = null;
        path = null;
        pathIndex = 0;
    }
}
