using System.Collections.Generic;
using System;
public class RBT<T> : BST<T> where T : MyComparableInterface
{
    protected class RBNode<Y> : BST<T>.Node<T> where Y : MyComparableInterface
    {
        public bool isRed;
        new public RBNode<T>[] context = new RBNode<T>[2];
        public RBNode(T data) : base(data) 
        {  
            // base(data)가 부모의 생성자를 호출해줌.
            // additional work 만 적어주면 됨.
            // null node는 무조건 black. 새로 생성되는 RBNode는 언제나 red
            this.isRed = (data != null); 
        }
    }
    new protected RBNode<T> root;
    /// <summary>
    /// Method <c>insert</c> insert given data to RBT.
    /// Rebalances automatically by using <c>checkAndRepairInsertion</c> method
    /// </summary>    
    new protected void insert(T data) {
        // 1. insert using naive bst insert
        RBNode<T> node = new RBNode<T>(data);
        this.insert(node);
        checkAndRepairInsertion(node);
    }
    /// <summary>
    /// Method <c>remove</c> removes given data from RBT if it exists.
    /// Rebalances automatically by using <c>replaceNode</c> method
    /// </summary>    
    new protected bool remove(T data) {
        return this.remove(data, this.replaceNode);
    }

    /// <summary>
    /// Method <c>replaceNode</c> replaces given node with replacement node in RBT
    /// Checks for violation and rebalances automatically using <c>checkAndRepairDeletion</c> method
    /// </summary>   
    private void replaceNode(RBNode<T> toReplace, RBNode<T> replacement){
        base.replaceNode(toReplace, replacement);
        if(!toReplace.isRed && replacement.isRed)
            replacement.isRed = false;
        if(!toReplace.isRed && (!replacement.isRed || replacement == null))
            checkAndRepairDeletion(replacement);
    }
    protected void checkAndRepairInsertion(RBNode<T> node) {
        if(isRRViolated(node))
            repairRR(node);
        if(isHeadViolated())
            this.root.isRed = false;
    }
    protected void checkAndRepairDeletion(RBNode<T> node) {
        /* input node is A
        / case 1 :
        / brother : black
        / child of brother : red
        / child of child of brother : black (null)
        / rotate C D E, color swap D and E >> rotate A B D, color 
        /
        / case 2 : 
        / brother : black
        / child of brother : black (null)
        / recolor, then resolve on parent
        /
        / case 3:
        / brother red
        / child of brother : black (null)
        / rotate & colo swap, then resolve
        */

        // if parent is null return
        if(node.context[0] == null) return;
        int nodeContext = 0;

        // if node is left child, nodeContext = 1. else, 2
        if (node.context[0].context[1] == node)
            nodeContext = 1;
        else
            nodeContext = 2;
        
        // get brother. if null, 
        RBNode<T> brother;
        if(node.context[0].context[3-nodeContext] == null) {
            brother = new RBNode<T>(default(T));
            brother.isRed = false;
        }
        else
            brother = node.context[0].context[3- nodeContext];
        
        // case 1 & 2; black brother
        if (!brother.isRed) {
            // case 1 : child of brother is red
            RBNode<T> brotherChild1 = brother.context[nodeContext];
            RBNode<T> brotherChild2 = brother.context[3-nodeContext];
            // if child on side of nodeContext is red, rotate and color swap
            if(brotherChild1 != null && brotherChild2 == null) {
                if(brotherChild1.isRed) {
                    this.rotate(brotherChild1, brother);
                    brotherChild1.isRed = false;
                    brother.isRed = true;
                    brother = node.context[0].context[3-nodeContext];
                    brotherChild1 = brother.context[nodeContext];
                    brotherChild2 = brother.context[3-nodeContext];
                    // this ensures that brotherChild2 is not null
                }
            }
            // rotate, color swap, recolor
            if(brotherChild2 != null) {
                if(brotherChild2.isRed) {
                    bool pRed = node.context[0].isRed;
                    this.rotate(brother, node.context[0]);
                    node.context[0].isRed = brother.isRed;
                    brother.isRed = pRed;
                    brotherChild2.isRed = false;
                    return;
                }
            }
            // case 2 : child of brother is black
            bool cB1 = brotherChild1 == null ? true : brotherChild1.isRed;
            bool cB2 = brotherChild2 == null ? true : brotherChild2.isRed;
            if (cB1 && cB2) {
                brother.isRed = true;
                checkAndRepairDeletion(node.context[0]);
                return;
            }
        }
        //case 3 : red brother
        else {
            RBNode<T> parent = node.context[0];
            bool[] pCr = { parent.isRed, brother.isRed };
            this.rotate(brother, parent);
            brother.isRed = pCr[0];
            parent.context[0].isRed = pCr[1];
            checkAndRepairDeletion(node);
        }


    }  
    private bool isHeadViolated() {
        // if root but red, return true
        if(this.root.isRed)
            return true;
        return false;
    }
    private bool isRRViolated(RBNode<T> node) {
        // if parent is red and self is red, return true
        if(node.context[0] == null)
            return false;
        else if(node.context[0].isRed && node.isRed)
            return true;
        else
            return false;
    }
    private void repairRR(RBNode<T> node) {
        // repair red node with red child
        if (node.context[0].isRed && node.isRed) {
            RBNode<T> gp = node.context[0].context[0];
            //predSide[0] is for aunt, predSide[1] is for parent
            int[] predSide = {1,2};
            if (gp.context[1] == node.context[0]) predSide = new int[2] {2,1};
            // if aunt is black or null, (rotate), rotate & color swap
            if (!gp.context[predSide[0]].isRed || gp.context[predSide[0]] == null) {
                // if node and parent are not in same direction
                if(node != node.context[0].context[predSide[1]]) {
                    // rotate node and its parent (d and e)
                    rotate(node, node.context[0]);
                }
                // rotate parent and grandparent
                RBNode<T> toSwap = node.context[0];
                rotate(node, toSwap);
                bool temp = node.isRed;
                node.isRed = toSwap.isRed;
                toSwap.isRed = temp;
            }
            // if aunt is red, recolor, then check
            else if (gp.context[predSide[0]].isRed) {
                gp.context[1].isRed = !gp.context[1].isRed;
                gp.context[2].isRed = !gp.context[2].isRed;
                gp.isRed = !gp.isRed;
                if(isRRViolated(gp)) {
                    checkAndRepairInsertion(gp);
                }
            }
        }
    }   

