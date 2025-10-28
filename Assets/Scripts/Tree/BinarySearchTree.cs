using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class BinarySearchTree<TKey, TValue> : IDictionary<TKey, TValue> where TKey : IComparable<TKey>
{
    protected TreeNode<TKey, TValue> root;

    public TValue this[TKey key]
    {
        get
        {
            if (TryGetValue(key, out var value))
            {
                return value;
            }
            else
            {
                throw new KeyNotFoundException($"key not exits");
            }
        }
        set
        {
            root = AddorUpdate(root, key, value);
        }
    }
    
    protected virtual TreeNode<TKey, TValue> AddorUpdate(TreeNode<TKey, TValue> node, TKey key, TValue value)
    {
        if (node == null)
        {
            return new TreeNode<TKey, TValue>(key, value);
        }

        int compare = key.CompareTo(node.Key);
        if (compare < 0)
        {
            node.Left = AddorUpdate(node.Left, key, value);
        }
        else if (compare > 0)
        {
            node.Right = AddorUpdate(node.Right, key, value);
        }
        else
        {
            node.Value = value;
        }

        UpdateHeight(node);
        return node;
    }

    public ICollection<TKey> Keys => InOrderTraversal().Select(kvp => kvp.Key).ToList();

    public ICollection<TValue> Values => InOrderTraversal().Select(kvp => kvp.Value).ToList();

    public int Count => CountNodes(root);
    
    protected virtual int CountNodes(TreeNode<TKey,TValue> node)
    {
        if (node == null)
            return 0;

        return 1 + CountNodes(node.Left) + CountNodes(node.Right);
    }

    public bool IsReadOnly => false;

    public BinarySearchTree()
    {
        root =  null;
    }

    public void Add(TKey key, TValue value)
    {
        root = Add(root, key, value);
    }

    protected virtual TreeNode<TKey, TValue> Add(TreeNode<TKey, TValue> node, TKey key, TValue value)
    {
        if (node == null)
        {
            return new TreeNode<TKey, TValue>(key, value);
        }

        int compare = key.CompareTo(node.Key);
        if (compare < 0)
        {
            node.Left = Add(node.Left, key, value);
        }
        else if (compare > 0)
        {
            node.Right = Add(node.Right, key, value);
        }
        else
        {
            throw new ArgumentException($"key : {key} already exist");
        }

        UpdateHeight(node);
        return node;
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }

    public void Clear()
    {
        root = null;
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return ContainsKey(item.Key);
    }

    public bool ContainsKey(TKey key)
    {
        return TryGetValue(key, out var _);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        // excep

        foreach (var kvp in this)
        {
            array[arrayIndex++] = new KeyValuePair<TKey, TValue>(kvp.Key, kvp.Value);
        }
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return InOrderTraversal().GetEnumerator();
    }

    public bool Remove(TKey key)
    {
        int beforeCount = Count;
        root = Remove(root, key);

        return beforeCount > Count;
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        return Remove(item.Key);
    }

    protected virtual TreeNode<TKey,TValue> Remove(TreeNode<TKey,TValue> node, TKey key)
    {
        if (node == null)
        {
            return node;
        }

        int compare = key.CompareTo(node.Key);
        if (compare < 0)
        {
            node.Left = Remove(node.Left, key);
        }
        else if (compare > 0)
        {
            node.Right = Remove(node.Right, key);
        }
        else
        {
            if (node.Left == null)
            {
                return node.Right;
            }
            else if (node.Right == null)
            {
                return node.Left;
            }

            var minNode = FindMin(node.Right);
            node.Key = minNode.Key;
            node.Value = minNode.Value;

            node.Right = Remove(node.Right, minNode.Key);
        }

        UpdateHeight(node);
        return node;
    }

    protected virtual TreeNode<TKey, TValue> FindMin(TreeNode<TKey,TValue> node)
    {
        while(node.Left != null)
        {
            node = node.Left;
        }

        return node;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        return TryGetValue(root, key, out value);
    }

    protected bool TryGetValue(TreeNode<TKey, TValue> node, TKey key, out TValue value)
    {
        if (node == null)
        {
            value = default(TValue);
            return false;
        }

        int compare = key.CompareTo(node.Key);
        if (compare < 0)
        {
            return TryGetValue(node.Left, key, out value);
        }
        else if (compare > 0)
        {
            return TryGetValue(node.Right, key, out value);
        }
        else
        {
            value = node.Value;
            return true;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public virtual IEnumerable<KeyValuePair<TKey, TValue>> InOrderTraversal()
    {
        return InOrderTraversal(root);
    }
    protected virtual IEnumerable<KeyValuePair<TKey, TValue>> InOrderTraversal(TreeNode<TKey, TValue> node)
    {
        if(node != null)
        {
            foreach (var kvp in InOrderTraversal(node.Left))
            {
                yield return kvp;
            }

            yield return new KeyValuePair<TKey, TValue>(node.Key, node.Value);

            foreach (var kvp in InOrderTraversal(node.Right))
            {
                yield return kvp;
            }
        }
    } 
    
    public virtual IEnumerable<KeyValuePair<TKey, TValue>> PreOrderTraversal()
    {
        return PreOrderTraversal(root);
    }
    protected virtual IEnumerable<KeyValuePair<TKey, TValue>> PreOrderTraversal(TreeNode<TKey, TValue> node)
    {
        if(node != null)
        {
            yield return new KeyValuePair<TKey, TValue>(node.Key, node.Value);
            
            foreach (var kvp in PreOrderTraversal(node.Left))
            {
                yield return kvp;
            }

            foreach (var kvp in PreOrderTraversal(node.Right))
            {
                yield return kvp;
            }
        }
    } 
    
    public virtual IEnumerable<KeyValuePair<TKey, TValue>> PostOrderTraversal()
    {
        return PostOrderTraversal(root);
    }
    protected virtual IEnumerable<KeyValuePair<TKey, TValue>> PostOrderTraversal(TreeNode<TKey, TValue> node)
    {
        if(node != null)
        {
            foreach (var kvp in PostOrderTraversal(node.Left))
            {
                yield return kvp;
            }

            foreach (var kvp in PostOrderTraversal(node.Right))
            {
                yield return kvp;
            }

            yield return new KeyValuePair<TKey, TValue>(node.Key, node.Value);
        }
    } 
    
    public virtual IEnumerable<KeyValuePair<TKey, TValue>> LevelOrderTraversal()
    {
        return LevelOrderTraversal(root);
    }
    protected virtual IEnumerable<KeyValuePair<TKey, TValue>> LevelOrderTraversal(TreeNode<TKey, TValue> node)
    {
        var nodeQueue = new Queue<TreeNode<TKey, TValue>>();
        nodeQueue.Enqueue(node);

        while (nodeQueue.Count != 0)
        {
            var first = nodeQueue.Dequeue();

            if (first == null)
                continue;

            yield return new KeyValuePair<TKey, TValue>(first.Key, first.Value);

            nodeQueue.Enqueue(first.Left);
            nodeQueue.Enqueue(first.Right);
        }
    } 

    protected virtual void UpdateHeight(TreeNode<TKey, TValue> node)
    {
        node.Height = Mathf.Max(Height(node.Left), Height(node.Right)) + 1;
    }
    
    protected virtual int Height(TreeNode<TKey, TValue> node)
    {
        return node == null ? 0 : node.Height;
    }
}
