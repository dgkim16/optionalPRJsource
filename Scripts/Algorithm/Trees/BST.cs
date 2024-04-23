using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class BST<T> : BSTInterface<T> where T : MyComparableInterface
{
    protected class Node<Y> : IComparable where Y : MyComparableInterface {
        public Y data;
        public Node<Y>[] context = new Node<Y>[3];
        public Node(Y data) { this.data = data; }
        public bool isRtChild() { return this.context[0].context[2] == this; }

        public int CompareTo(Node<T> obj)
        {
            return this.CompareTo(obj, 1);
        }

        public int CompareTo(object obj)
        {
            if(obj.GetType() == typeof(Node<T>))
                return this.CompareTo(obj as Node<T>);
            else
                throw new ArgumentException("Object is not a Node");
        }

        public int CompareTo(Node<T> obj, int mode)
        {
            if(obj == null) return 1;
            return this.data.CompareTo(obj.data, mode);
        }
    }

    protected Node<T> root;
    protected int size = 0;
    int Size { get { return this.size; } }
    public bool isEmpty() { return this.size == 0; }
    private Dictionary<T, Node<T>> nodeDict = new Dictionary<T, Node<T>>();
    private int compareIndex = 0;

    public void setCompareIndex(int index) {
        this.compareIndex = index;
    }

    ///<summary>
    ///Method <c>insert</c> preforms naive insertion of data into BST. No balancing.
    ///</summary>
    public void insert(T data)
    {
        if(data == null)
            throw new System.NotImplementedException("Data is null");
        Node<T> newNode = new Node<T>(data);
        nodeDict.Add(data, newNode);
        this.insert(newNode);
    }

    protected void insert(Node<T> newNode)
    {
        if(newNode == null)
            throw new System.NotImplementedException("Node is null");
        if(this.root == null) {
            this.root = newNode;
            return;
        } else {
            Node<T> current = this.root;
            while(true) {
                int compVal = newNode.CompareTo(current, this.compareIndex);
                if(compVal == 0 && current != this.root)
                    throw new System.ArgumentException("Data already exists in tree");
                else if(compVal < 0) {
                    if(current.context[1] == null) {
                        current.context[1] = newNode;
                        newNode.context[0] = current;
                        this.size++;
                        return;
                    } else {
                        current = current.context[1];
                    }
                } else if(compVal > 0) {
                    if(current.context[2] == null) {
                        current.context[2] = newNode;
                        newNode.context[0] = current;
                        return;
                    } else {
                        current = current.context[2];
                    }
                } else {
                    UnityEngine.Debug.Log("Something went wrong...");
                }

            }
        }
    }

    /// <summary>
    /// Method <c>rotate</c> rotates given child node to its parent node, if child is a child of parent.
    /// </summary>
    protected void rotate(Node<T> child, Node<T> parent) {
        // childContext == 1 : child is left child
        // childContext == 2 : child is right child
        int childContext = 0;
        if(child.context[0] != parent)
            throw new System.ArgumentException("Child is not a child of parent"); 
        else if (!child.isRtChild())
            childContext = 1;
        else if (child.isRtChild())
            childContext = 2;
        else
            throw new System.ArgumentException("Somthing is off. Child is not a child of parent");
        // is parent root?
        if(parent.context[0] != null) {
            if(parent.isRtChild()) {
                parent.context[0].context[2] = child;
            } else {
                parent.context[0].context[1] = child;
            }
        } else {
            this.root = child;
        }

        // child has children that needs to change its parent
        if(child.context[3-childContext] != null) {
            parent.context[childContext] = child.context[3-childContext];
            child.context[3-childContext].context[0] = parent.context[childContext];
            child.context[3-childContext] = null;
        }
        else
            parent.context[childContext] = null;

        child.context[3-childContext] = parent;
        parent.context[0] = child;
    }

    
    /// <summary>
    /// Method <c>remove</c> removes given data from BST if it exists.
    /// No rebalancing
    /// </summary>
    protected bool remove(T data, Action<Node<T>, Node<T>> methodName)
    {
        if (data == null) {
            UnityEngine.Debug.Log("while removing node in BST, data was found to be null. returning false");
            return false;
        }
        Node<T> nodeWithData = this.findNode(data);
        if (nodeWithData == null)
            return false;
        nodeDict.Remove(data);
        bool[] hasChild = new bool[2];
        hasChild[0] = nodeWithData.context[1]!=null;
        hasChild[1] = nodeWithData.context[2]!=null;
        if(hasChild[0] && hasChild[1]) {
            Node<T> scNode = this.findMinRtSubtree(nodeWithData);
            nodeWithData.data = scNode.data;
            if(scNode.context[2] == null)
                methodName(scNode, null);
            else
                methodName(scNode, scNode.context[2]);
        } else if (hasChild[1])
            methodName(nodeWithData, nodeWithData.context[2]);
        else if (hasChild[0])
            methodName(nodeWithData, nodeWithData.context[1]);
        else
            methodName(nodeWithData, null);
        this.size--;
        return true;
    }

    public bool remove(T data) {
        return this.remove(data, replaceNode);
    }

    protected void replaceNode(Node<T> toReplace, Node<T> replacement) 
    {
        if (toReplace == null) 
            throw new System.NullReferenceException("cannot replace null node");
        if (toReplace.context[0] == null) {
            if (replacement != null)
                replacement.context[0] = null;
            this.root = replacement;
        } else {
            if (replacement != null)
                replacement.context[0] = toReplace.context[0];
            if (toReplace.isRtChild())
                toReplace.context[0].context[2] = replacement;
            else
                toReplace.context[0].context[1] = replacement;
        }
    }
    public bool contains(T data) {
        if(data == null) return false;
        Node<T> nodeWithData = this.findNode(data);
        return (nodeWithData != null);
    }
    protected Node<T> findNode(T data)
    {
        if(nodeDict.TryGetValue(data, out Node<T> node))
            return node;
        else
            return null;
        /*
        Node<T> current = this.root;
        while(current != null) {
            int compVal = current.data.CompareTo(data, this.compareIndex);
            if(compVal == 0)    
                return current;
            else if(compVal < 0)    
                current = current.context[1];
            else    
                current = current.context[2];
        }
        return null;
        */
    }

    public T GetLargetData() {
        if(this.root == null)
            throw new Exception("Tree is empty");
        Node<T> current = this.root;
        while(current.context[2] != null)
            current = current.context[2];
        return current.data;
    }

    public T GetSmallestData() {
        if(this.root == null)
            throw new Exception("Tree is empty");
        Node<T> current = this.root;
        while(current.context[1] != null)
            current = current.context[1];
        return current.data;
    }
        
    protected Node<T> findMinRtSubtree(Node<T> node) {
        if (node.context[1] == null && node.context[2] == null)
            throw new Exception("Input Node must have two children");
        Node<T> current = node.context[2];
        while(current.context[1] != null)
            current = current.context[1];
        return current;
    }
    
    /// <summary>
    /// Method <c>findNode</c> finds node with given data in BST.
    /// In Order Traversal : 1. 위에서 아래로, 2. 왼쪽에서 오른쪽으로
    /// Stack 을 이용한 In Order Traversal
    /// 작은거 부터 큰거 까지, left -> root -> right
    /// </summary>
    public List<T> dataInOrder() {
        List<T> dataRet = new List<T>();
        if(this.root == null)
            return dataRet;
        Node<T> current = this.root;
        Stack<Node<T>> stack = new Stack<Node<T>>();
        while (!(stack.Count == 0) || current != null) {
            if (current == null) {
                Node<T> popped = stack.Pop();
                dataRet.Add(popped.data);
                current = popped.context[2];
            } else {
                stack.Push(current);
                current = current.context[1];
            }
        }
        return dataRet;
    }

    // hard coded for now. T is CharacterBattle. need to make it more generic.
    // deprecated
    public List<T> dataInOrderFilter(bool targetIsAlly) {
        List<T> dataRet = new List<T>();
        if(this.root == null)
            return dataRet;
        Node<T> current = this.root;
        Stack<Node<T>> stack = new Stack<Node<T>>();
        while (!(stack.Count == 0) || current != null) {
            if (current == null) {
                Node<T> popped = stack.Pop();
                if(((CharacterBattle)((object)popped.data)).isPlayable == targetIsAlly)
                    dataRet.Add(popped.data);
                current = popped.context[2];
            } else {
                stack.Push(current);
                current = current.context[1];
            }
        }
        return dataRet;
    }

    protected List<Node<T>> nodeInOrder() {
        List<T> datas = this.dataInOrder();
        List<Node<T>> nodes = new List<Node<T>>();
        foreach(T data in datas) {
            nodes.Add(this.findNode(data));
        }
        return nodes;
    }

    /// <summary>
    /// Method <c>dataPreOrder</c> returns data in Pre order traversal
    /// This method uses recursive helper method called <c>dataPrePostOrder</c>
    /// root -> left -> right
    /// </summary>
    public List<T> dataPreOrder() {
        List<T> dataRet = new List<T>();
        if(this.root == null)
            return dataRet;
        dataPrePostOrder(this.root, dataRet, 0);
        return dataRet;
    }

    /// <summary>
    /// Method <c>dataPostOrder</c> returns data in Post order traversal
    /// This method uses recursive helper method called <c>dataPrePostOrder</c>
    /// left -> right -> root
    /// </summary>
    public List<T> dataPostOrder() {
        List<T> dataRet = new List<T>();
        if(this.root == null)
            return dataRet;
        dataPrePostOrder(this.root, dataRet, 3);
        return dataRet;
    }

    /// <summary>
    /// Method <c>dataPrePostOrder</c> traverses & adds node in either pre or post order.
    /// </summary>
    /// <paramref>mode</paramref> is <typeparamref>integer</typeparamref>.
    /// 0 means pre order, and 3 means post order
    private void dataPrePostOrder(Node<T> node, List<T> dataList, int mode) {
        if(node == null) return;
        dataList.Add(node.data);
        // mode 0 pre order traversa;
        // mode 3 post order traversal

        dataPrePostOrder(node.context[Math.Abs(1-mode)], dataList, mode);
        dataPrePostOrder(node.context[Math.Abs(2-mode)], dataList, mode);
    }

    /// <summary>
    /// Method <c>dataLevelOrder</c> returns data in Level order traversal.
    /// Level Order Traversal : 레벨별로 왼쪽에서 오른쪽으로
    /// Queue 를 이용한 Level Order Traversal
    /// </summary>
    public List<T> dataLevelOrder() {
        List<T> dataRet = new List<T>();
        if(this.root == null)
            return dataRet;
        Queue<Node<T>> queue = new Queue<Node<T>>();
        queue.Enqueue(this.root);
        while(queue.Count != 0) {
            Node<T> current = queue.Dequeue();
            dataRet.Add(current.data);
            if(current.context[1] != null)
                queue.Enqueue(current.context[1]);
            if(current.context[2] != null)
                queue.Enqueue(current.context[2]);
        }
        return dataRet;
    }


}
