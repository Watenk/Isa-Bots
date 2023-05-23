using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//A self-written attempt at AStar
//It takes some weird routes?? need to fix that

public class AStar
{
    private Tile startTile;
    private Tile targetTile;
    private List<MainID> allowedMainTiles;
    private List<GroundID> allowedGroundTiles;
    private Dictionary<Tile, int> fCost = new Dictionary<Tile, int>(); //Tiles with calculated fCost
    private Dictionary<Tile, Tile> parent = new Dictionary<Tile, Tile>(); //Parent of tile
    private List<Tile> pendingTiles = new List<Tile>();
    private List<Tile> path = new List<Tile>(); //List of tiles with fastest path

    private TileGrid tileGrid;

    public List<Tile> CalcPath(Tile startTile, Tile targetTile, TileGrid tileGrid, List<MainID> allowedMainTiles, List<GroundID> allowedGroundTiles)
    {
        //(Re)Set values
        Tile currentTile = startTile;
        this.startTile = startTile;
        this.targetTile = targetTile;
        this.tileGrid = tileGrid;
        this.allowedMainTiles = allowedMainTiles;
        this.allowedGroundTiles = allowedGroundTiles;
        fCost.Clear();
        parent.Clear();
        pendingTiles.Clear();
        path.Clear();

        //Calc all lowest fCosts until targetTile
        while (currentTile != targetTile)
        {
            CalcSurroundingTiles(currentTile);
            currentTile = GetLowestPending();
            if (currentTile == null) { return null; } //Return null if cant reach target
        }

        //Retrace fastest path from targetTile to startTile
        while (currentTile != startTile)
        {
            path.Add(currentTile);
            parent.TryGetValue(currentTile, out Tile newTile);
            currentTile = newTile;
        }

        path.Reverse();
        return path;
    }

    private void CalcSurroundingTiles(Tile currentTile)
    {
        Vector2Int currentTilePos = currentTile.Pos;
        //up
        Tile upTile = tileGrid.GetTile(new Vector2Int(currentTilePos.x, currentTilePos.y - 1));
        CalcTileCost(upTile, currentTile);

        //right
        Tile rightTile = tileGrid.GetTile(new Vector2Int(currentTilePos.x + 1, currentTilePos.y));
        CalcTileCost(rightTile, currentTile);

        //down
        Tile downTile = tileGrid.GetTile(new Vector2Int(currentTilePos.x, currentTilePos.y + 1));
        CalcTileCost(downTile, currentTile);

        //left
        Tile leftTile = tileGrid.GetTile(new Vector2Int(currentTilePos.x - 1, currentTilePos.y));
        CalcTileCost(leftTile, currentTile);

        pendingTiles.Remove(currentTile);
    }

    private void CalcTileCost(Tile directionTile, Tile parentTile)
    {
        if (directionTile == null) return;
        if (!fCost.ContainsKey(directionTile) && allowedMainTiles.Contains(directionTile.MainID) && allowedGroundTiles.Contains(directionTile.GroundID)) //if value is not calculated & isWalkable
        {
            Vector2Int currentTilePos = directionTile.Pos;
            Vector2Int startTilePos = startTile.Pos;
            Vector2Int targetTilePos = targetTile.Pos;

            //CalcGCost
            int xDifferenceGCost = Mathf.Abs(currentTilePos.x - startTilePos.x);
            int yDifferenceGCost = Mathf.Abs(currentTilePos.y - startTilePos.y);
            int gCostInt = xDifferenceGCost + yDifferenceGCost;

            //CalcHCost
            int xDifferenceHCost = Mathf.Abs(currentTilePos.x - targetTilePos.x);
            int yDifferenceHCost = Mathf.Abs(currentTilePos.y - targetTilePos.y);
            int hCostInt = xDifferenceHCost + yDifferenceHCost;

            //CalcFCost
            int fCostInt = gCostInt + hCostInt;

            fCost.Add(directionTile, fCostInt);
            parent.Add(directionTile, parentTile);
            pendingTiles.Add(directionTile);
        }
    }

    private Tile GetLowestPending()
    {
        int lowestValue = 100000;
        Tile currentTile = null;

        for (int i = 0; i < pendingTiles.Count; i++)
        {
            fCost.TryGetValue(pendingTiles[i], out int value);
            if (value < lowestValue)
            {
                lowestValue = value;
                currentTile = pendingTiles[i];
            }
            else if (value == lowestValue)
            {
                int randomInt = Random.Range(0, 100);
                if (randomInt >= 50)
                {
                    currentTile = pendingTiles[i];
                }
            }
        }

        return currentTile;
    }
}