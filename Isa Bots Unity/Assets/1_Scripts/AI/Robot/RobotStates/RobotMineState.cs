using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
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
        robot.tileGrid.SetTile(targetTile.Pos, MainID.none, targetTile.GroundID, targetTile.Temp, true);
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
                path = aStar.CalcPath(startTile, robot.tileGrid.GetTile(new Vector2Int(targetTile.Pos.x, targetTile.Pos.y - 1)), robot.tileGrid, allowedMainTiles, allowedGroundTiles);
                directions.Remove(direction);
            }
            else if (direction == 1) //Right
            {
                path = aStar.CalcPath(startTile, robot.tileGrid.GetTile(new Vector2Int(targetTile.Pos.x + 1, targetTile.Pos.y)), robot.tileGrid, allowedMainTiles, allowedGroundTiles);
                directions.Remove(direction);
            }
            else if (direction == 2) //Down
            {
                path = aStar.CalcPath(startTile, robot.tileGrid.GetTile(new Vector2Int(targetTile.Pos.x, targetTile.Pos.y + 1)), robot.tileGrid, allowedMainTiles, allowedGroundTiles);
                directions.Remove(direction);
            }
            else if (direction == 3) //Left
            {
                path = aStar.CalcPath(startTile, robot.tileGrid.GetTile(new Vector2Int(targetTile.Pos.x - 1, targetTile.Pos.y)), robot.tileGrid, allowedMainTiles, allowedGroundTiles);
                directions.Remove(direction);
            }

            if (path != null) // if path is found go out of while loop
            {
                directions.Clear();
            }
        }
    }
}
