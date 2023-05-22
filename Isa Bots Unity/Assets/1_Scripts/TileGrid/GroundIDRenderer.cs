using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundIDRenderer : BaseClass
{
    public TileGrid Grid;
    public int TileAmount;
    public float TexturePixelWidth;
    public float UVFloatErrorMargin;

    //References
    private Mesh mesh;

    //Quads
    private Vector3[] vertices;
    private Vector2[] uv;
    private int[] triangles;

    //UV
    private Vector2[] uv00;
    private Vector2[] uv11;

    public override void OnStart()
    {
        GenerateMesh();
        GenerateQuadCollections();
        GenerateUVs(TileAmount, TexturePixelWidth);
        Draw();
    }

    public override void OnUPS()
    {
        SetMeshBoundToCam();
    }

    public void Draw()
    {
        GenerateQuads();
        UpdateMesh();
    }

    private void GenerateMesh()
    {
        mesh = new Mesh();
        mesh = GetComponent<MeshFilter>().mesh;
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
    }

    private void UpdateMesh()
    {
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }

    private void SetMeshBoundToCam()
    {
        Transform camTransform = Camera.main.transform;
        float distToCenter = (Camera.main.farClipPlane - Camera.main.nearClipPlane) / 2.0f;
        Vector3 center = camTransform.position + camTransform.forward * distToCenter;
        float extremeBound = 500.0f;
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.sharedMesh.bounds = new Bounds(center, new Vector3(1, 1) * extremeBound);
    }

    private void GenerateQuads()
    {
        int i = 0;
        for (int y = 0; y < Grid.Height; y++)
        {
            for (int x = 0; x < Grid.Width; x++)
            {
                int currentTileID = (int)Grid.GetTile(new Vector2Int(x, y)).GroundID;

                //Generate quads
                //Vertices
                int verticesAndUvIndex = i * 4;
                vertices[verticesAndUvIndex + 0] = new Vector3(x, -y);
                vertices[verticesAndUvIndex + 1] = new Vector3(x, -y + 1);
                vertices[verticesAndUvIndex + 2] = new Vector3(x + 1, -y + 1);
                vertices[verticesAndUvIndex + 3] = new Vector3(x + 1, -y);

                //Triangles
                int trianglesIndex = i * 6;
                triangles[trianglesIndex + 0] = verticesAndUvIndex + 0;
                triangles[trianglesIndex + 1] = verticesAndUvIndex + 1;
                triangles[trianglesIndex + 2] = verticesAndUvIndex + 2;
                triangles[trianglesIndex + 3] = verticesAndUvIndex + 0;
                triangles[trianglesIndex + 4] = verticesAndUvIndex + 2;
                triangles[trianglesIndex + 5] = verticesAndUvIndex + 3;

                //Map values to uv
                uv[verticesAndUvIndex + 1] = new Vector2(uv00[currentTileID].x, uv00[currentTileID].y);
                uv[verticesAndUvIndex + 2] = new Vector2(uv11[currentTileID].x, uv00[currentTileID].y);
                uv[verticesAndUvIndex + 0] = new Vector2(uv00[currentTileID].x, uv11[currentTileID].y);
                uv[verticesAndUvIndex + 3] = new Vector2(uv11[currentTileID].x, uv11[currentTileID].y);
                
                i++;
            }
        }
    }

    private void GenerateQuadCollections()
    {
        int quadAmount = Grid.Width * Grid.Height;
        vertices = new Vector3[4 * quadAmount];
        uv = new Vector2[4 * quadAmount];
        triangles = new int[6 * quadAmount];
    }

    private void GenerateUVs(int tileAmount, float texturePixelWidth)
    {
        uv00 = new Vector2[tileAmount];
        uv11 = new Vector2[tileAmount];

        float tileWidthNormalized = (texturePixelWidth / tileAmount) / texturePixelWidth;
        //TileHeight is always 0 and 1

        for (int i = 0; i < tileAmount; i++)
        {
            uv00[i] = new Vector2(tileWidthNormalized * i + UVFloatErrorMargin, 0);
            uv11[i] = new Vector2((tileWidthNormalized * i - UVFloatErrorMargin) + tileWidthNormalized, 1);
        }
    }
}