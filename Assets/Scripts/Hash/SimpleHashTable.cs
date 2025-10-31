using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using Unity.XR.Oculus.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class SimpleHashTable<TKey, TValue> : IDictionary<TKey, TValue>
{
    private const int DefaultCapacity = 16;
    private const double loadFactor = 0.75;

    private KeyValuePair<TKey, TValue>[] table;
    private bool[] occupied;

    private int size;
    private int count;

    public SimpleHashTable()
    {
        table = new KeyValuePair<TKey, TValue>[DefaultCapacity];
        occupied = new bool[DefaultCapacity];
        size = DefaultCapacity;
        count = 0;
    }

    private int GetIndex(TKey key)
    {
        return GetIndex(key, size);
    }

    private int GetIndex(TKey key, int size)
    {
        if (key == null)
            throw new ArgumentException(nameof(key));

        int hash = key.GetHashCode();
        return Math.Abs(hash) % size;
    }

    public TValue this[TKey key]
    {
        get
        {
            if (TryGetValue(key, out TValue value))
            {
                return value;
            }

            throw new KeyNotFoundException("No key");
        }
        set
        {
            if (key == null)
                throw new ArgumentNullException("Invalid key");

            int index = GetIndex(key);
            if (occupied[index] && table[index].Key.Equals(key))
            {
                table[index] = new KeyValuePair<TKey, TValue>(key, value);
            }
            else if (!occupied[index])
            {
                table[index] = new KeyValuePair<TKey, TValue>(key, value);
                occupied[index] = true;
                count++;
            }
            else
            {
                throw new InvalidOperationException("Hash Collision");
            }
        }
    }

    public ICollection<TKey> Keys => table.Where(x => x.Key != null && occupied[GetIndex(x.Key)]).Select(x => x.Key).ToArray();

    public ICollection<TValue> Values => table.Where(x => x.Key != null && occupied[GetIndex(x.Key)]).Select(x => x.Value).ToArray();

    public int Count => count;

    public bool IsReadOnly => false;

    public void Add(TKey key, TValue value)
    {
        if ((double)count / size >= loadFactor)
        {
            Resize();
        }

        int index = GetIndex(key);
        if (!occupied[index])
        {
            table[index] = new KeyValuePair<TKey, TValue>(key, value);
            occupied[index] = true;
            count++;
        }
        else if (table[index].Key.Equals(key))
        {
            throw new ArgumentException("Same Key");
        }
        else
        {
            // Debug.Log(index);
            throw new InvalidOperationException("Hash Collision");
        }
    }
    
    public void Resize()
    {
        int newSize = size * 2;
        var newTable = new KeyValuePair<TKey, TValue>[newSize];
        var newOccupied = new bool[newSize];

        for (int i = 0; i < size; i++)
        {
            if (!occupied[i])
                continue;

            var newIndex = GetIndex(table[i].Key, newSize);

            if (newOccupied[newIndex])
                throw new InvalidOperationException("Hash Collision");

            newTable[newIndex] = table[i];
            newOccupied[newIndex] = true;
        }

        table = newTable;
        occupied = newOccupied;
        size = newSize;
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }

    public void Clear()
    {
        Array.Clear(table, 0, size);
        Array.Clear(occupied, 0, size);
        count = 0;
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        int index = GetIndex(item.Key);
        return ContainsKey(item.Key) && table[index].Value.Equals(item.Value);
    }

    public bool ContainsKey(TKey key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        int index = GetIndex(key);
        return occupied[index] && table[index].Key.Equals(key); 
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        for (int i = 0; i < size; i++)
        {
            if (occupied[i])
                array[arrayIndex++] = new KeyValuePair<TKey, TValue>(table[i].Key, table[i].Value);
        }
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        for (int i = 0; i < size; i++)
        {
            if (occupied[i])
                yield return table[i];
        }
    }

    public bool Remove(TKey key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        int index = GetIndex(key);
        if (occupied[index] && table[index].Key.Equals(key))
        {
            occupied[index] = false;
            table[index] = default;
            count--;
            return true;
        }

        return false;
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        return Remove(item.Key);
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        int index = GetIndex(key);
        if (occupied[index] && table[index].Key.Equals(key))
        {
            value = table[index].Value;
            return true;
        }

        value = default;
        return false;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
