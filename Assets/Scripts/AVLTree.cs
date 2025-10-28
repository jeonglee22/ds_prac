using System;
using Unity.Android.Gradle;
using UnityEngine;


public class AVLTree<Tkey, TValue> : BinarySearchTree<Tkey, TValue> where Tkey : IComparable<Tkey>
{
    public AVLTree() : base()
    {
    }

    protected override TreeNode<Tkey, TValue> Add(TreeNode<Tkey, TValue> node, Tkey key, TValue value)
    {
        node = base.Add(node, key, value);

        return Balance(node);
    }

    protected override TreeNode<Tkey, TValue> AddorUpdate(TreeNode<Tkey, TValue> node, Tkey key, TValue value)
    {
        node = base.AddorUpdate(node, key, value);
        return Balance(node);
    }

    protected override TreeNode<Tkey, TValue> Remove(TreeNode<Tkey, TValue> node, Tkey key)
    {
        node = base.Remove(node, key);
        if (node == null)
        {
            return node;
        }
        return Balance(node);
    }

    protected int BalanceFactor(TreeNode<Tkey, TValue> node)
    {
        return node == null ? 0 : Height(node.Left) - Height(node.Right);
    }

    protected TreeNode<Tkey, TValue> Balance(TreeNode<Tkey, TValue> node)
    {
        int balanceFactor = BalanceFactor(node);

        if (balanceFactor > 1)
        {
            if(BalanceFactor(node.Left) < 0)
            {
                node.Left = RotateLeft(node.Left);
            }

            return RotateRight(node);
        }
        else if (balanceFactor < -1)
        {
            if(BalanceFactor(node.Right) > 0)
            {
                node.Right = RotateRight(node.Right);
            }

            return RotateLeft(node);
        }

        return node;
    }

    protected TreeNode<Tkey, TValue> RotateRight(TreeNode<Tkey, TValue> node)
    {
        var leftChild = node.Left;
        var rightSubtreeOfLeftChild = leftChild.Right;

        leftChild.Right = node;
        node.Left = rightSubtreeOfLeftChild;

        UpdateHeight(node);
        UpdateHeight(leftChild);

        return leftChild;
    }

    protected TreeNode<Tkey, TValue> RotateLeft(TreeNode<Tkey, TValue> node)
    {
        var rightChild = node.Right;
        var leftSubtreeOfRightChild = rightChild.Left;

        rightChild.Left = node;
        node.Right = leftSubtreeOfRightChild;

        UpdateHeight(node);
        UpdateHeight(rightChild);

        return rightChild;
    }
}
