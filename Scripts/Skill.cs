using System;
using System.Collections.Generic;
using UnityEngine;

// 모든 스킬을 담은 클래스 AllSkills의 인스턴스와 고유 id를 받고 AllSkills의 인스턴스에서 고유id에 해당하는 스킬을 찾아서 Skill 클래스의 인스턴스를 생성한다.
// AllSkills와 Skill 클래스를 통합하는 방법을 잘 모르겠어서 일단 이렇게 나눠놨다. 추후에 최적화 과정에서 수정할 수도 있다.

[Serializable]
public class Skill : SkillInterface
{
    public string skillID;
    public string skillName;
    public string skillType;
    public float skillFactor;
    public int skillCost;
    public int skillRange;
    public Animation skillAnimation;
    public Texture skillIcon;
    public object target;


    public Skill(int id) {
        this.skillID = DataManager.allSkills.ults[id].skillID;
        this.skillName = DataManager.allSkills.ults[id].skillName;
        this.skillType = DataManager.allSkills.ults[id].skillType;
        this.skillFactor = DataManager.allSkills.ults[id].skillFactor;
        this.skillAnimation = Resources.Load<Animation>("Animations/" + skillID);
        this.skillIcon = Resources.Load<Texture>("Images/SkillIcons/Ult/" + skillID);
        this.skillRange = 1; // fixed for now
        this.target = new object();
        this.skillCost = 0;
    }

    public Skill(int id, int skillNumb) {
        this.skillID = DataManager.allSkills.normals[id].normgroup[skillNumb].skillID;
        this.skillName = DataManager.allSkills.normals[id].normgroup[skillNumb].skillName;
        this.skillType = DataManager.allSkills.normals[id].normgroup[skillNumb].skillType;
        this.skillFactor = DataManager.allSkills.normals[id].normgroup[skillNumb].skillFactor;
        this.skillAnimation = Resources.Load<Animation>("Animations/" + skillID);
        this.skillIcon = Resources.Load<Texture>("Images/SkillIcons/Norm/" + skillID);
        this.skillRange = 1; // fixed for now
        this.target = new object();
        this.skillCost = this.skillID[this.skillID.Length-1] == 'a' ? -1 : 1;
    }

    public void ExecuteSkill() {
        BattleManager.ExecuteBattle();
    }

    // 이건 execute skill 안에 들어가게 generic 형식으로 위 코드를 수정해야함.
    public void EnhanceSkill() {
        Debug.Log("Skill Enhanced");
    }



    string SkillInterface.skillID { get => skillID; set => skillID = value; }
    string SkillInterface.skillName { get => skillName; set => skillName = value; }
    string SkillInterface.skillType { get => skillType; set => skillType = value; }
    float SkillInterface.skillFactor { get => skillFactor; set => skillFactor = value; }
    object SkillInterface.target { get => target; set => target = value; }
    Animation SkillInterface.skillAnimation { get => skillAnimation; set => skillAnimation = value; }
    Texture SkillInterface.skillIcon { get => skillIcon; set => skillIcon = value; }
    int SkillInterface.skillRange { get => skillRange; set => skillRange = value; }
    int SkillInterface.skillCost { get => skillCost; set => skillCost = value; }
}
