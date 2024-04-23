using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AVLT<T> : BST<T> where T : MyComparableInterface
{
    // AVL tree : self balancing binary tree, rotations to maintain balance
    // Keep H(height) of tree(with N keys) growing in O(log N)
    // bf = height(left subtree) - height(rt subtree)
    // balanced if |bf| < 2
    // all nodes must be balanced
    // sign of bf tells direction of inbalance

    protected class AVLNode<Y> : BST<T>.Node<T> where Y : MyComparableInterface
    {
        public int level;   // root level is 1. leaf level is height of tree
        public AVLNode(T data) : base(data)
        {
            this.level = 0;
        }
        public int getHeight() {
            int leftHeight = 0;
            int rightHeight = 0;
            if(this.context[1] != null)
                leftHeight = ((AVLNode<T>)this.context[1]).getHeight();
            if(this.context[2] != null)
                rightHeight = ((AVLNode<T>)this.context[2]).getHeight();
            return 1 + Mathf.Max(leftHeight, rightHeight);
        }
    }

    new protected void insert(T data) {
        // 1. insert using naive bst insert
        AVLNode<T> node = new AVLNode<T>(data);
        this.insert(node);
        // may need to add termination condition
        balanceAVL(node);
        updateAVLNodeLevel();
    }

    private void updateAVLNodeLevel() {
        List<Node<T>> nodes = this.nodeInOrder();
        AVLNode<T> root = (AVLNode<T>)this.root;
        foreach(Node<T> n in nodes) {
            AVLNode<T> avlNode = (AVLNode<T>)n;
            avlNode.level = root.getHeight() - avlNode.getHeight() + 1;
        }
    }

    private int calcBalanceFactor(AVLNode<T> node) {
        int leftHeight = 0;
        int rightHeight = 0;
        if(node.context[1] != null)
            leftHeight = ((AVLNode<T>)node.context[1]).getHeight();
        if(node.context[2] != null)
            rightHeight = ((AVLNode<T>)node.context[2]).getHeight();
        return leftHeight - rightHeight;
    }

    private void balanceAVL(AVLNode<T> insertedNode) {
        int depth = insertedNode.context[0] == null ? calcBalanceFactor((AVLNode<T>)insertedNode) : calcBalanceFactor((AVLNode<T>)insertedNode.context[0]);
        if(System.Math.Abs(depth) >= 2) {
            int initDir = depth > 0 ? 1 : 2;
            AVLNode<T> t1 = (AVLNode<T>)insertedNode.context[initDir];
            int secDir = calcBalanceFactor(t1) > 0 ? 1 : 2;
            AVLNode<T> t2 = (AVLNode<T>)t1.context[secDir];
            this.rotate(t2, t1);
        }
        if(insertedNode.context[0] != null)
            balanceAVL((AVLNode<T>)insertedNode.context[0]);
    }
}
