using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class TileGrid : BaseClass
{
    public int Width; 
    public int Height;
    [Header("Terrain Generation")]
    public MainID[] MainGenerationTiles;
    public GroundID[] GroundGenerationTiles;
    public int MainRandomness; //Add randomness in MainGeneration
    public int PerlinMagnification; //The higher the bigger the area's will be

    [Header("Ore Generation")]
    public int ironAmount; 
    public int ironSize; //Size of vein
    public int tryToGetLocationAmount; //Amount generator tries to find a location
    public int freeSpaceRequired; //Amount of space around a vein needed to spawn the vein

    protected Tile[,] gridArray;

    //Generation
    private int perlinXOffset;
    private int perlinYOffset;
    private int totalStoneAmount;
    private int failedGeneration;

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

    public void SetTile(Vector2Int pos, MainID mainID, GroundID groundID, int temp) 
    { 
        if (IsInGridBounds(pos))
        {
            Tile currentTile = GetTile(pos);

            currentTile.MainID = mainID;
            currentTile.GroundID = groundID;
            currentTile.Temp = temp;
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

    public void SetTiles(Vector2Int pos1, Vector2Int pos2, MainID mainID, GroundID groundID, int temp)
    {
        if (IsInGridBounds(pos1) && IsInGridBounds(pos2))
        {
            for (int y = pos1.y; y < pos2.y; y++)
            {
                for (int x = pos1.x; x < pos2.x; x++)
                {
                    SetTile(new Vector2Int(x, y), mainID, groundID, temp);
                }
            }
        }
        else
        {
            Debug.Log("SetTiles Out of Bounds: " + pos1.x + ", " + pos1.y + " & " + pos2.x + ", " + pos2.y);
        }
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
        //Terrain
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                gridArray[x, y] = new Tile(new Vector2Int(x, y), (MainID)GetPerlinMain(x, y, MainGenerationTiles), (GroundID)GetPerlinGround(x, y, GroundGenerationTiles), 0, 0);
            }
        }

        //Ores
        CalcTotalStoneAmount();
        int ironSpawnAmount = totalStoneAmount / 1000 * ironAmount;

        for (int i = 0; i < ironSpawnAmount; i++)
        {
            Tile spawnLocation = GetOreSpawnLocation();

            if (spawnLocation != null)
            {
                Vector2Int pos1 = new Vector2Int(spawnLocation.Pos.x - (ironSize / 2), spawnLocation.Pos.y - (ironSize / 2));
                Vector2Int pos2 = new Vector2Int(spawnLocation.Pos.x + (ironSize / 2), spawnLocation.Pos.y + (ironSize / 2));
                SetTiles(pos1, pos2, MainID.ironOre, GroundID.dirt, 0);
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

    private void CalcTotalStoneAmount()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (GetTile(new Vector2Int(x, y)).MainID == MainID.stone)
                {
                    totalStoneAmount++;
                }
            }
        }
    }

    private Tile GetOreSpawnLocation()
    {
        Vector2Int isNull = new Vector2Int(-1, -1);

        for (int i = 0; i < tryToGetLocationAmount; i++)
        {
            Vector2Int result = CalcOreSpawnLocation();

            if (result != isNull)
            {
                return GetTile(result);
            }
        }
        failedGeneration++;
        return null;
    }

    private Vector2Int CalcOreSpawnLocation()
    {
        Vector2Int isNull = new Vector2Int(-1, -1);
        int randomX = UnityEngine.Random.Range(0, Width - 1);
        int randomY = UnityEngine.Random.Range(0, Height - 1);

        for (int x = randomX - (freeSpaceRequired / 2); x < randomX + (freeSpaceRequired / 2); x++)
        {
            for (int y = randomY - freeSpaceRequired; y < randomY + freeSpaceRequired; y++)
            {
                if (IsInGridBounds(new Vector2Int(x, y)))
                {
                    if (GetTile(new Vector2Int(x, y)).MainID != MainID.stone)
                    {
                        return isNull;
                    }
                }
                else
                {
                    return isNull;
                }
            }
        }
        return new Vector2Int(randomX, randomY);
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