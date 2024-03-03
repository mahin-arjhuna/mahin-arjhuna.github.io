using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMap : MonoBehaviour
{
    // Public class variables
    public Tilemap tilemap;
    public UnityEngine.Tilemaps.Tile[] unityTiles;
    public ParameterManager parameterManager;
    // Private class varaibles
    private Tile[,] tiles;
    private float[,] values;
    private Vector2Int xBounds;
    private int width;
    private Vector2Int yBounds;
    private int height;
    private int numVertexTypes;
    private int numTileTypes;
    private string[] spriteNamesFromVertices;
    private Vertices[] verticesFromSpriteNames;

    // Initialize tilemap
    void Start()
    {
        xBounds = new Vector2Int(tilemap.cellBounds.min.x, tilemap.cellBounds.max.x);
        width = xBounds.y - xBounds.x;
        yBounds = new Vector2Int(tilemap.cellBounds.min.y, tilemap.cellBounds.max.y);
        height = yBounds.y - yBounds.x;

        tiles = new Tile[width, height];
        values = new float[width + 1, height + 1];

        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                tiles[i, j] = new Tile(i - xBounds.x, j - yBounds.x, new Vertices());
            }
        }

        numVertexTypes = 2;
        numTileTypes = (int)Mathf.Pow(numVertexTypes, 4);

        InitializeSpriteNamesArray();
        InitializeVerticesArray();
    }

    // Update all tiles in tilemap
    void Update()
    {
    }

    public void GenerateTileMap()
    {
        int seed = parameterManager.GetSeed();
        float terrainSize = parameterManager.GetNoisePeriod();
        float seaLevel = parameterManager.GetSeaLevel();

        Debug.Log(seed + ", " + terrainSize + ", " + seaLevel);

        PerlinNoise2D noise = new(seed, 256);
        for (int i = 0; i <= width; ++i)
        {
            for (int j = 0; j <= height; ++j)
            {
                values[i, j] = noise.Value(i / terrainSize, j / terrainSize);
            }
        }

        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                Vertices vertices = GetVertex(i, j);
                vertices.Set(0, (values[i    , j + 1] <= seaLevel) ? 0 : 1);
                vertices.Set(1, (values[i + 1, j + 1] <= seaLevel) ? 0 : 1);
                vertices.Set(2, (values[i    , j    ] <= seaLevel) ? 0 : 1);
                vertices.Set(3, (values[i + 1, j    ] <= seaLevel) ? 0 : 1);
                SetVertex(i, j, vertices);
            }
        }
    }

    public void UpdateSeaLevel()
    {
        float seaLevel = parameterManager.GetSeaLevel();
        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                Vertices vertices = GetVertex(i, j);
                vertices.Set(0, (values[i    , j + 1] <= seaLevel) ? 0 : 1);
                vertices.Set(1, (values[i + 1, j + 1] <= seaLevel) ? 0 : 1);
                vertices.Set(2, (values[i    , j    ] <= seaLevel) ? 0 : 1);
                vertices.Set(3, (values[i + 1, j    ] <= seaLevel) ? 0 : 1);
                SetVertex(i, j, vertices);
            }
        }
    }

    public Vertices GetVertex(int x, int y)
    {
        return tiles[x, y].vertices;
    }

    public void SetVertex(int x, int y, Vertices vertices)
    {
        tiles[x, y].vertices = vertices;
        tiles[x, y].name = GetSpriteNameFromVertices(vertices);
        string indexStr = tiles[x, y].name.Substring(7); // sprite_
        int index = int.Parse(indexStr);

        Vector3Int v = new Vector3Int(xBounds.x + x, yBounds.x + y, 0);
        tilemap.SetTile(v, unityTiles[index]);
    }

    public Sprite GetSprite(int x, int y)
    {
        int x_ = tiles[x, y].x;
        int y_ = tiles[x, y].y;
        Vector3Int v = new Vector3Int(x_, y_, 0);
        TileBase t = tilemap.GetTile(v);
        TileData data = new ();
        t.GetTileData(v, tilemap, ref data);
        return data.sprite;
    }

    private void InitializeSpriteNamesArray()
    {
        spriteNamesFromVertices = new string[numTileTypes];
        for (int i = 0; i < numTileTypes; ++i)
        {
            string name = "sprite_";
            if (i < 10)
                name += '0';
            name += i;
            spriteNamesFromVertices[i] = name;
        }
    }

    private void InitializeVerticesArray()
    {
        verticesFromSpriteNames = new Vertices[numTileTypes];
        for (int i = 0; i < numTileTypes; ++i)
        {
            Vertices v = new ();
            DecimalToBaseN(i, ref v);
            verticesFromSpriteNames[i] = v;
        }
    }

    private void DecimalToBaseN(int value, ref Vertices v)
    {
        // Error checking
        if (value >= (int)Mathf.Pow(numVertexTypes, v.size))
        {
            print("Value " + value + " (base " + numVertexTypes + ") cannot be represented with " + v.size + " bits");
            for (int i = 0; i < v.size; ++i) v.Set(i, -1);
            return;
        }

        for (int i = 0; i < v.size; ++i)
        {
            for (int j = numVertexTypes - 1; j >= 0; --j)
            {
                int current = (int)Mathf.Pow(numVertexTypes, v.size - i - 1) * j;
                if (value >= current)
                {
                    v.Set(i, j);
                    value -= current;
                    break;
                }
            }
        }
    }

    private string GetSpriteNameFromVertices(Vertices v)
    {
        // index = v0*n^3 + v1*n^2 + v2*n + v3
        int index = 0;
        for (int i = 0; i < v.size; ++i)
            index += v.Get(i) * (int)Mathf.Pow(numVertexTypes, v.size - i - 1);
        return spriteNamesFromVertices[index];
    }

    private Vertices GetVertexFromSpriteName(string name)
    {
        string spriteIndex = name.Substring(name.Length - 2);
        int index = int.Parse(spriteIndex);
        return verticesFromSpriteNames[index];
    }
}
