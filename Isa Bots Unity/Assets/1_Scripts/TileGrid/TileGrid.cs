using System;
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
    public int ironVeinSize;

    [Header("Vegetation")]
    public int TreeAmount;
    public int TallGrassAmount;
    public int FlowersAmount;
    public int StepsOnGrassToDie;

    [Header("Sprites")]
    public GameObject SpriteParent;
    public GameObject TreeSprite;
    public GameObject TallGrassSprite;
    public GameObject FlowersSprite;
    public GameObject FactorySprite;
    private Dictionary<MainID, GameObject> spriteDictionary = new Dictionary<MainID, GameObject>();
    private Dictionary<Vector2Int, GameObject> currentSprites = new Dictionary<Vector2Int, GameObject>();

    protected Tile[,] gridArray;

    //Generation
    private int perlinXOffset;
    private int perlinYOffset;
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

        //Dictionary's
        spriteDictionary.Add(MainID.tree, TreeSprite);
        spriteDictionary.Add(MainID.tallGrass, TallGrassSprite);
        spriteDictionary.Add(MainID.flowers, FlowersSprite);
        spriteDictionary.Add(MainID.factory, FactorySprite);

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

    public void SetTile(Vector2Int pos, MainID mainID, GroundID groundID, bool redraw) 
    { 
        if (IsInGridBounds(pos))
        {
            Tile currentTile = GetTile(pos);

            //Remove or add Sprite
            if (currentTile.MainID != mainID)
            {
                if (spriteDictionary.ContainsKey(mainID)) //If new MainID containes a sprite
                {
                    spriteDictionary.TryGetValue(mainID, out GameObject newSprite);
                    AddSprite(newSprite, pos);
                }

                if (spriteDictionary.ContainsKey(currentTile.MainID)) //If old MainID containes a sprite
                {
                    RemoveSprite(pos);
                }
            }

            currentTile.MainID = mainID;
            currentTile.GroundID = groundID;

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

    public void SetTiles(Vector2Int pos1, Vector2Int pos2, MainID mainID, GroundID groundID, bool redraw)
    {
        if (IsInGridBounds(pos1) && IsInGridBounds(pos2))
        {
            for (int y = pos1.y; y < pos2.y; y++)
            {
                for (int x = pos1.x; x < pos2.x; x++)
                {
                    SetTile(new Vector2Int(x, y), mainID, groundID, false);
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

    public void AddStepOnTile(Vector2Int pos)
    {
        Tile currentTile = GetTile(pos);
        currentTile.StepCount++;

        if (currentTile.StepCount >= StepsOnGrassToDie && currentTile.GroundID == GroundID.grass)
        {
            currentTile.GroundID = GroundID.dirt;
            RedrawGrid();
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

        GenerateOres(); //SetTile out of bounds??
        GenerateVegetation();

        SetTiles(new Vector2Int(0, 0), new Vector2Int(10, 10), MainID.none, GroundID.grass, false); //Clear Starter Area
    }


    private void GenerateTerrain()
    {
        GeneratePerlinTerrain();
        GenerateStoneWalls();
    }

    private void GenerateOres()
    {
        //Iron
        for (int i = 0; i < NormalizeSpawnAmount(ironAmount); i++)
        {
            SpawnVein(MainID.ironOre, ironVeinSize);
        }
    }

    private void GenerateVegetation()
    {
        //Trees
        for (int i = 0; i < NormalizeSpawnAmount(TreeAmount); i++)
        {
            GeneratePlant(MainID.tree, TreeSprite);
        }

        //TallGrass
        for (int i = 0; i < NormalizeSpawnAmount(TallGrassAmount); i++)
        {
            GeneratePlant(MainID.tallGrass, TallGrassSprite);
        }

        //Flowers
        for (int i = 0; i < NormalizeSpawnAmount(FlowersAmount); i++)
        {
            GeneratePlant(MainID.flowers, FlowersSprite);
        }
    }

    private void GeneratePlant(MainID plant, GameObject prefab)
    {
        Tile currentTile = GetRandomTile(true, 1);
        SetTile(currentTile.Pos, plant, GroundID.grass, false);
    }

    private void SpawnVein(MainID id, int veinSize)
    {
        veinSize /= 2;
        Tile currentTile = GetRandomTile(false, veinSize);

        if (currentTile != null)
        {
            Vector2Int pos1 = new Vector2Int(currentTile.Pos.x - veinSize, currentTile.Pos.y - veinSize);
            Vector2Int pos2 = new Vector2Int(currentTile.Pos.x + veinSize, currentTile.Pos.y + veinSize);

            SetTiles(pos1, pos2, id, currentTile.GroundID, false);
        }
        else
        {
            failedGeneration++;
        }
    }

    private int NormalizeSpawnAmount(int spawnAmount)
    {
        return spawnAmount * (Width * Height) / 1000;
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

    private void AddSprite(GameObject sprite, Vector2Int pos)
    {
        GameObject newSprite = Instantiate(sprite, new Vector3(pos.x, -pos.y, 0), quaternion.identity);
        newSprite.transform.SetParent(SpriteParent.transform);
        currentSprites.Add(pos, newSprite);
    }

    private void RemoveSprite(Vector2Int pos)
    {
        if (currentSprites.TryGetValue(pos, out GameObject sprite))
        {
            currentSprites.Remove(pos);
            Destroy(sprite);
        }
    }


    private void GeneratePerlinTerrain()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                gridArray[x, y] = new Tile(new Vector2Int(x, y), (MainID)GetPerlinMain(x, y, MainGenerationTiles), GetPerlinGround(x, y, GroundGenerationTiles));
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
                        SetTile(currentTile.Pos, MainID.stoneWall, currentTile.GroundID, false);
                    }
                }
            }
        }
    }

    private MainID GetPerlinMain(float x, float y, MainID[] generationTiles)
    {
        float offsetPerlin = Mathf.PerlinNoise((x + perlinXOffset) / PerlinMagnification, (y + perlinYOffset) / PerlinMagnification); //Generate perlin with offset
        float tileIDPerlin = Mathf.Clamp(offsetPerlin, 0.0f, 1.0f) * generationTiles.Length; //Clamp all values between 0.0 and 1.0 and multiply with tileAmount
        tileIDPerlin = Mathf.Clamp(tileIDPerlin, 0.0f, generationTiles.Length); //Clamp between 0 and tileAmount
        return (MainID)Mathf.FloorToInt(tileIDPerlin); //Return clostest GroundID
    }

    private GroundID GetPerlinGround(float x, float y, GroundID[] generationTiles)
    {
        float offsetPerlin = Mathf.PerlinNoise((x + perlinXOffset) / PerlinMagnification, (y + perlinYOffset) / PerlinMagnification); //Generate perlin with offset
        float tileIDPerlin = Mathf.Clamp(offsetPerlin, 0.0f, 1.0f) * generationTiles.Length; //Clamp all values between 0.0 and 1.0 and multiply with tileAmount
        tileIDPerlin = Mathf.Clamp(tileIDPerlin, 0.0f, generationTiles.Length); //Clamp between 0 and tileAmount
        return (GroundID)Mathf.FloorToInt(tileIDPerlin); //Return clostest GroundID
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