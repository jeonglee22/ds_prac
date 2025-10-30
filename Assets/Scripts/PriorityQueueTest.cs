using System.Linq;
using UnityEngine;

public class PriorityQueueTest : MonoBehaviour
{
    [SerializeField] private int nodeCount = 10;
    [SerializeField] private int minKey = 1;
    [SerializeField] private int maxKey = 1000;

    private PriorityQueue<string, int> minHeap;

    public int keyInput;

    private void Start()
    {
        var pq = new PriorityQueue<string, int>();
        pq.Enqueue("Low", 1);
        pq.Enqueue("High", 5);
        pq.Enqueue("Medium", 10);

        Debug.Log(pq.Dequeue()); // "High" (우선순위 1)
        Debug.Log(pq.Dequeue()); // "Medium" (우선순위 5)
        Debug.Log(pq.Dequeue()); // "Low" (우선순위 10)

        RegeneratePQ();
    }

    public void GenerateRandomPQ()
    {
        minHeap = new PriorityQueue<string, int>();

        int addedNodes = 0;
        while (addedNodes < nodeCount)
        {
            int key = Random.Range(minKey, maxKey + 1);

            string value = $"V-{key}";
            minHeap.Enqueue(value, key);
            addedNodes++;
        }
    }

    [ContextMenu("Generate New Random Heap")]
    public void RegeneratePQ()
    {
        GenerateRandomPQ();
        ShowPQ();
    }

    [ContextMenu("Remove Element in Heap")]
    public void DequeueElement()
    {
        var ele = minHeap.Dequeue();
        Debug.Log(ele);
        ShowPQ();
    }

    [ContextMenu("Add Element in Heap")]
    public void EnqueueElement()
    {
        int key = Random.Range(minKey, maxKey + 1);

        string value = $"V-{key}";
        minHeap.Enqueue(value, key);

        ShowPQ();
    }

    private void ShowPQ()
    {
        Debug.Log(minHeap.ShowElement());
    }
}
