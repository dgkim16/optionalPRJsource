using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BattleManager
{
    public static Battle battle;
    public static Battle storedBattle;
    // caster 는 Battle.cs에서 설정해줌
    public static CharacterBattle skillCaster;
    // skillTargets는 Skill.cs에서 설정해줌
    public static CharacterBattle skillMainTarget;
    public static CharacterBattle[] skillSubTargets;
    public static Transform originalTransf;
    public static SkillInterface skill;
    public static SkillPointsManager skillPointsManager;
    public static bool isUlt;

    public static void ExecuteBattle() {
        battle.canUpdateStatusPanel = true;
        battle.SetSFX();
        //Debug.Log("expecting response from \nCASTER:" + skillCaster.character.getName() + "_Ally:" + skillCaster.isPlayable  + "\n AFFECTED: " + skillMainTarget.character.getName() + "_Ally:" + skillMainTarget.isPlayable );
        DataManager.UIHandler.skillButtonGrouper.HideSkillButtons();
        battle.DisableTargets();
        originalTransf = skillCaster.charGO.transform;
        originalTransf.position = skillCaster.charGO.GetComponent<Animator>().rootPosition;
        originalTransf.rotation = skillCaster.charGO.GetComponent<Animator>().rootRotation;
        skillCaster.charGO.GetComponent<EventReciever>().isTurn = true;
        List<Transform> targets = new List<Transform>();
        targets.Add(skillMainTarget.charGO.transform);
        try {
        foreach(CharacterBattle target in skillSubTargets) {
            targets.Add(target.charGO.transform.Find("Armature").Find("Hips"));
        }
        } catch (Exception e) {}
        skillCaster.charGO.GetComponent<EventReciever>().AddTargets(targets);
        //Debug.Log(skillCaster.charGO.GetComponent<EventReciever>().targets[0].position);
        //Debug.Log(originalTransf.position);
        skillCaster.charGO.GetComponent<Animator>().SetBool("turn", true);
        skillCaster.charGO.GetComponent<Animator>().SetTrigger("execute");
        if(skillCaster.isPlayable) skillPointsManager.changePoints(skill.skillCost);
        ChangeCanLerpBar(false);
    }

    public static void DestroyObjects() {
        for(int i = 0; i < battle.destroyListTurnEnd.Count; i++) {
            GameObject.Destroy(battle.destroyListTurnEnd[i]);
        }
        battle.destroyListTurnEnd.Clear();
    }

    public static void ChangeCanLerpBar(bool b) {
        skillMainTarget.canLerpBar = b;
        skillCaster.canLerpBar = b;
        try {
        foreach(CharacterBattle charb in skillSubTargets)
            charb.canLerpBar = b;
        } catch (Exception e) {}

    }

    // called by event caller in caster's animation
    public static void ChangeHP(float factor, int scale) {
        Debug.Log("factor from skill is: " + factor);
        float f = 0.0f;
        if(skill.skillType.Equals("heal")) {
            f = factor * skillCaster.character.getAtk() * 0.25f;
            skillMainTarget.charGO.GetComponent<Animator>().SetTrigger("buffed");
        } else if(skill.skillType.Equals("damage")) {
            f = -factor * skillCaster.character.getAtk();
            if(scale < 1)
                skillMainTarget.charGO.GetComponent<Animator>().SetTrigger("damaged");
            else
                skillMainTarget.charGO.GetComponent<Animator>().SetTrigger("stunned");
            //skillMainTarget.Damage(-(factor * skillCaster.character.getAtk()))
        } else {
            Debug.Log("skill Type :" + skill.skillType + " not found.");
        }
        skillMainTarget.ChangeHP(f);
        if(skillSubTargets == null) return;
        try {
            foreach(CharacterBattle target in skillSubTargets) {
                if(skill.skillType.Equals("heal")) {
                    target.character.addHp(factor * skillCaster.character.getAtk());
                    target.charGO.GetComponent<Animator>().SetTrigger("buffed");
                } else if(skill.skillType.Equals("damage")) {
                    target.character.addHp(-(factor * skillCaster.character.getAtk()) / target.character.getDef());
                    if(scale < 1)
                        target.charGO.GetComponent<Animator>().SetTrigger("damaged");
                    else
                        target.charGO.GetComponent<Animator>().SetTrigger("stunned");
                }
            }
        } catch (Exception e) {
            // Debug.Log(e);
        }
    }

    // currently energy is altered here. 
    public static void AffectedBySkill(int scale) {
        skillMainTarget.charGO.GetComponent<Animator>().SetTrigger("affected");
        battle.PlaySFX();
        string skillTypeFactor = skill.skillID.Substring(skill.skillID.Length -1);
        float factor;
        // need separate function that computes skill factor. Function should be added to skill interface.
        if(skillTypeFactor.Equals("a")) factor = 10.0f;
        else if(skillTypeFactor.Equals("b")) factor = 20.0f;
        else if(skillTypeFactor.Equals("u")) factor = 30.0f;
        else {
            //Debug.Log("Error: Skill Type Factor not found." + skill.skillID + " " + skill.skillFactor + " " + skillTypeFactor);
            factor = 40f;
        }
        //Debug.Log("Energy before addition: " + skillCaster.character.getEnergy());
        if(!isUlt) {
            skillCaster.character.addEnergy(factor);
        }
        // Debug.Log("Energy Added: " + factor + " to " + skillCaster.character.getName() + ". Current Energy: " + skillCaster.character.getEnergy());
        ChangeHP(skill.skillFactor, scale);
        
        if(skillSubTargets == null) return;
        try {
            foreach(CharacterBattle target in skillSubTargets) {
                target.charGO.GetComponent<Animator>().SetTrigger("affected");
                //skillCaster.character.addEnergy(factor/5.0f);
                target.character.addEnergy(factor/5.0f);
            }
        } catch (Exception e) {
            Debug.Log(e);
        }
    }
    // use Animator.MatchTarget to move skill caster to skill target
    // Animator.applyRootMotion must be enabled for MatchTarget to take effect.
    // https://docs.unity3d.com/ScriptReference/Animator.MatchTarget.html
    // gets off the ground at 14.1% of animation clip. (0.141f)
    // land on its feet at 78% of animation clip. (0.78f)
    // animator.MatchTarget(jumpTarget.position, jumpTarget.rotation, AvatarTarget.LeftFoot, new MatchTargetWeightMask(Vector3.one, 1f), 0.141f, 0.78f);  
}