    /*
    private RBNode<T>[] fixLeavesArray() {
        RBNode<T>[] leaves = new RBNode<T>[nullCount];
        int newNullCount = 0;
        foreach(RBNode<T> leaf in this.nullChildren) {
            if(leaf == null)
                break;
            if(leaf.context[0].context[1] == leaf || leaf.context[0].context[2] == leaf) {
                leaves[newNullCount] = leaf;
                newNullCount++;
            }
        }
        if(newNullCount / nullChildren.Length >= 0.75) {
            RBNode<T>[] fixedLeaves = new RBNode<T>[nullCount * 2];
            for(int i = 0; i < newNullCount; i++) {
                fixedLeaves[i] = leaves[i];
            }
            leaves = fixedLeaves;
            this.nullCount = newNullCount;
        }
        return leaves;
    }

    private void addNullChildren(RBNode<T> node) {
        int mode = 1;
        do {
        RBNode<T> newNull = new RBNode<T>(default(T));
        newNull.context[0] = node;
        node.context[mode] = newNull;
        this.nullChildren[nullCount] = newNull;
        nullCount++;
        mode++;
        } while(mode < 3);
    }

    // this needs to be updated    
    private bool isPathCostViolated() {
        int[] pathBlackCount = new int[nullCount];
        int pathCount = -1;
        foreach(RBNode<T> leaf in this.nullChildren) {
            if(leaf == null) break;
            int currentPathCount = 0;
            RBNode<T> current = leaf;
            while(current.context[0] != null)
            {
                if(!current.isRed)
                    currentPathCount++;
                current = current.context[0];
            }
            if(pathCount != currentPathCount) {
                if(pathCount != -1)
                    return true;
                pathCount = currentPathCount;
            }
        }
        return false;
    }
  */
}