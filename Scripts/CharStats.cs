using System;
[Serializable]
public class CharStats
{
    public bool isMember;
    public string charName;
    public int[] charSkills;
    public int charID;
    public int charLevel;
    public float levelExp;
    public int charRank;
    public float speed;
    public float atk;
    public float def;
    public float maxHP;
    public float currHP;
    public float shield;
    public int type;
    public CharStats Clone()
    {
        return new CharStats
        {
            isMember = isMember,
            charName = charName,
            charSkills = charSkills,
            charID = charID,
            charLevel = charLevel,
            levelExp = levelExp,
            charRank = charRank,
            speed = speed,
            atk = atk,
            def = def,
            maxHP = maxHP,
            currHP = currHP,
            shield = shield,
            type = type
        };
    }
}
