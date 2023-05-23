using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RobotMineState : AIPathFinding
{
    protected override void LocationReached()
    {
        Mine();
        base.LocationReached();
    }

    protected void Mine()
    {
        robot.tileGrid.SetTile(targetTile.Pos, MainID.none, targetTile.GroundID, targetTile.Temp);
    }

    protected override void CalcPath()
    {
        List<int> directions = new List<int>
        {
            { 0 }, { 1 }, { 2 }, { 3 }
        };

        //Choose random side
        while (directions.Count > 0)
        {
            int direction = directions[Random.Range(0, directions.Count)];

            if (direction == 0) //up
            {
                path = aStar.CalcPath(robot.tileGrid.GetTile(new Vector2Int(robot.Pos.x, robot.Pos.y - 1)), targetTile, robot.tileGrid, allowedMainTiles, allowedGroundTiles);
                directions.Remove(direction);
            }
            else if (direction == 1) //Right
            {
                path = aStar.CalcPath(robot.tileGrid.GetTile(new Vector2Int(robot.Pos.x + 1, robot.Pos.y)), targetTile, robot.tileGrid, allowedMainTiles, allowedGroundTiles);
                directions.Remove(direction);
            }
            else if (direction == 2) //Down
            {
                path = aStar.CalcPath(robot.tileGrid.GetTile(new Vector2Int(robot.Pos.x, robot.Pos.y + 1)), targetTile, robot.tileGrid, allowedMainTiles, allowedGroundTiles);
                directions.Remove(direction);
            }
            else if (direction == 3) //Left
            {
                path = aStar.CalcPath(robot.tileGrid.GetTile(new Vector2Int(robot.Pos.x - 1, robot.Pos.y)), targetTile, robot.tileGrid, allowedMainTiles, allowedGroundTiles);
                directions.Remove(direction);
            }

            if (path != null) // if path is found go out of while loop
            {
                directions.Clear();
            }
        }

        if (path == null)
        {
            Debug.Log("No room around tile");
        }
    }
}
