using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GraphTest : MonoBehaviour
{
    public enum Algorithm
    {
        DFS,
        BFS,
        DFSRecursive,
        PathFindingBFS,
        Dijkstra,
        Astar,
    }

    public UiGraphNode nodePrefab;
    public List<UiGraphNode> uiNodes;

    public Transform uiNodeRoot;

    private Graph graph;

    private void Start()
    {
        int[,] map = new int[5, 5]
        {
            {1,-1,1,1,2},
            {1,-1,10,-1,1},
            {1,-1,1,2,1},
            {1,-1,10,-1,1},
            {1,1,1,15,1},
        };

        graph = new Graph();
        graph.Init(map);
        InitUiNodes(graph);
    }

    private void InitUiNodes(Graph graph)
    {
        foreach (var node in graph.nodes)
        {
            var uiNode = Instantiate(nodePrefab, uiNodeRoot);
            uiNode.SetNode(node);
            uiNode.Reset();
            uiNodes.Add(uiNode);
        }
    }

    private void ResetUiNodes()
    {
        foreach (var node in uiNodes)
        {
            node.Reset();
        }
    }

    public Algorithm algorithm;

    public int startIndex;
    public int endIndex;

    [ContextMenu("Search")]
    public void Search()
    {
        var search = new GraphSearch();
        search.Init(graph);

        switch(algorithm)
        {
            case Algorithm.DFS:
                search.DFS(graph.nodes[startIndex]);
                break;
            case Algorithm.BFS:
                search.BFS(graph.nodes[startIndex]);
                break;
            case Algorithm.DFSRecursive:
                search.DFSRecursive(graph.nodes[startIndex]);
                break;
            case Algorithm.PathFindingBFS:
                search.PathFindingBFS(graph.nodes[startIndex], graph.nodes[endIndex]);
                break;
            case Algorithm.Dijkstra:
                search.Dijkstra(graph.nodes[startIndex], graph.nodes[endIndex]);
                break;
            case Algorithm.Astar:
                search.Astar(graph.nodes[startIndex], graph.nodes[endIndex]);
                break;
        }
        ResetUiNodes();

        for (int i = 0; i < search.path.Count; i++)
        {
            var node = search.path[i];
            var color = Color.Lerp(Color.red, Color.green, (float)i / (search.path.Count - 1));
            uiNodes[node.id].SetColor(color);
            uiNodes[node.id].SetText($"ID: {node.id} \nweight : {node.weight}\n Path : {i}");
        }
    }
}
