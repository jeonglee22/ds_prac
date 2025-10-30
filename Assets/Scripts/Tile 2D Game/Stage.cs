using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Stage : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject tilePrefab;
    private GameObject[] tileObjs;
    private GameObject player;

    public int mapWidth = 20;
    public int mapHeight = 20;

    [Range(0f, 0.9f)]
    public float erodePercent = 0.5f;
    public int erodeIteration = 2;
    [Range(0f, 0.9f)]
    public float lakePercent = 0.1f;

    [Range(0f, 0.9f)]
    public float treePercent = 0.1f;
    [Range(0f, 0.9f)]
    public float hillPercent = 0.1f;
    [Range(0f, 0.9f)]
    public float moutainPercent = 0.1f;
    [Range(0f, 0.9f)]
    public float townPercent = 0.1f;
    [Range(0f, 0.9f)]
    public float monsterPercent = 0.1f;

    public Vector2 tileSize = new Vector2(16, 16);

    //public Texture2D islandTexture;
    public Sprite[] islandSprites;
    public Sprite[] fowSprites;

    private Map map;

    public Map Map
    {
        get { return map; }
    }

    private Vector3 firstTilePos;

    public int ScreenPosToTileId(Vector3 screenPos)
    {
        screenPos.z = Mathf.Abs(transform.position.z - cam.transform.position.z);
        var worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        return WorldPosToTileId(worldPos);
    }

    public int WorldPosToTileId(Vector3 worldPos)
    {
        var pivot = firstTilePos;
        pivot.x -= tileSize.x * 0.5f;
        pivot.y += tileSize.y * 0.5f;

        var diff = worldPos - pivot;
        int x = Mathf.FloorToInt(diff.x / tileSize.x);
        int y = -Mathf.CeilToInt(diff.y / tileSize.y);

        x = Mathf.Clamp(x, 0, mapWidth - 1);
        y = Mathf.Clamp(y, 0, mapHeight - 1);

        return y * mapWidth + x;
    }

    public Vector3 GetTilePos(int y, int x)
    {
        var pos = firstTilePos;
        pos.x += tileSize.x * x;
        pos.y -= tileSize.y * y;
        return pos;
    }

    public Vector3 GetTilePos(int tileId)
    {
        return GetTilePos(tileId / mapWidth, tileId % mapWidth);
    }

    private void ResetStage()
    {
        bool succeed = false;
        while (!succeed)
        {
            map = new Map();
            map.Init(mapHeight, mapWidth);
            succeed = map.CreateIsland(erodePercent, erodeIteration, lakePercent,
                treePercent, hillPercent, moutainPercent, townPercent, monsterPercent);
        }
        CreateGrid();
        CreatePlayer();
    }

    private void CreatePlayer()
    {
        if (player != null)
        {
            Destroy(player);
        }
        player = Instantiate(playerPrefab, GetTilePos(map.startTile.id), Quaternion.identity);
    }

    private void CreateGrid()
    {
        if (tileObjs != null)
        {
            foreach (var tile in tileObjs)
            {
                Destroy(tile.gameObject);
            }
        }
        tileObjs = new GameObject[mapHeight * mapWidth];

        firstTilePos = Vector3.zero;
        firstTilePos.x -= mapWidth * tileSize.x * 0.5f;
        firstTilePos.y += mapHeight * tileSize.y * 0.5f;
        var pos = firstTilePos;
        for (int i = 0; i < mapHeight; ++i)
        {
            for (int j = 0; j < mapWidth; ++j)
            {
                var tileId = i * mapWidth + j;
                var tile = map.tiles[tileId];

                var newGo = Instantiate(tilePrefab, transform);
                newGo.transform.localPosition = pos;
                pos.x += tileSize.x;
                newGo.name = $"Tile ({i} , {j})";
                tileObjs[tileId] = newGo;
                DecorateTile(tileId);
            }
            pos.x = firstTilePos.x;
            pos.y -= tileSize.y;
        }
    }

    public void DecorateTile(int tileId)
    {
        var tile = map.tiles[tileId];
        var tileGo = tileObjs[tileId];
        var ren = tileGo.GetComponent<SpriteRenderer>();
        if (tile.autoTileId != (int)TileTypes.Empty)
        {
            ren.sprite = islandSprites[tile.autoTileId];
        }
        else
        {
            ren.sprite = null;
        }

        if (tile.isVisited)
        {
            if (tile.autoTileId != (int)TileTypes.Empty)
            {
                ren.sprite = islandSprites[tile.autoTileId];
            }
            else
            {
                ren.sprite = null;
            }
        }
        else
        {
            ren.sprite = fowSprites[tile.autoFowId];
        }
    }

    public int visiteRadius = 1;

    public void OnTileVisited(Tile tile)
    {
        int centerX = tile.id % mapWidth;
        int centerY = tile.id / mapWidth;

        int radius = visiteRadius;
        for (int i = -radius; i <= radius; ++i)
        {
            for (int j = -radius; j <= radius; ++j)
            {
                int x = centerX + j;
                int y = centerY + i;
                if (x < 0 || x >= mapWidth || y < 0 || y >= mapHeight)
                    continue;

                int id = y * mapWidth + x;
                map.tiles[id].isVisited = true;
                DecorateTile(id);
            }
        }
        radius += 1;
        for (int i = -radius; i <= radius; ++i)
        {
            for (int j = -radius; j <= radius; ++j)
            {
 
                if (i == radius || i == -radius || j == radius || j == -radius)
                {
                    int x = centerX + j;
                    int y = centerY + i;
                    if (x < 0 || x >= mapWidth || y < 0 || y >= mapHeight)
                        continue;

                    int id = y * mapWidth + x;
                    map.tiles[id].UpdateAuotoFowId();
                    DecorateTile(id);
                }
            }
        }
    }

    private Camera cam;
    private GameObject castle;

    private bool isMove = false;

    private float movingTime;
    public float movingInterval = 0.5f;
    private int pos = 0;
    private List<Tile> currentPath;

    private Vector3 playerBeforePos;

    private void Awake()
    {
        cam = Camera.main;
    }
    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log(ScreenPosToTileId(Input.mousePosition));
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetStage();
            OnTileVisited(map.startTile);
            isMove = false;
            pos = 0;
            movingTime = 0f;
        }

        if (isMove)
        {
            movingTime += Time.deltaTime;
            movingTime = Mathf.Clamp01(movingTime);

            var curr = currentPath[pos];

            Tile next;
            if (currentPath.Count == 1)
            {
                next = currentPath[pos];
            }
            else
            {
                next = currentPath[pos+1];
            }            

            var currPos = GetTilePos(curr.id);
            var nextPos = GetTilePos(next.id);

            if(playerBeforePos.z != -1 && pos == 0)
            {
                currPos = playerBeforePos;
            }

            player.transform.position = Vector3.Lerp(currPos, nextPos, movingTime / movingInterval);

            if (movingTime >= movingInterval)
            {
                pos += 1;
                movingTime = 0f;
                OnTileVisited(curr);
                if (pos == currentPath.Count - 1)
                {
                    isMove = false;
                    pos = 0;
                    playerBeforePos.z = -1;
                    currentPath = null;
                }
            }
        }

        if (map != null && Input.GetMouseButtonDown(0))
        {
            var playerPosId = WorldPosToTileId(player.transform.position);
            var mouseId = ScreenPosToTileId(Input.mousePosition);

            if (map.FindRouteAStar(map.tiles[playerPosId], map.tiles[mouseId]))
            {
                if (isMove)
                {
                    playerBeforePos = player.transform.position;
                    pos = 0;
                    movingTime = 0f;
                }

                currentPath = new List<Tile>(map.path);
                isMove = true;
            }
        }

        if(player != null)
        {
            var pos = player.transform.position;
            pos.z -= 10;
            cam.transform.position = pos;
        }
    }
}
