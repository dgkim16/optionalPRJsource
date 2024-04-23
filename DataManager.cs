using UnityEngine;

public static class DataManager
{
    public static UserInfo userInfo;
    public static string userHash;
    public static string userID;
    public static string userName;
    public static CharStats[] userChars;
    public static CharStats[] enemyChars;
    public static AllSkills allSkills;
    public static Party[] allyParties = new Party[4];
    public static int currentParty = 0;
    public static Party enemyParty = new Party();
    public static bool debug = true;
    

    // allows access to UI elements from various scripts and scenes
    public static class UIHandler {
        public static BattleEnemyHpGUI battleEnemyHpGUI;
        public static BattleEnemyHpGUI enemyTargetGUI;
        public static BattleEnemyHpGUI allyTargetGUI;
        public static SkillButton skillButtonGrouper;
        public static SkillPointsManager skillPointsManager;
    }

    // reads json file and saves it to DataManager's static variables
    public static void readJson() {
        TextAsset textAsset = Resources.Load<TextAsset>("Data/Data");
        userInfo = JsonUtility.FromJson<UserInfo>(textAsset.text);
        userHash = userInfo.userHash;
        userID = userInfo.userID;
        userName = userInfo.userName;
        userChars = userInfo.userChars;
        enemyChars = userInfo.enemyChars;
    }

    public static void writeJson(string input) {
        //userInfo.userHash = input;
    }

    public static void saveJson(UserInfo userInfo) {
        string json = JsonUtility.ToJson(userInfo);
        System.IO.File.WriteAllText(Application.dataPath + "/Resources/Data/Data.json", json);
    }

    public static void ReadSkillJson() {
        TextAsset textAsset = Resources.Load<TextAsset>("Data/SkillData");
        allSkills = JsonUtility.FromJson<AllSkills>(textAsset.text);
        Debug.Log(allSkills);
    }

    public static Party GetCurrentParty() {
        return allyParties[currentParty];
    }
    
}
