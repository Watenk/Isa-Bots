using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//A self-written attempt at AStar

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

    //References
    private TileGrid tileGrid;

    public List<Tile> CalcPath(Tile startTile, Tile targetTile, TileGrid tileGrid, List<MainID> allowedMainTiles, List<GroundID> allowedGroundTiles)
    {
        //Set values
        this.startTile = startTile;
        this.targetTile = targetTile;
        this.tileGrid = tileGrid;
        this.allowedMainTiles = allowedMainTiles;
        this.allowedGroundTiles = allowedGroundTiles;
        Tile currentTile = startTile;

        //Calc all lowest fCosts until targetTile
        while (currentTile != targetTile)
        {
            CalcSurroundingTiles(currentTile, tileGrid);
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

    private void CalcSurroundingTiles(Tile currentTile, TileGrid tileGrid)
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

    private void CalcTileCost(Tile currentTile, Tile parentTile)
    {
        if (currentTile == null) return;
        if (!fCost.ContainsKey(currentTile) && allowedMainTiles.Contains(currentTile.MainID) && allowedGroundTiles.Contains(currentTile.GroundID)) //if value is not calculated & isWalkable
        {
            Vector2Int currentTilePos = currentTile.Pos;
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

            fCost.Add(currentTile, fCostInt);
            parent.Add(currentTile, parentTile);
            pendingTiles.Add(currentTile);
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