using NUnit.Framework;
using UnityEngine;

public class HashTableTest : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // var hashTable = new OpenAdressingHashTable<string, int>(ProbingStrategy.DoubleHash);
        var hashTable = new ChainingHashTable<string, int>();

        hashTable.Add("1", 1);
        hashTable.Add("2", 2);
        hashTable.Add("3", 3);
        hashTable.Add("4", 4);
        hashTable.Add("5", 5);
        hashTable.Add("6", 6);
        hashTable.Add("7", 7);
        hashTable.Add("8", 8);

        foreach (var kvp in hashTable)
        {
            Debug.Log(kvp.Key + ", " + kvp.Value);
        }

        hashTable.Remove("6");
        foreach (var kvp in hashTable)
        {
            Debug.Log(kvp.Key + ", " + kvp.Value);
        }
        Debug.Log(hashTable.Count);

        hashTable.Add("15", 15);
        foreach (var kvp in hashTable)
        {
            Debug.Log(kvp.Key + ", " + kvp.Value);
        }
        Debug.Log(hashTable.Count);

        hashTable.Clear();
        for (int i = 0; i < 50; i++)
        {
            hashTable.Add($"{i}", i);
        }
        Debug.Log(hashTable.Keys.Count);
        Debug.Log(hashTable.Values.Count);
        Debug.Log(hashTable.Count);
        for (int i = 0; i < 20; i++)
        {
            hashTable.Remove($"{i}");
        }
        Debug.Log(hashTable.Keys.Count);
        Debug.Log(hashTable.Values.Count);
        Debug.Log(hashTable.Count);
    }
}
