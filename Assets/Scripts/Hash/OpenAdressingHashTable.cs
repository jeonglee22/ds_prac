using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.XR.Oculus.Input;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public enum ProbingStrategy
{
    Linear,
    Quadratic,
    DoubleHash,

}

public class OpenAdressingHashTable<TKey, TValue> : IDictionary<TKey, TValue>
{
    private const int DefaultCapacity = 16;
    private const double LoadFactor = 0.6;

    private KeyValuePair<TKey, TValue>[] table;
    private bool[] occupied;
    private bool[] deleted;
    private int size;
    private int count;

    private ProbingStrategy probingStrategy;
    public ProbingStrategy ProbingStrategy { get { return probingStrategy; } set { probingStrategy = value; }}

    public int Size { get { return size; } }
    public bool isSizeChanged { get; set; }

    public OpenAdressingHashTable(ProbingStrategy probingStrategy = ProbingStrategy.Linear)
    {
        table = new KeyValuePair<TKey, TValue>[DefaultCapacity];
        occupied = new bool[DefaultCapacity];
        deleted = new bool[DefaultCapacity];
        size = DefaultCapacity;
        count = 0;
        this.probingStrategy = probingStrategy;
    }

    public int GetPrimaryHash(TKey key)
    {
        if (key == null)
            throw new ArgumentNullException();

        int hash = key.GetHashCode();
        return Math.Abs(hash) % size;
    }
    
    public int GetSecondaryHash(TKey key)
    {
        int hash = key.GetHashCode();
        
        return 1 + (Math.Abs(hash) % (size - 1));
    }

    public int GetProbeIndex(TKey key, int attempt)
    {
        int primaryHash = GetPrimaryHash(key);
        int secondaryHash = GetSecondaryHash(key);

        switch (probingStrategy)
        {
            case ProbingStrategy.Linear:
                return (primaryHash + attempt) % size;
            case ProbingStrategy.Quadratic:
                return (primaryHash + attempt * attempt) % size;
            case ProbingStrategy.DoubleHash:
                return (primaryHash + attempt * secondaryHash) % size;
        }

        throw new ArgumentException();
    }

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

            if ((double)count / size >= LoadFactor)
            {
                Resize();
            }

            int attempt = 0;

            do
            {
                int index = GetProbeIndex(key, attempt);
                if (!occupied[index] || deleted[index])
                {
                    table[index] = new KeyValuePair<TKey, TValue>(key, value);
                    occupied[index] = true;
                    deleted[index] = false;
                    count++;
                    return;
                }

                if (table[index].Key.Equals(key))
                {
                    table[index] = new KeyValuePair<TKey, TValue>(key, value);
                    return;
                }

                attempt++;

                if(attempt > size)
                {
                    Resize();
                    attempt = 0;
                }
            }
            while (true);
        }
    }

    public ICollection<TKey> Keys => Enumerable.Range(0, size).Where(i => occupied[i] && !deleted[i]).Select(i => table[i].Key).ToList();

    public ICollection<TValue> Values => Enumerable.Range(0, size).Where(i => occupied[i] && !deleted[i]).Select(i => table[i].Value).ToList();

    public int Count => count;

    public bool IsReadOnly => false;

    public void Add(TKey key, TValue value)
    {
        if (key == null)
            throw new ArgumentNullException();

        if ((double)count / size >= LoadFactor)
        {
            Resize();
        }

        int attempt = 0;

        do
        {
            int index = GetProbeIndex(key, attempt);
            if (!occupied[index] || deleted[index])
            {
                table[index] = new KeyValuePair<TKey, TValue>(key, value);
                occupied[index] = true;
                deleted[index] = false;
                count++;
                return;
            }

            if (table[index].Key.Equals(key))
            {
                throw new ArgumentException();
            }

            attempt++;

            if(attempt > size)
            {
                Resize();
                attempt = 0;
            }
        }
        while (true);
    }

    private void Resize()
    {
        int newSize = size * 2;

        var oldTable = table;
        var oldOccupied = occupied;
        var oldDeleted = deleted;
        var oldSize = size;

        table = new KeyValuePair<TKey, TValue>[newSize];
        occupied = new bool[newSize];
        deleted = new bool[newSize];
        size = newSize;
        count = 0;

        for (int i = 0; i < oldSize; i++)
        {
            if (oldOccupied[i] && !oldDeleted[i])
            {
                Add(oldTable[i].Key, oldTable[i].Value);
            }
        }
        isSizeChanged = true;
    }
    
    public int FindIndex(TKey key)
    {
        if (key == null)
            throw new ArgumentNullException();

        int attempt = 0;

        do
        {
            int index = GetProbeIndex(key, attempt);
            if (!occupied[index] && !deleted[index])
            {
                return -1;
            }

            if (occupied[index] && !deleted[index] && table[index].Key.Equals(key))
            {
                return index;
            }

            attempt++;

        } while (attempt < size);

        return -1;
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }

    public void Clear()
    {
        Array.Clear(table, 0, size);
        Array.Clear(occupied, 0, size);
        Array.Clear(deleted, 0, size);
        count = 0;
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        int index = FindIndex(item.Key);
        if (index != -1)
        {
            return table[index].Value.Equals(item.Value);
        }

        return false;
    }

    public bool ContainsKey(TKey key)
    {
        return FindIndex(key) != -1;
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        int index = arrayIndex;

        foreach (var kvp in this)
        {
            array[index++] = kvp;
        }
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        for (int i = 0; i < size; i++)
        {
            if (occupied[i] && !deleted[i])
            {
                yield return table[i];
            }
        }
    }

    public bool Remove(TKey key)
    {
        int index = FindIndex(key);

        if (index != -1)
        {
            deleted[index] = true;
            count--;
            return true;
        }

        return false;
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        int index = FindIndex(item.Key);

        if (index != -1 && table[index].Value.Equals(item.Value))
        {
            deleted[index] = true;
            count--;
            return true;
        }

        return false;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        int index = FindIndex(key);

        if (index != -1)
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
