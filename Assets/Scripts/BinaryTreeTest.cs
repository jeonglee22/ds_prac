using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class BinaryTreeTest : MonoBehaviour
{
    public BinaryTreeVisualizer treeVisualizer;

    [SerializeField] private int nodeCount = 10;
    [SerializeField] private int minKey = 1;
    [SerializeField] private int maxKey = 1000;

    private VisualizableBST<int, string> tree;

    public int keyInput;

    private void Start()    
    {
        GenerateRandomTree();
    }

    public void GenerateRandomTree()
    {
        tree = new VisualizableBST<int, string>();

        int addedNodes = 0;
        while (addedNodes < nodeCount)
        {
            int key = Random.Range(minKey, maxKey + 1);

            if (!tree.ContainsKey(key))
            {
                string value = $"V-{key}";
                tree.Add(key, value);
                addedNodes++;
            }
        }

        treeVisualizer.VisualizeTree(tree);
    }

    [ContextMenu("Add Random Node In Tree")]
    public void AddRandomNodeInTree()
    {
        while (true)
        {
            int key = Random.Range(minKey, maxKey + 1);

            if (!tree.ContainsKey(key))
            {
                string value = $"V-{key}";
                tree.Add(key, value);

                Debug.Log($"key : {key}, value : {value}");
                break;
            }
        }

        treeVisualizer.VisualizeTree(tree);
    }

    
    public void RemoveNodeInTree()
    {
        if (tree.ContainsKey(keyInput))
        {
            tree.Remove(keyInput);
        }

        treeVisualizer.VisualizeTree(tree);
    }

    [ContextMenu("Generate New Random Tree")]
    public void RegenerateTree()
    {
        GenerateRandomTree();
    }

    [ContextMenu("InOrder Checking")]
    public void ShowInOrder()
    {
        Debug.Log("[InOrder] Start Checking");
        foreach (var node in tree.InOrderTraversal())
        {
            Debug.Log($"key : {node.Key}, value : {node.Value}");
        }
        Debug.Log("[InOrder] Finish Checking");
    }

    [ContextMenu("PreOrder Checking")]
    public void ShowPreOrder()
    {
        Debug.Log("[PreOrder] Start Checking");
        foreach (var node in tree.PreOrderTraversal())
        {
            Debug.Log($"key : {node.Key}, value : {node.Value}");
        }
        Debug.Log("[PreOrder] Finish Checking");
    }

    [ContextMenu("PostOrder Checking")]
    public void ShowPostOrder()
    {
        Debug.Log("[PostOrder] Start Checking");
        foreach (var node in tree.PostOrderTraversal())
        {
            Debug.Log($"key : {node.Key}, value : {node.Value}");
        }
        Debug.Log("[PostOrder] Finish Checking");
    }

    [ContextMenu("LevelOrder Checking")]
    public void ShowLevelOrder()
    {
        Debug.Log("[LevelOrder] Start Checking");
        foreach (var node in tree.LevelOrderTraversal())
        {
            Debug.Log($"key : {node.Key}, value : {node.Value}");
        }
        Debug.Log("[LevelOrder] Finish Checking");
    }
}