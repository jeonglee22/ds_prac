using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

public enum TileTypes
{
    Empty = -1,
    // 0, 14
    Grass = 15,
    Tree = 16,
    Hills = 17,
    Mountains = 18,
    Towns = 19,
    Castle = 20,
    Monster = 21
}

public class Map
{ 
    public int rows = 0;
    public int cols = 0;

    public Tile[] tiles;

    public Tile castleTile;
    public Tile startTile;

    public Tile[] CoastTiles
    {
        
        get
        {
            return tiles.Where(t => t.autoTileId < (int)TileTypes.Grass).ToArray();
        }
    } 

    public Tile[] LandTiles
    {
        get
        {
            return tiles.Where(t => t.autoTileId >= (int)TileTypes.Grass).ToArray();
        }
    }

    public void Init(int rows, int cols)   // 0: O 1: X
    {
        this.rows = rows;
        this.cols = cols;

        tiles = new Tile[rows * cols];
        for (int i = 0; i  < tiles.Length; i++)
        {
            tiles[i] = new Tile();
            tiles[i].id = i;
        }

        for (var r = 0; r < rows; ++r)
        {
            for (var c = 0; c < cols; ++c)
            {
                var index = r * cols + c;

                var indexU = (r - 1) * cols + c;
                var indexR = r * cols + c + 1;
                var indexD = (r + 1) * cols + c;
                var indexL = r * cols + c - 1;

                var indexUL = (r - 1) * cols + c - 1;
                var indexUR = (r - 1) * cols + c + 1;
                var indexDR = (r + 1) * cols + c + 1;
                var indexDL = (r + 1) * cols + c - 1;

                if ((r - 1) >= 0)
                {
                    tiles[index].adjacents[(int)Sides.Top] = tiles[indexU];
                    if ((c - 1) >= 0)
                    {
                        tiles[index].adjacents[(int)Sides.TopLeft] = tiles[indexUL];
                    }
                    if(c + 1 < cols)
                    {
                        tiles[index].adjacents[(int)Sides.TopRight] = tiles[indexUR];
                    }
                }
                if (c + 1 < cols)
                {
                    tiles[index].adjacents[(int)Sides.Right] = tiles[indexR];
                    if(r - 1 >= 0)
                    {
                        tiles[index].adjacents[(int)Sides.TopRight] = tiles[indexUR];
                    }
                    if(r + 1 < rows)
                    {
                        tiles[index].adjacents[(int)Sides.BottomRight] = tiles[indexDR];
                    }
                }
                if (r + 1 < rows)
                {
                    tiles[index].adjacents[(int)Sides.Bottom] = tiles[indexD];
                    if(c - 1 >= 0)
                    {
                        tiles[index].adjacents[(int)Sides.BottomLeft] = tiles[indexDL];
                    }
                    if(c + 1 < cols)
                    {
                        tiles[index].adjacents[(int)Sides.BottomRight] = tiles[indexDR];
                    }
                }
                if (c - 1 >= 0)
                {
                    tiles[index].adjacents[(int)Sides.Left] = tiles[indexL];
                    if(r - 1 >= 0)
                    {
                        tiles[index].adjacents[(int)Sides.TopLeft] = tiles[indexUL];
                    }
                    if(r + 1 < rows)
                    {
                        tiles[index].adjacents[(int)Sides.BottomLeft] = tiles[index];
                    }
                }
            }
        }

        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i].UpdateAuotoTileId();
            tiles[i].UpdateAuotoFowId();
        }
    }

    public bool CreateIsland(
        float erodePercent,
        int erodeIterations,
        float lakePercent,
        float treePercent,
        float hillPercent,
        float mountainPercent,
        float townPercent,
        float monsterPercent)
    {
        DecorateTiles(LandTiles, lakePercent, TileTypes.Empty);

        for (int i = 0; i < erodeIterations; ++i)
            DecorateTiles(CoastTiles, erodePercent, TileTypes.Empty);

        DecorateTiles(LandTiles, treePercent, TileTypes.Tree);
        DecorateTiles(LandTiles, hillPercent, TileTypes.Hills);
        DecorateTiles(LandTiles, mountainPercent, TileTypes.Mountains);
        DecorateTiles(LandTiles, townPercent, TileTypes.Towns);
        DecorateTiles(LandTiles, monsterPercent, TileTypes.Monster);

        var towns = tiles.Where(x => x.autoTileId == (int)TileTypes.Towns).ToArray();
        ShuffleTiles(towns);
        startTile = towns[0];

        var catsleTargets = tiles.Where(x => x.autoTileId == (int)TileTypes.Towns && x.id != startTile.id).ToArray();
        if (!GetCastleTile(catsleTargets))
            return false;

        return true;
    }
    
    private bool GetCastleTile(Tile[] targets)
    {
        List<Tile> CanGo = new List<Tile>();

        foreach (var target in targets)
        {
            if (FindRouteAStar(startTile, target))
            {
                CanGo.Add(target);
            }
        }

        if (CanGo.Count == 0)
            return false;

        castleTile = CanGo[Random.Range(0, CanGo.Count)];
        castleTile.autoTileId = (int)TileTypes.Castle;
        return true;
    }

    public void DecorateTiles(Tile[] tiles, float percent, TileTypes tileType)
    {
        int total = Mathf.FloorToInt(tiles.Length * percent);

        ShuffleTiles(tiles);

        for (int i = 0; i < total; ++i)
        {
            if (tileType == TileTypes.Empty)
                tiles[i].ClearAdjacents();

            tiles[i].autoTileId = (int)tileType;
        }
    }

    public void ShuffleTiles(Tile[] tiles)
    {
        // Fisher-Yates 셔플 알고리즘 구현
        for (int i = tiles.Length - 1; i > 0; i--)
        {
            // 0과 i 사이의 무작위 인덱스 선택
            int randomIndex = Random.Range(0, i + 1);

            // i번째 요소와 무작위로 선택된 요소 교환
            Tile temp = tiles[i];
            tiles[i] = tiles[randomIndex];
            tiles[randomIndex] = temp;
        }
    }

    public List<Tile> path = new List<Tile>();

    private void ResetTilesPrevious()
    {
        foreach (var tile in tiles)
        {
            tile.previous = null;
        }
    }

    private int Heuristic(Tile a, Tile b)
    {
        int ax = a.id % cols;
        int ay = a.id / cols;
        int bx = b.id % cols;
        int by = b.id / cols;

        return Mathf.Abs(ax - bx) + Mathf.Abs(ay - by);
    }

    public bool FindRouteAStar(Tile start, Tile end)
    {
        path.Clear();
        ResetTilesPrevious();

        var visited = new HashSet<Tile>();
        var pQueue = new PriorityQueue<Tile, int>();

        var distances = new int[tiles.Length];
        var scores = new int[tiles.Length];
        for (int i = 0; i < distances.Length; i++)
        {
            distances[i] = int.MaxValue;
            scores[i] = int.MaxValue;
        }
        bool success = false;

        distances[start.id] = start.Weight;
        scores[start.id] = distances[start.id] + Heuristic(start, end);
        pQueue.Enqueue(start, scores[start.id]);

        while (pQueue.Count > 0)
        {
            var currentNode = pQueue.Dequeue();
            if (visited.Contains(currentNode))
                continue;

            if (currentNode == end)
            {
                success = true;
                break;
            }

            visited.Add(currentNode);

            foreach (var adjacent in currentNode.adjacents)
            {
                if (adjacent == null || (adjacent != null && (!adjacent.CanMove || visited.Contains(adjacent))))
                    continue;

                var newDis = distances[currentNode.id] + adjacent.Weight;
                if (distances[adjacent.id] > newDis)
                {
                    distances[adjacent.id] = newDis;
                    scores[adjacent.id] = distances[adjacent.id] + Heuristic(adjacent, end);
                    adjacent.previous = currentNode;

                    pQueue.Enqueue(adjacent, scores[adjacent.id]);
                }
            }
        }

        if (!success)
            return false;

        Tile step = end;
        while (step != null)
        {
            path.Add(step);
            step = step.previous;
        }
        path.Reverse();
        return true;
    }
    
    private float HeuristicDiagonal(Tile a, Tile b)
    {
        int ax = a.id % cols;
        int ay = a.id / cols;
        int bx = b.id % cols;
        int by = b.id / cols;
        float dx = Mathf.Abs(ax - bx);
        float dy = Mathf.Abs(ay - by);

        return dx + dy + (Mathf.Sqrt(2) - 2) * Mathf.Min(dx, dy);
    }

    public bool FindRouteAStarDiagonal(Tile start, Tile end)
    {
        path.Clear();
        ResetTilesPrevious();

        var visited = new HashSet<Tile>();
        var pQueue = new PriorityQueue<Tile, float>();

        var distances = new float[tiles.Length];
        var scores = new float[tiles.Length];
        for (int i = 0; i < distances.Length; i++)
        {
            distances[i] = int.MaxValue;
            scores[i] = int.MaxValue;
        }
        bool success = false;

        distances[start.id] = start.Weight;
        scores[start.id] = distances[start.id] + HeuristicDiagonal(start, end);
        pQueue.Enqueue(start, scores[start.id]);

        while (pQueue.Count > 0)
        {
            var currentNode = pQueue.Dequeue();
            if (visited.Contains(currentNode))
                continue;

            if (currentNode == end)
            {
                success = true;
                break;
            }

            visited.Add(currentNode);

            for (int i = 0; i < currentNode.adjacents.Length;  i++)
            {
                var adjacent = currentNode.adjacents[i];
                if (adjacent == null || (adjacent != null && (!adjacent.CanMove || visited.Contains(adjacent))))
                    continue;

                var newDis = distances[currentNode.id] + adjacent.Weight;
                if (distances[adjacent.id] > newDis)
                {
                    distances[adjacent.id] = newDis;
                    scores[adjacent.id] = distances[adjacent.id] * (i <= 3 ? 1.4f : 1f) + HeuristicDiagonal(adjacent, end);
                    adjacent.previous = currentNode;

                    pQueue.Enqueue(adjacent, scores[adjacent.id]);
                }
            }
        }

        if (!success)
            return false;

        Tile step = end;
        while (step != null)
        {
            path.Add(step);
            step = step.previous;
        }
        path.Reverse();
        return true;
    }
}
