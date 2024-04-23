using System;
[Serializable]
// structure of json file containing userInfo. stored as static object in DataManager
public class UserInfo
{
    public string userHash;
    public string userID;
    public string userName;
    public CharStats[] userChars;
    public CharStats[] enemyChars;
}
