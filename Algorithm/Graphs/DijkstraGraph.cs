using System;
using System.Collections;
using System.Collections.Generic;

public class DijkstraGraph<NodeType, integer> : BaseGraph<NodeType, integer>, GraphADT<NodeType, integer> 
    {

    public class SearchNode : IComparable<SearchNode> {
        public Node node;
        public double cost;
        public SearchNode predecessor;
        public SearchNode(Node node, double cost, SearchNode predecessor) {
            this.node = node;
            this.cost = cost;
            this.predecessor = predecessor;
        }
        public int CompareTo(SearchNode other) {
            if( cost > other.cost ) return +1;
            if( cost < other.cost ) return -1;
            return 0;
        }
    }

    
    protected class PriorityQueue {
        LinkedList<SearchNode> list;
        public PriorityQueue() {
            list = new LinkedList<SearchNode>();
        }
        public void add (SearchNode toAdd) {
            // if list is empty, add toAdd to list
            if(list.Count == 0) {
                list.AddLast(toAdd);
                return;
            }
            // if list is not empty, iterate through list
            LinkedListNode<SearchNode> current = list.First;
            while(current != null) {
                // if toAdd is less than current, add toAdd before current
                if(toAdd.CompareTo(current.Value) < 0) {
                    list.AddBefore(current, toAdd);
                    return;
                }
                // if toAdd is equal to current, add toAdd after current
                if(toAdd.CompareTo(current.Value) == 0) {
                    list.AddAfter(current, toAdd);
                    return;
                }
                // if toAdd is greater than current, move to next node
                current = current.Next;
            }
            // if toAdd is greater than all nodes, add toAdd to end of list
            list.AddLast(toAdd);
        }

        public bool isEmpty() {
            return list.Count == 0;
        }

        public SearchNode poll() {
            if(list.Count == 0) throw new Exception("PriorityQueue is empty");
            SearchNode toReturn = list.First.Value;
            list.RemoveFirst();
            return toReturn;
        }

    }

    protected SearchNode computeShortestPath(NodeType start, NodeType end) {
        // TODO: implement in step 6
        // create SearchNode with start to create [S:S,0]
        if(!containsNode(start) || !containsNode(end))
            throw new Exception("No given end or start node exists in graph");
        double startWeight = 0.0;
        // predecessor for start node is null
        SearchNode startSearch = new SearchNode((Node)nodes[start], startWeight, null);
        // create PriorityQueue
        PriorityQueue pq = new PriorityQueue();
        // insert into priority queue
        pq.add(startSearch);
        SearchNode c = null;
        // hashtable for visited nodes
        Hashtable visited = new Hashtable();
        while(!pq.isEmpty()) {
        // [C: pred, cost] = removeMin();
        c = pq.poll();
        // if polled node was not visited
        if(!visited.ContainsValue(c.node)) {
            // mark c node as visited
            visited.Add(c.node.data, c.node);
            // if current searchNode is the end node, return the current searchNode
            if(c.node == nodes[end])
            return c;
            // set accumulated cost to tCost
            double tCost = c.cost;
            // for each edge with weight edge.data to unvisited successor edge.successor of c
            foreach(Edge edge in c.node.edgesLeaving) {
            // pq.insert([edge.successor, Cost + edge.data, c])
            if(!visited.ContainsValue(edge.successor))
                pq.add(new SearchNode(edge.successor, tCost + edge.toDouble(), c));
            }
        }
      }
      // if no value was returned within the while loop, throw NoSuchElementException
      throw new Exception("No path leads from given start node to given end node.");
    }

    public List<NodeType> shortestPathData(NodeType start, NodeType end) {
        SearchNode c = computeShortestPath(start, end);
        // retnodes will contain list of node data that will be returned
        LinkedList<NodeType> retnodes = new LinkedList<NodeType>();
        // back trace predecessors until predecessor is itself (being start node)
        while(c.predecessor != null) {    
            retnodes.AddFirst(c.node.data);
            c = c.predecessor;
        }
        // add start node to sequence
        retnodes.AddFirst(c.node.data);
        List<NodeType> retnodes2 = new List<NodeType>(retnodes);
        foreach(NodeType node in retnodes) {
            retnodes2.Add(node);
        }
        // linked list can be casted to list and returned
        // linked list is not inherited from list in c#
        // so a new list must be created
        return retnodes2;
    }

    public double shortestPathCost(NodeType start, NodeType end) {
        // get SearchNode using method from step 6.
        SearchNode endSearch = computeShortestPath(start, end);
        // if end node found and if exception was not thrown, simply return the cost stored.
        return endSearch.cost;
    }

    List<NodeType> GraphADT<NodeType, integer>.shortestPathData(NodeType start, NodeType end)
    {
        throw new NotImplementedException();
    }
}   