using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TTFT<T> : BST<T> where T : MyComparableInterface
{
    protected class TTFTNode<Y> : BST<T>.Node<T> where Y : MyComparableInterface {
        public TTFTNode(T data) : base(data)
        {
            this.children = new TTFTNode<T>[4];
            this.keys = new LinkedList<T>();
            keys.AddFirst(data);
        }

        public TTFTNode(T data, TTFTNode<T> parentNode, TTFTNode<T>[] childNodes) : base(data)
        {
            this.parent = parentNode;
            this.children = childNodes;
            keys.AddFirst(data);
        }
        public TTFTNode<T> parent;
        public TTFTNode<T>[] children;
        public LinkedList<T> keys;
    }


// start from root.
// if root is null, insert as root.
// if greater than current value, go right.
// if smaller than current value, go left.
// if smaller than current value and no child node but there is space, change order
    new public void insert(T data) {
        if(this.root == null) {
            this.root = new TTFTNode<T>(data);
            return;
        }
        TTFTNode<T> compareNode = (TTFTNode<T>)this.root;
        insertLooper(compareNode, data);

    }

    private void insertLooper(TTFTNode<T> compareNode, T dataValue) {
        LinkedListNode<T> compareNodeKey = compareNode.keys.First;
        // start from root
        // if compareNodeKey is full, split, then try again
        if(compareNode.keys.Count == 3) {
            split(compareNode);
            insertLooper(compareNode, dataValue);
        }
        // compareNodeKey is not full (there is space)
        // data value is smaller than compareNodeKey
        int childIndex = 0;
        int keyIndex = 0;
        while(keyIndex < 3) {
            if(dataValue.CompareTo(compareNodeKey.Value) < 0) {
                // there is no child node
                if(compareNode.children[childIndex] == null) {
                    compareNode.keys.AddBefore(compareNodeKey, dataValue);    
                    return;
                }
                insertLooper(compareNode.children[childIndex], dataValue);
            }
            childIndex++;
            compareNodeKey = compareNodeKey.Next;
            keyIndex++;
        }
        
    }

    protected void split(TTFTNode<T> node) {
        if(node.keys.Count != 3)
            throw new System.Exception("Node must be full to split");
        TTFTNode<T> parentNode = new TTFTNode<T>(node.keys.First.Next.Value);
        TTFTNode<T> childNodeLft = new TTFTNode<T>(node.keys.First.Value);
        TTFTNode<T> childNodeRt = new TTFTNode<T>(node.keys.First.Next.Next.Value);
        childNodeLft.parent = parentNode;
        childNodeLft.children[0] = node.children[0];
        childNodeLft.children[0].parent = childNodeLft; //try
        childNodeLft.children[1] = node.children[1];
        childNodeLft.children[1].parent = childNodeLft; //try
        childNodeRt.parent = parentNode;
        childNodeRt.children[0] = node.children[2];
        childNodeRt.children[0].parent = childNodeRt; //try
        childNodeRt.children[1] = node.children[3];
        childNodeRt.children[1].parent = childNodeRt; //try
        parentNode.children[0] = childNodeLft;
        parentNode.children[1] = childNodeRt;
        parentNode.parent = node.parent;
        if(node.parent != null) {
            node.parent.children[System.Array.IndexOf(node.parent.children, node)] = parentNode;
        }
    }
}
