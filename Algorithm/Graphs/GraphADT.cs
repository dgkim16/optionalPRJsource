using System;
using System.Collections.Generic;
public interface GraphADT<NodeType, integer> {
    public bool insertNode(NodeType data);
    public bool removeNode(NodeType data);
    public bool containsNode(NodeType data);
    public int getNodeCount();
    public bool insertEdge(NodeType pred, NodeType succ, int weight);
    public bool removeEdge(NodeType pred, NodeType succ);
    public bool containsEdge(NodeType pred, NodeType succ);
    public int getEdge(NodeType pred, NodeType succ);
    public int getEdgeCount();
    public List<NodeType> shortestPathData(NodeType start, NodeType end);
    public double shortestPathCost(NodeType start, NodeType end);

}