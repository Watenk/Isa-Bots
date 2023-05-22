using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotMineState : BaseState
{


    public override void OnStart()
    {
        //List<int> directions = new List<int>
        //{
        //    { 0 }, { 1 }, { 2 }, { 3 }
        //};

        ////Choose random side
        //while (directions.Count > 0)
        //{
        //    int direction = directions[Random.Range(0, directions.Count)];

        //    if (direction == 0) //up
        //    {
        //        robot.Path = robot.AStar.CalcPath(tileGrid.GetTile(new Vector2Int(robot.Pos.x, robot.Pos.y - 1)), tileGrid.GetTile(TargetPos), tileGrid, robot.AllowedMainTiles, robot.AllowedGroundTiles);
        //    }
        //    else if (direction == 1) //Right
        //    {
        //        robot.Path = robot.AStar.CalcPath(tileGrid.GetTile(new Vector2Int(robot.Pos.x + 1, robot.Pos.y)), tileGrid.GetTile(TargetPos), tileGrid, robot.AllowedMainTiles, robot.AllowedGroundTiles);
        //    }
        //    else if (direction == 2) //Down
        //    {
        //        robot.Path = robot.AStar.CalcPath(tileGrid.GetTile(new Vector2Int(robot.Pos.x, robot.Pos.y + 1)), tileGrid.GetTile(TargetPos), tileGrid, robot.AllowedMainTiles, robot.AllowedGroundTiles);
        //    }
        //    else if (direction == 3) //Left
        //    {
        //        robot.Path = robot.AStar.CalcPath(tileGrid.GetTile(new Vector2Int(robot.Pos.x - 1, robot.Pos.y)), tileGrid.GetTile(TargetPos), tileGrid, robot.AllowedMainTiles, robot.AllowedGroundTiles);
        //    }

        //    if (robot.Path != null)
        //    {
        //        directions.Clear();
        //    }
        //}

        //if (robot.Path == null)
        //{
        //    //Target Unreachable
        //    tasks.AddTask(robotTaskState.currentTask);
        //    robotTaskState.currentTask = null;
        //    owner.SwitchState(typeof(RobotWaitState));
        //}
    }

    public override void OnUPS()
    {
        //if (robot.pathIndex !> robot.Path.Count)
        //{
        //    //Take step
        //    robot.Pos = robot.Path[robot.pathIndex].Pos;
        //    robot.pathIndex++;
        //}
        //else
        //{
        //    //Loation reached
        //    robot.TaskDone();
        //    owner.SwitchState(typeof(RobotWaitState));
        //}
    }

    public override void OnExit()
    {

    }
}
