using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GraphSearch
{
    private Graph graph;
    public List<GraphNode> path = new List<GraphNode>();

    public void Init(Graph graph)
    {
        this.graph = graph;
    }

    public void DFS(GraphNode node)
    {
        path.Clear();

        var visited = new HashSet<GraphNode>();
        var stack = new Stack<GraphNode>();

        stack.Push(node);
        while(stack.Count > 0)
        {
            var currentNode = stack.Pop();
            path.Add(currentNode);
            visited.Add(currentNode);

            foreach (var adjacent in currentNode.adjacents)
            {
                if (!adjacent.CanVisit || visited.Contains(adjacent) || stack.Contains(adjacent))
                    continue;

                stack.Push(adjacent);
            }
        }
    }

    public void BFS(GraphNode node)
    {
        path.Clear();

        var visited = new HashSet<GraphNode>();
        var queue = new Queue<GraphNode>();

        queue.Enqueue(node);
        while (queue.Count > 0)
        {
            var currentNode = queue.Dequeue();
            path.Add(currentNode);
            visited.Add(currentNode);

            foreach (var adjacent in currentNode.adjacents)
            {
                if (!adjacent.CanVisit || visited.Contains(adjacent) || queue.Contains(adjacent))
                    continue;

                queue.Enqueue(adjacent);
            }
        }
    }

    public void DFSRecursive(GraphNode node)
    {
        path.Clear();
        DFSRecursive(node, new HashSet<GraphNode>());
    }

    public void DFSRecursive(GraphNode node, HashSet<GraphNode> RecVisited)
    {
        path.Add(node);
        RecVisited.Add(node);

        foreach (var adjacent in node.adjacents)
        {
            if (!adjacent.CanVisit || RecVisited.Contains(adjacent) || path.Contains(adjacent))
                continue;

            DFSRecursive(adjacent, RecVisited);
        }
    }

    public void PathFindingBFS(GraphNode start, GraphNode end)
    {
        if (!end.CanVisit)
            return;

        var visited = new HashSet<GraphNode>();
        var queue = new Queue<GraphNode>();

        queue.Enqueue(start);
        while (queue.Count > 0 && queue.Peek().id != end.id)
        {
            var currentNode = queue.Dequeue();
            visited.Add(currentNode);

            foreach (var adjacent in currentNode.adjacents)
            {
                if (!adjacent.CanVisit || visited.Contains(adjacent) || queue.Contains(adjacent))
                    continue;

                queue.Enqueue(adjacent);
                adjacent.previous = currentNode;
            }
        }

        path.Clear();
        if (queue.Count == 0 || queue.Peek().id != end.id)
        {
            Debug.Log("No Path from start to end");
        }
        else if (queue.Peek().id == end.id)
        {
            path.Add(end);

            var pathNode = queue.Peek();
            while (pathNode.previous != null)
            {
                path.Add(pathNode.previous);
                pathNode = pathNode.previous;
            }

            path.Reverse();
        }
    }

    public bool Dijkstra(GraphNode start, GraphNode end)
    {
        path.Clear();
        graph.ResetNodePrevious();

        var visited = new HashSet<GraphNode>();
        var pQueue = new PriorityQueue<GraphNode, int>();

        var distances = new int[graph.nodes.Length];
        for (int i = 0; i < distances.Length; i++)
        {
            distances[i] = int.MaxValue;
        }

        distances[start.id] = start.weight;
        pQueue.Enqueue(start, distances[start.id]);

        bool success = false;
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

            foreach (var node in currentNode.adjacents)
            {
                if (!node.CanVisit || visited.Contains(node))
                    continue;

                var newDis = distances[currentNode.id] + node.weight;
                if (distances[node.id] > newDis)
                {
                    distances[node.id] = newDis;
                    pQueue.Enqueue(node, newDis);
                    node.previous = currentNode;
                }
            }
        }

        if (!success)
            return false;

        GraphNode step = end;
        while (step != null)
        {
            path.Add(step);
            step = step.previous;
        }
        path.Reverse();
        return true;
    }

    protected int Heuristic(GraphNode a, GraphNode b)
    {
        int ax = a.id % graph.cols;
        int ay = a.id / graph.cols;
        int bx = b.id % graph.cols;
        int by = b.id / graph.cols;

        return Mathf.Abs(ax - bx) + Mathf.Abs(ay - by);
    }
    
    public bool Astar(GraphNode start, GraphNode end)
    {
        path.Clear();
        graph.ResetNodePrevious();

        var visited = new HashSet<GraphNode>();
        var pQueue = new PriorityQueue<GraphNode, int>();

        var distances = new int[graph.nodes.Length];
        var scores = new int[graph.nodes.Length];
        for (int i = 0; i < distances.Length; i++)
        {
            distances[i] = int.MaxValue;
            scores[i] = int.MaxValue;
        }
        bool success = false;

        distances[start.id] = start.weight;
        scores[start.id] = distances[start.id] + Heuristic(start, end);
        pQueue.Enqueue(start, scores[start.id]);

        while(pQueue.Count > 0)
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
                if (!adjacent.CanVisit || visited.Contains(adjacent))
                    continue;

                var newDis = distances[currentNode.id] + adjacent.weight;
                if(distances[adjacent.id] > newDis)
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

        GraphNode step = end;
        while (step != null)
        {
            path.Add(step);
            step = step.previous;
        }
        path.Reverse();
        return true;
    }
}
