using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileGrid : BaseClass
{
    public int Width; 
    public int Height;
    [Header("Terrain Generation")]
    public MainID[] MainGenerationTiles;
    public GroundID[] GroundGenerationTiles;
    public GameObject Tree;
    public int MainRandomness; //Add randomness in MainGeneration
    public int PerlinMagnification; //The higher the bigger the area's will be

    [Header("Ore Generation")]
    public int ironAmount; 
    public int ironVeinSize;

    protected Tile[,] gridArray;

    //Generation
    private int perlinXOffset;
    private int perlinYOffset;
    private int totalStoneAmount;
    private int failedGeneration;

    //References
    private MainIDRenderer mainIDRenderer;
    private GroundIDRenderer groundIDRenderer;

    public override void OnAwake()
    {
        mainIDRenderer = FindObjectOfType<MainIDRenderer>(); 
        groundIDRenderer = FindObjectOfType<GroundIDRenderer>();
    }

    public override void OnStart()
    {
        gridArray = new Tile[Width, Height];
        perlinXOffset = UnityEngine.Random.Range(-100000, 100000);
        perlinYOffset = UnityEngine.Random.Range(-100000, 100000);

        GenerateWorld();
        PrintGridSize();
    }

    //---------------------------------------------------------------------

    public Tile GetTile(Vector2Int pos) 
    {
        if (IsInGridBounds(pos))
        {
            return gridArray[pos.x, pos.y]; 
        }
        return null;
    }

    public Tile GetRandomTile(bool locationMustBeEmpty, int surroundingFreeSpace)
    {
        for (int i = 0; i < 10000; i++)
        {
            int randomX = UnityEngine.Random.Range(0, Width - 1);
            int randomY = UnityEngine.Random.Range(0, Height - 1);

            if (CheckTile(randomX, randomY, locationMustBeEmpty, surroundingFreeSpace))
            {
                return GetTile(new Vector2Int(randomX, randomY));
            }
        }
        return null;
    }

    public void SetTile(Vector2Int pos, MainID mainID, GroundID groundID, int temp, bool redraw) 
    { 
        if (IsInGridBounds(pos))
        {
            Tile currentTile = GetTile(pos);

            currentTile.MainID = mainID;
            currentTile.GroundID = groundID;
            currentTile.Temp = temp;

            if (redraw)
            {
                RedrawGrid();
            }
        }
        else
        {
            Debug.Log("SetTile Out of Bounds: " + pos.x + ", " + pos.y);
        }
    }

    public void SetTile(Tile currentTile, MainID mainID, GroundID groundID, int temp)
    {
        if (IsInGridBounds(currentTile.Pos))
        {
            currentTile.MainID = mainID;
            currentTile.GroundID = groundID;
            currentTile.Temp = temp;
        }
        else
        {
            Debug.Log("SetTile Out of Bounds: " + currentTile.Pos.x + ", " + currentTile.Pos.y);
        }
    }

    public void SetTiles(Vector2Int pos1, Vector2Int pos2, MainID mainID, GroundID groundID, int temp, bool redraw)
    {
        if (IsInGridBounds(pos1) && IsInGridBounds(pos2))
        {
            for (int y = pos1.y; y < pos2.y; y++)
            {
                for (int x = pos1.x; x < pos2.x; x++)
                {
                    SetTile(new Vector2Int(x, y), mainID, groundID, temp, false);
                }
            }

            if (redraw)
            {
                RedrawGrid();
            }
        }
        else
        {
            Debug.Log("SetTiles Out of Bounds: " + pos1.x + ", " + pos1.y + " & " + pos2.x + ", " + pos2.y);
        }
    }

    public void RedrawGrid()
    {
        GenerateStoneWalls(); //Extremely inefficient here (make extra function for one tile only)
        
        mainIDRenderer.Draw();
        groundIDRenderer.Draw();
    }

    public bool IsInGridBounds(Vector2Int pos)
    {
        if (pos.x >= 0 && pos.x <= Width - 1 && pos.y >= 0 && pos.y <= Height - 1)
        {
            return true;
        }
        return false;
    }

    //-----------------------------------------------------------

    private void GenerateWorld()
    {
        GenerateTerrain();
        GenerateOres();
        GenerateVegetation();
        
        SetTiles(new Vector2Int(0, 0), new Vector2Int(10, 10), MainID.none, GroundID.dirt, 0, false); //Clear Starter Area
    }


    private void GenerateTerrain()
    {
        GeneratePerlinTerrain();
        GenerateStoneWalls();
    }

    private void GenerateOres()
    {
        CalcTotalStoneAmount();

        //Iron
        for (int i = 0; i < NormalizeOreAmount(ironAmount); i++)
        {
            SpawnVein(MainID.ironOre, ironVeinSize);
        }
    }

    private void GenerateVegetation()
    {
        //need to implement
    }

    private void SpawnVein(MainID id, int veinSize)
    {
        veinSize /= 2;
        Tile currentTile = GetRandomTile(false, veinSize);

        if (currentTile != null)
        {
            Vector2Int pos1 = new Vector2Int(currentTile.Pos.x - veinSize, currentTile.Pos.y - veinSize);
            Vector2Int pos2 = new Vector2Int(currentTile.Pos.x + veinSize, currentTile.Pos.y + veinSize);

            SetTiles(pos1, pos2, id, currentTile.GroundID, currentTile.Temp, false);
        }
        else
        {
            failedGeneration++;
        }
    }

    private int NormalizeOreAmount(int oreAmount)
    {
        return oreAmount * totalStoneAmount / 1000;
    }

    private void CalcTotalStoneAmount()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Tile currentTile = GetTile(new Vector2Int(x, y));

                if (currentTile.MainID == MainID.stone || currentTile.MainID == MainID.stoneWall)
                {
                    totalStoneAmount++;
                }
            }
        }
    }

    private bool CheckTile(int _x, int _y, bool locationMustBeEmpty, int surroundingFreeSpace)
    {
        for (int x = _x - surroundingFreeSpace; x < _x + surroundingFreeSpace; x++)
        {
            for (int y = _y - surroundingFreeSpace; y < _y + surroundingFreeSpace; y++)
            {
                if (IsInGridBounds(new Vector2Int(x, y)))
                {
                    Tile currentTile = GetTile(new Vector2Int(x, y));

                    //Check if Tile is available (empty or not)
                    if (locationMustBeEmpty && currentTile.MainID != MainID.none)
                    {
                        return false;
                    }
                    else if (!locationMustBeEmpty && currentTile.MainID == MainID.none)
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    private void GeneratePerlinTerrain()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                gridArray[x, y] = new Tile(new Vector2Int(x, y), (MainID)GetPerlinMain(x, y, MainGenerationTiles), (GroundID)GetPerlinGround(x, y, GroundGenerationTiles), 0, 0);
            }
        }
    }

    private void GenerateStoneWalls()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (IsInGridBounds(new Vector2Int(x, y + 1)))
                {
                    Tile currentTile = GetTile(new Vector2Int(x, y));
                    Tile downTile = GetTile(new Vector2Int(x, y + 1));

                    if (currentTile.MainID == MainID.stone && downTile.MainID == MainID.none)
                    {
                        currentTile.MainID = MainID.stoneWall;
                    }
                }
            }
        }
    }

    private int GetPerlinGround(float x, float y, GroundID[] generationTiles)
    {
        float offsetPerlin = Mathf.PerlinNoise((x + perlinXOffset) / PerlinMagnification, (y + perlinYOffset) / PerlinMagnification); //Generate perlin with offset
        float tileIDPerlin = Mathf.Clamp(offsetPerlin, 0.0f, 1.0f) * generationTiles.Length; //Clamp all values between 0.0 and 1.0 and multiply with tileAmount
        tileIDPerlin = Mathf.Clamp(tileIDPerlin, 0.0f, generationTiles.Length); //Clamp between 0 and tileAmount
        return Mathf.FloorToInt(tileIDPerlin); //Return int
    }

    private int GetPerlinMain(float x, float y, MainID[] generationTiles)
    {
        float offsetPerlin = Mathf.PerlinNoise((x + perlinXOffset + UnityEngine.Random.Range(-MainRandomness, MainRandomness)) / (PerlinMagnification - 1), (y + perlinYOffset + UnityEngine.Random.Range(-MainRandomness, MainRandomness)) / (PerlinMagnification - 1)); //Generate perlin with offset
        float tileIDPerlin = Mathf.Clamp(offsetPerlin, 0.0f, 1.0f) * generationTiles.Length; //Clamp all values between 0.0 and 1.0 and multiply with tileAmount
        tileIDPerlin = Mathf.Clamp(tileIDPerlin, 0.0f, generationTiles.Length); //Clamp between 0 and tileAmount
        return Mathf.FloorToInt(tileIDPerlin); //Return int
    }

    private void PrintGridSize()
    {
        if (gridArray.Length < 1000000)
        {
            Debug.Log(gameObject.name + " size: " + gridArray.Length / 1000 + "K");
        }
        else
        {
            Debug.Log(gameObject.name + " size: " + gridArray.Length / 1000000 + "M");
        }
        Debug.Log("Failed generation: " + failedGeneration);
    }
}