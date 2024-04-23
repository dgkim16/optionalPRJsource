using System;
using UnityEngine.SceneManagement;

public class Backend
{
    private AllSkills allSkills;
    private Character[] userChars;
    private BSTInterface<Character> treeType;
    private GraphADT<Character, int> graphType;

    public void load() { load("test", "test", false); }
    public void load(string id, string pw, bool isNetwork) {
        if(!isNetwork) 
            DataManager.readJson();
        else {
            GetApiData apiData = new GetApiData();
            DataManager.userInfo = apiData.GetData(id, pw);
        }
        DataManager.ReadSkillJson();
        userChars = new Character[DataManager.userInfo.userChars.Length];
        for(int i = 0; i < userChars.Length; i++)
            userChars[i] = new Character(DataManager.userInfo.userChars[i]);
    }
    ///summary
    ///<c>createTree</c> takes in a treeType and a mode, and creates a tree based on the mode.
    ///<c>mode</c> 0: name, 1: id, 2: level, 3: rank
    ///summary
    public void createTree(BSTInterface<Character> treeType, int mode) {
        treeType.setCompareIndex(mode);
        foreach(Character character in userChars) {
            treeType.insert(character);
        }
        this.treeType = treeType;
    }

    public void createTree(BSTInterface<Character> treeType) {
        this.createTree(treeType, 1);
    }

    // 친밀도 시스템으로 활용 가능한 graph
    public void createGraph(GraphADT<Character, int> graphType, int mode) {
        if(graphType == null) return;
        //GraphADT<Character, int> newGraph = graphType;
        Type type = graphType.GetType();
        GraphADT<Character, int> newGraph = (GraphADT<Character, int>)Activator.CreateInstance(type);
        // create nodes for all characters
        foreach(Character character in userChars) {
            newGraph.insertNode(character);
        }
        // connect nodes
        /*
        foreach(Character character in userChars) {
            foreach(Character otherCharacter in userChars) {
                if(character == otherCharacter) continue;
                newGraph.insertEdge(character, otherCharacter, 1);
            }
        }
        */
        this.graphType = newGraph;
    }

    public void createGraph(GraphADT<Character, int> graphType) {
        this.createGraph(graphType, 1);
    }

    public DijkstraGraph<Character, int> getDijkstraGraph() { 
        return (DijkstraGraph<Character, int>)graphType;
    }

    public BSTInterface<Character> getTree() { return treeType; }

    
    public void save() { DataManager.saveJson(DataManager.userInfo); }
    public void setUserName(string input) { DataManager.userInfo.userName = input; }
    public string getUserName() { return DataManager.userInfo.userName; }
    public string getUserHash() { return DataManager.userInfo.userHash; }
    public Character getCharacter(int index) { return userChars[index]; }
    public void setCharacter(int index, Character character) { DataManager.userInfo.userChars[index] = character.charSt; }
}
