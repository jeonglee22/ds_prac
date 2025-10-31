using NUnit.Framework;
using UnityEngine;

public class HashTableTest : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var hashTable = new SimpleHashTable<string, int>();

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

        hashTable.Add("15", 15);
        foreach (var kvp in hashTable)
        {
            Debug.Log(kvp.Key + ", " + kvp.Value);
        }

        for (int i = 20; i < 50; i++)
        {
            hashTable.Add($"{i}", i);
        }
        foreach (var kvp in hashTable)
        {
            Debug.Log(kvp.Key + ", " + kvp.Value);
        }
        Debug.Log(hashTable.Count);
    }
}
