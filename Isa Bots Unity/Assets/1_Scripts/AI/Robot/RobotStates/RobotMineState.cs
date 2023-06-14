using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class RobotMineState : AIPathFinding
{
    public GameObject MineParticle;

    private Dictionary<MainID, int> mineTimes = new Dictionary<MainID, int>()
    {
        { MainID.stone, 50 },
        { MainID.ironOre, 100 },
        { MainID.stoneWall, 50 },
        { MainID.tree, 25 },
        { MainID.tallGrass, 5 },
        { MainID.flowers, 5 },
    };

    private int targetMineTime;
    private int mineProgress;
    private bool miningComplete;

    private InventoryManager inventory;

    public override void OnAwake()
    {
        inventory = FindObjectOfType<InventoryManager>();
    }

    protected override void LocationReached()
    {
        Mine();

        if (miningComplete)
        {
            ResetValues();
            base.LocationReached();
        }
    }

    protected override void StepTroughPath()
    {
        if (pathIndex < path.Count)
        {
            robot.SetPos(path[pathIndex].Pos);
            robot.tileGrid.AddStepOnTile(path[pathIndex].Pos);
            pathIndex++;
        }
        else
        {
            mineTimes.TryGetValue(targetTile.MainID, out int time);
            targetMineTime = time;
            LocationReached();
        }
    }

    protected override void CalcPath()
    {
        //Choose random side
        int trys = 0;
        for (int i = Random.Range(0, 3); trys < 10; trys++)
        {
            int direction = i;

            if (direction == 0) //up
            {
                Tile upTile = robot.tileGrid.GetTile(new Vector2Int(targetTile.Pos.x, targetTile.Pos.y - 1));
                TryToCalcPath(upTile,direction);
            }
            else if (direction == 1) //Right
            {
                Tile rightTile = robot.tileGrid.GetTile(new Vector2Int(targetTile.Pos.x + 1, targetTile.Pos.y));
                TryToCalcPath(rightTile, direction);
            }
            else if (direction == 2) //Down
            {
                Tile downTile = robot.tileGrid.GetTile(new Vector2Int(targetTile.Pos.x, targetTile.Pos.y + 1));
                TryToCalcPath(downTile, direction);
            }
            else if (direction == 3) //Left
            {
                Tile leftTile = robot.tileGrid.GetTile(new Vector2Int(targetTile.Pos.x - 1, targetTile.Pos.y));
                TryToCalcPath(leftTile, direction);
            }

            if (path != null) // if path is found go out of while loop
            {
                i = 10;
            }
            else
            {
                i = Random.Range(0, 3);
            }
        }
    }

    private void Mine()
    {
        mineProgress++;
        Instantiate(MineParticle, new Vector3(targetTile.Pos.x, -targetTile.Pos.y, -6), Quaternion.identity);

        if (mineProgress >= targetMineTime)
        {
            MainID newResource = robot.tileGrid.GetTile(targetTile.Pos).MainID;
            inventory.AddResource(newResource);
            robot.tileGrid.SetTile(targetTile.Pos, MainID.none, targetTile.GroundID, true);
            miningComplete = true;
        }
    }

    private void TryToCalcPath(Tile currentTile, int currentDirection)
    {
        if (currentTile != null)
        {
            if (currentTile.MainID == MainID.none)
            {
                path = aStar.CalcPath(startTile, currentTile, robot.tileGrid, allowedMainTiles, allowedGroundTiles);
            }
            else
            {
                path = null;
            }
        }
    }

    private void ResetValues()
    {
        targetMineTime = 0;
        mineProgress = 0;
        miningComplete = false;
    }
}
