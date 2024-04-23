using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Testing : MonoBehaviour
{
    void Start()
    {
        //init();
        rung();
    }

    void init() {
        Backend be = new Backend();
        be.load();
        //be.createGraph(new DijkstraGraph<Character, int>());
        be.createTree(new RBT<Character>(), 2);
        
        RBT<Character> tree = (RBT<Character>)be.getTree();
        //DijkstraGraph<Character, int> graph = be.getDijkstraGraph();
        //Debug.Log(graph.getNodeCount());
        List<Character> chars = tree.dataPreOrder();
        foreach(Character character in chars) {
            Debug.Log(character.getLevel() + ", " + character.getName());
        }
        
    }

    
    // Update is called once per frame
    void Update()
    {
        
    }

    void rung() {
        Dictionary<string, int>[] dictionaries;
        Dictionary<string, int> runCharCount = new Dictionary<string, int>();
        dictionaries = new Dictionary<string, int>[10];
        dictionaries[0] = runCharCount;
        runCharCount.Add("a", 1);
        runCharCount.Add("b", 2);
        runCharCount.Add("c", 3);
        dictionaries[0]["a"] = 4;
        Debug.Log(dictionaries[0]["a"]);
    }
    

}
