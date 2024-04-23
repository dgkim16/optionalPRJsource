
public interface SkillInterface 
{
    public string skillID { get; set; }
    public string skillName { get; set; }
    public string skillType { get; set; }
    public float skillFactor { get; set; } // factor is the amount of damage or healing. uses integer to make it easier to calculate
    public int skillCost { get; set; }
    public object target { get; set; }
    public UnityEngine.Animation skillAnimation { get; set; }
    public UnityEngine.Texture skillIcon { get; set; }
    public void ExecuteSkill();
    public void EnhanceSkill();
    public int skillRange { get; set; }
}

