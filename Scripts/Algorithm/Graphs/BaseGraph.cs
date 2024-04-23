using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BaseGraph<NodeType, integer> {
    public class Node {
        public NodeType data;
        public LinkedList<Edge> edgesLeaving = new LinkedList<Edge>();
        public LinkedList<Edge> edgesEntering = new LinkedList<Edge>();
        public Node(NodeType data) {
            this.data = data;
        }
    }

    protected Hashtable nodes = new Hashtable();

    public class Edge {
        public int data;
        public Node predecessor;
        public Node successor;
        public Edge (int data, Node pred, Node succ) {
            this.data = data;
            this.predecessor = pred;
            this.successor = succ;
        }

        public double toDouble() {
            return Convert.ToDouble(data);
        }
    }
    protected int edgeCount = 0;

    public bool insertNode(NodeType data) {
        if(nodes.ContainsKey(data)) return false; // throws NPE when data's null
        nodes.Add(data,new Node(data));
        return true;
    }

    public bool removeNode(NodeType data) {
        // remove this node from nodes collection
        if(!nodes.ContainsKey(data)) return false; // throws NPE when data==null
        //Node oldNode = nodes.Remove(data);
        Node oldNode = (Node)nodes[data];
        // remove all edges entering neighboring nodes from this one
        foreach (Edge edge in oldNode.edgesLeaving)
            edge.successor.edgesEntering.Remove(edge);
        // remove all edges leaving neighboring nodes toward this one
        foreach(Edge edge in oldNode.edgesEntering)
            edge.predecessor.edgesLeaving.Remove(edge);
        return true;
    }

    public bool containsNode(NodeType data) {
        return nodes.ContainsKey(data);
    }
    
    public int getNodeCount() {
        return nodes.Count;
    }

    public bool insertEdge(NodeType pred, NodeType succ, int weight) {
        // find nodes associated with node data, and return false when not found
        Node predNode = (Node)nodes[pred];
        Node succNode = (Node)nodes[succ];
        if(predNode == null || succNode == null) return false;
        try {
            // when an edge alread exists within the graph, update its weight
            Edge existingEdge = getEdgeHelper(pred,succ);
            existingEdge.data = weight;
        } catch(Exception e) {
            Console.WriteLine(e.Message);
            // otherwise create a new edges
            Edge newEdge = new Edge(weight,predNode,succNode);
            this.edgeCount++;
            // and insert it into each of its adjacent nodes' respective lists
            predNode.edgesLeaving.AddLast(newEdge);
            succNode.edgesEntering.AddLast(newEdge);
        }
        return true;
    }

    public bool removeEdge(NodeType pred, NodeType succ) {
        try {
            // when an edge exists
            Edge oldEdge = getEdgeHelper(pred,succ);        
            // remove it from the edge lists of each adjacent node
            oldEdge.predecessor.edgesLeaving.Remove(oldEdge);
            oldEdge.successor.edgesEntering.Remove(oldEdge);
            // and decrement the edge count before removing
            this.edgeCount--;
            return true;
        } catch(Exception e) {
            // when no such edge exists, return false instead
            Console.WriteLine(e.Message);
            return false;
        }
    }

    public bool containsEdge(NodeType pred, NodeType succ) {
        try { getEdgeHelper(pred,succ); return true; }
        catch(Exception e) { 
            Console.WriteLine(e.Message);
            return false; 
        }
    }

    public int getEdge(NodeType pred, NodeType succ) {
        return getEdgeHelper(pred,succ).data;
    }
    
    protected Edge getEdgeHelper(NodeType pred, NodeType succ) {
        Node predNode = (Node)nodes[pred];
        // search for edge through the predecessor's list of leaving edges
        foreach(Edge edge in predNode.edgesLeaving)
            // compare succ to the data in each leaving edge's successor
            if(edge.successor.data.Equals(succ))
                return edge;
        // when no such edge can be found, throw NSE
        throw new Exception("No edge from "+pred.ToString()+" to "+
                                         succ.ToString());
    }

    public int getEdgeCount() {
        return this.edgeCount;
    }
}