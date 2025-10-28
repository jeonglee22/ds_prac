using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyBinarySearchTree<Tkey, TValue> : IDictionary<Tkey, TValue> where Tkey : IComparable<Tkey>
{
    private TreeNode<Tkey, TValue> root;

    public TValue this[Tkey key]
    {
        get
        {
            throw new NotImplementedException();
        }
        set
        {
            throw new NotImplementedException();
        }
    }

    public ICollection<Tkey> Keys => throw new NotImplementedException();

    public ICollection<TValue> Values => throw new NotImplementedException();

    public int Count => throw new NotImplementedException();

    public bool IsReadOnly => false;

    public MyBinarySearchTree()
    {
        root = null;
    }

    public void Add(Tkey key, TValue value)
    {
        root = Add(root, key, value);
    }

    public void Add(KeyValuePair<Tkey, TValue> item)
    {
        throw new NotImplementedException();
    }

    public virtual TreeNode<Tkey, TValue> Add(TreeNode<Tkey, TValue> node, Tkey key, TValue value)
    {
        if (node == null)
        {
            node = new TreeNode<Tkey, TValue>(key, value);
            return node;
        }

        int compare = key.CompareTo(node.Key);
        if (compare < 0)
        {
        }
        else if (compare > 0)
        {

        }
        else
        {
            throw new ArgumentException($"key already exists");
        }

        return node;
    }

    public void Clear()
    {
        throw new NotImplementedException();
    }

    public bool Contains(KeyValuePair<Tkey, TValue> item)
    {
        throw new NotImplementedException();
    }

    public bool ContainsKey(Tkey key)
    {
        throw new NotImplementedException();
    }

    public void CopyTo(KeyValuePair<Tkey, TValue>[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }

    public IEnumerator<KeyValuePair<Tkey, TValue>> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    public bool Remove(Tkey key)
    {
        throw new NotImplementedException();
    }

    public bool Remove(KeyValuePair<Tkey, TValue> item)
    {
        throw new NotImplementedException();
    }

    public bool TryGetValue(Tkey key, out TValue value)
    {
        return TryGetValue(root, key, out value);
    }

    protected bool TryGetValue(TreeNode<Tkey, TValue> node, Tkey key, out TValue value)
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
}
