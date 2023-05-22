using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AStar
{
    //A self-written attempt at AStar
    public List<Tile> CalcPath(Tile startTile, Tile targetTile, TileGrid grid, List<MainID> allowedTiles)
    {
        Dictionary<Tile, int> fCost = new Dictionary<Tile, int>(); //Tiles with calculated fCost
        Dictionary<Tile, Tile> parent = new Dictionary<Tile, Tile>(); //Parent of tile
        List<Tile> pendingTiles = new List<Tile>();
        List<Tile> path = new List<Tile>(); //List of tiles with fastest path

        Tile currentTile = startTile;

        //Calc all lowest fCosts until targetTile
        while (currentTile != targetTile)
        {
            CalcSurroundingTiles(currentTile, targetTile, startTile, fCost, pendingTiles, grid, allowedTiles, parent);
            currentTile = GetLowestPending(pendingTiles, fCost);
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

    private Tile GetLowestPending(List<Tile> pendingTiles, Dictionary<Tile, int> fCost)
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

    private void CalcSurroundingTiles(Tile currentTile, Tile targetTile, Tile startTile, Dictionary<Tile, int> fCost, List<Tile> pendingTiles, TileGrid grid, List<MainID> allowedTiles, Dictionary<Tile, Tile> parent)
    {
        Vector2Int currentTilePos = currentTile.Pos;
        //up
        Tile upTile = grid.GetTile(new Vector2Int(currentTilePos.x, currentTilePos.y - 1));
        CalcTileCost(upTile, currentTile, targetTile, startTile, fCost, pendingTiles, allowedTiles, parent);

        //right
        Tile rightTile = grid.GetTile(new Vector2Int(currentTilePos.x + 1, currentTilePos.y));
        CalcTileCost(rightTile, currentTile, targetTile, startTile, fCost, pendingTiles, allowedTiles, parent);

        //down
        Tile downTile = grid.GetTile(new Vector2Int(currentTilePos.x, currentTilePos.y + 1));
        CalcTileCost(downTile, currentTile, targetTile, startTile, fCost, pendingTiles, allowedTiles, parent);

        //left
        Tile leftTile = grid.GetTile(new Vector2Int(currentTilePos.x - 1, currentTilePos.y));
        CalcTileCost(leftTile, currentTile, targetTile, startTile, fCost, pendingTiles, allowedTiles, parent);

        pendingTiles.Remove(currentTile);
    }

    private void CalcTileCost(Tile currentTile, Tile parentTile, Tile targetTile, Tile startTile, Dictionary<Tile, int> fCost, List<Tile> pendingTiles, List<MainID> allowedTiles, Dictionary<Tile, Tile> parent)
    {
        if (currentTile == null) return;
        if (!fCost.ContainsKey(currentTile) && allowedTiles.Contains(currentTile.MainID)) //if value is not calculated & isWalkable
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
}