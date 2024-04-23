using UnityEngine.UI;
using UnityEngine;

public interface CharacterInterface
{
    public string getName();
    public int addID();
    public int getID();
    public int getLevel();
    public void gainExp(int exp);
    public void checkLvUp();
    public float getSpeed();
    public void addSpeed(float speed);
    public int CompareTo(object obj, int mode);
    public float getHp();
    public void addHp(float hp);
    public float getMaxHp();
    public float getAtk();
    public float getDef();
    public float getShield();
    public Texture getTexture(int mode);
    public GameObject getCharModel();
    public int getType();
    public float getEnergy();
    public void addEnergy(float factor);
    public void resetEnergy();
    
    public SkillInterface getSkillUlt();
    public SkillInterface getSkillOne();
    public SkillInterface getSkillTwo();
    public SkillInterface[] getSkills();
    
    //public StatusInterface getStatus();
    //public CharacterTypeInterface getCharType();
    //public float getResistance(string ccName);
}

