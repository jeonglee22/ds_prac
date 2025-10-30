using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework.Constraints;

public class PriorityQueue<TElement, TPriority>
    where TPriority : IComparable<TPriority>
{
    private List<(TElement Element, TPriority Priority)> heap;

    public PriorityQueue()
    {
        heap = new List<(TElement, TPriority)>();
    }

    public int Count => heap.Count;

    private int GetMorePriorityChild(int index)
    {
        var leftChild = index * 2 + 1;
        var rightChild = index * 2 + 2;

        int compare = Compare(leftChild, rightChild);
        return compare < 0 ? leftChild : rightChild;
    }

    private void Swap(int index1, int index2)
    {
        var temp = heap[index1];
        heap[index1] = heap[index2];
        heap[index2] = temp;
    }

    private int Compare(int index1, int index2)
    {
        return heap[index1].Priority.CompareTo(heap[index2].Priority);
    }

    public void Enqueue(TElement element, TPriority priority)
    {
        // TODO: 구현
        // 1. 새 요소를 리스트 끝에 추가
        // 2. HeapifyUp으로 힙 속성 복구
        heap.Add((element, priority));
        HeapifyUp(Count - 1);
    }

    public TElement Dequeue()
    {
        // TODO: 구현
        // 1. 빈 큐 체크 및 예외 처리
        // 2. 루트 요소 저장
        // 3. 마지막 요소를 루트로 이동
        // 4. HeapifyDown으로 힙 속성 복구
        // 5. 저장된 루트 요소 반환
        if (Count == 0)
            throw new Exception();

        var root = heap[0];

        heap[0] = heap[Count - 1];
        heap.RemoveAt(Count - 1);

        if(Count > 0)
            HeapifyDown(0);

        return root.Element;
    }

    public TElement Peek()
    {
        // TODO: 구현
        // 1. 빈 큐 체크 및 예외 처리
        // 2. 루트 요소 반환
        if (Count == 0)
            throw new Exception();

        return heap[0].Element;
    }

    public void Clear()
    {
        // TODO: 구현
        heap.Clear();
    }

    private void HeapifyUp(int index)
    {
        // TODO: 구현
        // 현재 노드가 부모보다 작으면 교환하며 위로 이동
        if (index == 0)
            return;

        int parent = (index - 1) / 2;

        if (Compare(index, parent) < 0)
        {
            Swap(index, parent);
            HeapifyUp(parent);
        }
    }

    private void HeapifyDown(int index)
    {
        // TODO: 구현
        // 현재 노드가 자식보다 크면 더 작은 자식과 교환하며 아래로 이동
        var leftChild = index * 2 + 1;
        var rightChild = index * 2 + 2;

        if (leftChild >= Count)
            return;
        else if (rightChild >= Count)
        {
            if(Compare(index, leftChild) > 0)
                Swap(index, leftChild);
            return;
        }

        if ((Compare(index, leftChild) <= 0) && (Compare(index, rightChild) <= 0))
            return;
        else
        {
            int priorIndex = GetMorePriorityChild(index);
            Swap(index, priorIndex);
            HeapifyDown(priorIndex);
        }
    }
    
    public string ShowElement()
    {
        var str = new StringBuilder();

        str.Append("Heap Elements : ");
        foreach (var i in heap)
        {
            str.Append(i.Element).Append(", ");
        }

        return str.ToString();
    }
}