using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Character : MyComparableInterface, CharacterInterface, IComparable<Character>
{
    private SkillInterface[] charSkills;
    public CharStats charSt;
    public Texture[] charTexture;
    public GameObject charModel;
    public float energy;
    public Character(CharStats stats) {
        this.charSt = stats;
        this.charTexture = new Texture[2];
        charSkills = new Skill[3];
        charSkills[0] = new Skill(stats.charSkills[0]);
        charSkills[1] = new Skill(stats.charSkills[1], 0);
        charSkills[2] = new Skill(stats.charSkills[1], 1);
        this.charModel = (GameObject)Resources.Load("Prefabs/Characters/" + this.charSt.charName, typeof(GameObject));
        this.charTexture[1] = Resources.Load<Texture>("Images/Characters/" + this.charSt.charName + "_nobg");
        this.charTexture[0] = Resources.Load<Texture>("Images/Characters/" + this.charSt.charName);
    }
    public GameObject getCharModel() {
        return this.charModel;
    }

    public string getName() { return this.charSt.charName; }
    public int getLevel() { return this.charSt.charLevel; }
    public void gainExp(int exp) { 
        this.charSt.levelExp += exp; 
        this.checkLvUp(); 
    }
    public void checkLvUp() { 
        // load the exp table using charRank, charLevel, levelExp
        // do calculation, then if lv up, increment level, set levelExp to 0
    }
    public Texture getTexture(int mode) { return this.charTexture[mode]; }

    
    public bool getIsMember() { return this.charSt.isMember; }
    public int getID() {return this.charSt.charID;  }
    public int addID() { return this.charSt.charID++; }
    public int getRank() { return this.charSt.charRank; }
    public float getSpeed() { return this.charSt.speed; }
    public void addSpeed(float factor) { this.charSt.speed += factor; }
    public float getAtk() { return this.charSt.atk; }
    public void addAtk(float factor) { this.charSt.atk += factor; }
    public float getDef() { return this.charSt.def; }
    public void addDef(float factor) { this.charSt.def += factor; }
    public float getMaxHp() { return this.charSt.maxHP; }
    public void addMaxHp(float factor) { this.charSt.maxHP += factor; }
    public float getHp() { return this.charSt.currHP; }
    public void addHp(float factor) { this.charSt.currHP += factor; }
    public float getShield() { return this.charSt.shield; }
    public void addShield(float factor) { 
        this.charSt.shield += factor; 
    }
    public int getType() { return this.charSt.type; }
    public float getEnergy() { return this.energy; }
    public void addEnergy(float factor) { this.energy = Mathf.Min(100.0f, this.energy + factor); }
    public void resetEnergy() { this.energy = 0f; }

    // not part of the interface. Left it here just in case
    public void changeHP(float factor) { 
        this.charSt.currHP += factor; 
        if(this.charSt.currHP > this.charSt.maxHP)
            this.charSt.currHP = this.charSt.maxHP;
    }

    public void checkCharStatus() {

    }

    public SkillInterface getSkillUlt() {
        return this.charSkills[0];
    }

    public SkillInterface getSkillOne() {
        return this.charSkills[1];
    }
    public SkillInterface getSkillTwo() {
        return this.charSkills[2];
    }
    public SkillInterface[] getSkills() {
        return this.charSkills;
    }

    /// <summary>
    /// Method <c>CompareTo</c> takes in an object and a mode, and compares the current Character object to the object passed in.
    /// </summary>
    public int CompareTo(Character other, int mode) 
    {
        switch (mode) {
            case 0:
                //Debug.Log("Comparing by name");
                return this.charSt.charName.CompareTo(other.charSt.charName);
            case 1:
                //Debug.Log("Comparing by ID");
                if(this.getID() == other.getID())
                    return 0;
                else if (this.getID() > other.getID())
                    return 1;
                else if (this.getID() < other.getID())
                    return -1;
                else
                    throw new ArgumentException("Character ID is not a number");
            case 2:
                //Debug.Log("Comparing by Level");
                if(this.getLevel() == other.getLevel())
                    return this.CompareTo(other, 3);
                else if (this.getLevel() > other.getLevel())
                    return 1;
                else if (this.getLevel() < other.getLevel())
                    return -1;
                else
                    throw new ArgumentException("Character Level is not a number");
            case 3:
                //Debug.Log("Comparing by Rank");
                if(this.getRank() == other.getRank())
                    return this.CompareTo(other, 1);
                else if (this.getRank() > other.getRank())
                    return 1;
                else if (this.getRank() < other.getRank())
                    return -1;
                else
                    throw new ArgumentException("Character Rank is not a number");
            default:
                //Debug.Log("Comparing by default (name)");
                return this.CompareTo(other, 1);
        }
    }

    /// <summary>
    /// Method <c>CompareTo</c> takes in an object, and calls the other CompareTo method with a default mode of 1.
    /// </summary>
    public int CompareTo(object obj)
    {
        if(obj == null)
            throw new ArgumentException("Object is null");
        if(!(obj is Character))
            throw new ArgumentException("Object is not a Character");
        return this.CompareTo((Character)obj, 1);
    }

    public int CompareTo(Character other)
    {
        if(other == null)
            throw new ArgumentException("Object is null");
        return this.CompareTo(other, 1);
    }

    public int CompareTo(object obj, int mode)
    {
        if(obj == null)
            throw new ArgumentException("Object is null");
        if(!(obj is Character))
            throw new ArgumentException("Object is not a Character");
        return this.CompareTo((Character)obj, mode);
    }

}
