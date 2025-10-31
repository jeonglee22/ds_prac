using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ChainingHashTable<TKey, TValue> : IDictionary<TKey, TValue>
{
    private const int DefaultCapacity = 16;
    private const double LoadFactor = 0.75;

    private LinkedList<KeyValuePair<TKey, TValue>>[] table;
    private bool[] occupied;

    private int size;
    private int count;

    public TValue this[TKey key]
    {
        get
        {
            if (TryGetValue(key, out TValue value))
            {
                return value;
            }

            throw new KeyNotFoundException();
        }
        set
        {
            if (key == null)
            throw new ArgumentNullException();

            if ((double)count / size > LoadFactor)
            {
                Resize();
            }

            int index = GetIndex(key);
            var kvp = new KeyValuePair<TKey, TValue>(key, value);

            if (occupied[index])
            {
                var list = table[index];

                bool exist = false;
                foreach (var item in list)
                {
                    if (item.Key.Equals(key))
                    {
                        table[index].Remove(item);
                        exist = true;
                        break;
                    }
                }
                
                table[index].AddLast(kvp);
            }
            else
            {
                table[index] = new LinkedList<KeyValuePair<TKey, TValue>>();
                table[index].AddLast(kvp);
                occupied[index] = true;
                count++;
            }
        } 
    }

    public ICollection<TKey> Keys
    {
        get
        {
            var keys = new List<TKey>();
            for (int i = 0; i < size; i++)
            {
                if (occupied[i])
                {
                    keys.AddRange(table[i].Select(i => i.Key));
                }
            }
            return keys;
        }
    }

    public ICollection<TValue> Values 
    {
        get
        {
            var values = new List<TValue>();
            for (int i = 0; i < size; i++)
            {
                if (occupied[i])
                {
                    values.AddRange(table[i].Select(i => i.Value));
                }
            }
            return values;
        }
    }    

    public int Count => count;

    public bool IsReadOnly => false;

    public ChainingHashTable()
    {
        table = new LinkedList<KeyValuePair<TKey, TValue>>[DefaultCapacity];
        occupied = new bool[DefaultCapacity];
        size = DefaultCapacity;
        count = 0;
    }

    public LinkedList<KeyValuePair<TKey, TValue>> GetlistForKey(TKey key)
    {
        return table[GetIndex(key)];
    }

    public int GetIndex(TKey key)
    {
        if (key == null)
            throw new ArgumentNullException();

        int hash = key.GetHashCode();
        return Math.Abs(hash) % size;
    }

    public void Add(TKey key, TValue value)
    {
        if (key == null)
            throw new ArgumentNullException();

        if ((double)count / size > LoadFactor)
        {
            Resize();
        }

        int index = GetIndex(key);
        var kvp = new KeyValuePair<TKey, TValue>(key, value);

        if (occupied[index])
        {
            foreach (var item in table[index])
            {
                if (item.Key.Equals(key))
                    throw new ArgumentException();
            }

            table[index].AddLast(kvp);
        }
        else
        {
            table[index] = new LinkedList<KeyValuePair<TKey, TValue>>();
            table[index].AddLast(kvp);
            occupied[index] = true;
        }
        count++;
    }

    private void Resize()
    {
        var oldSize = size;
        var oldTable = table;
        var oldOccupied = occupied;

        size *= 2;
        table = new LinkedList<KeyValuePair<TKey, TValue>>[size];
        occupied = new bool[size];
        count = 0;

        for (int i = 0; i < oldSize; i++)
        {
            if (oldOccupied[i])
            {
                var list = oldTable[i];

                foreach (var item in list)
                {
                    Add(item.Key, item.Value);
                }
            }
        }
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
        if (TryGetValue(item.Key, out TValue value))
        {
            return value.Equals(item.Value);
        }

        return false;
    }

    public bool ContainsKey(TKey key)
    {
        return TryGetValue(key, out TValue _);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        var index = arrayIndex;

        foreach (var kvp in this)
        {
            array[index++] = kvp;
        }
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        for (int i = 0; i < size; i++)
        {
            if(occupied[i])
            {
                foreach (var item in table[i])
                {
                    yield return item;
                }
            }
        }
    }

    public bool Remove(TKey key)
    {
        if (key == null)
            throw new ArgumentNullException();

        int index = GetIndex(key);

        if (occupied[index])
        {
            foreach (var kvp in table[index])
            {
                if (kvp.Key.Equals(key))
                {
                    table[index].Remove(kvp);
                    count--;

                    if(table[index].Count == 0)
                    {
                        table[index] = null;
                        occupied[index] = false;
                    }

                    return true;
                }
            }
        }

        return false;
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        int index = GetIndex(item.Key);

        if (occupied[index])
        {
            foreach (var kvp in table[index])
            {
                if (kvp.Key.Equals(item.Key) && kvp.Value.Equals(item.Value))
                {
                    table[index].Remove(kvp);
                    count--;

                    if (table[index].Count == 0)
                    {
                        table[index] = null;
                        occupied[index] = false;
                    }
                    
                    return true;
                }
            }
        }

        return false;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        if (key == null)
            throw new ArgumentNullException();

        int index = GetIndex(key);

        if (occupied[index])
        {
            foreach (var kvp in table[index])
            {
                if (kvp.Key.Equals(key))
                {
                    value = kvp.Value;
                    return true;
                }
            }
        }

        value = default;
        return false;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
