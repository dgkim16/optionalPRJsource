using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillPointsManager
{
    public SkillPointsManager(int points, int maxPoints, GameObject skillPointsUIparent)
    {
        this.skillPoints = points;
        this.maxSkillPoints = maxPoints;
        this.skillPointsUI = InstantiateSkillPointsUI(skillPointsUIparent.transform, null);
        changePoints(0);
    }

    public SkillPointsManager(int points, int maxPoints, GameObject skillPointsUIparent, GameObject prefab)
    {
        this.skillPoints = points;
        this.maxSkillPoints = maxPoints;
        this.skillPointsUI = InstantiateSkillPointsUI(skillPointsUIparent.transform, prefab);
        changePoints(0);
    }

    private int skillPoints = 3;
    private int maxSkillPoints = 5;
    private GameObject[] skillPointsUI;

    public GameObject[] InstantiateSkillPointsUI(Transform parent, GameObject prefab)
    {
        if(prefab == null)
            prefab = Resources.Load<GameObject>("Placeholders/SkillPointUI");
        skillPointsUI = new GameObject[maxSkillPoints];
        for(int i = 0; i < maxSkillPoints; i++)
        {
            skillPointsUI[i] = GameObject.Instantiate(prefab, parent);
            skillPointsUI[i].name = "SkillPoint" + (i+1);
        }
        return skillPointsUI;
    }

    public int getPoints()
    {
        return skillPoints;
    }

    public bool CanUseSkillPoints(int points) {
        return getPoints() >= points;
    }

    // NEGATIVE COST MEANS GAINING POINTS
    public void showsChangeOnMove(int points) {
        //Debug.Log("may change with points: " + points);
        if(!CanUseSkillPoints(points)) return;
        // WILL USE POINTS TO EXECUTE
        if(points > 0) {
            for(int i = 1; i <= maxSkillPoints; i++) {
                Animator animator = skillPointsUI[i-1].GetComponent<Animator>();
                if(i <= skillPoints && i > skillPoints - points)
                    animator.SetTrigger("willempty");
                else if ( i > skillPoints )
                    animator.SetTrigger("emptied");
                else
                    animator.SetTrigger("filled");
            }
        }
        // WILL RECOVER POINTS AFTER EXECUTE
        else {
            for(int i = 1; i <= maxSkillPoints; i++) {
                Animator animator = skillPointsUI[i-1].GetComponent<Animator>();
                if(i > skillPoints && i <= skillPoints - points)
                    animator.SetTrigger("willfill");
                else if ( i > skillPoints - points )
                    animator.SetTrigger("emptied");
                else
                    animator.SetTrigger("filled");
            }
        }
    }

    public void changePoints(int points)
    {
        //Debug.Log("changing with points: " + points);
        int newPoints = Mathf.Clamp(skillPoints - points, 0, maxSkillPoints);
        for(int i = 1; i <= maxSkillPoints; i++) {
            Animator animator = skillPointsUI[i-1].GetComponent<Animator>();
            // USE POINTS
            if(points > 0) {
                if(i <= newPoints)
                    animator.SetTrigger("filled");
                else if ( i > newPoints && i <= skillPoints)
                    animator.SetTrigger("empty");
                else
                    animator.SetTrigger("emptied");
            }
            else {
                if(i > newPoints)
                    animator.SetTrigger("emptied");
                else if ( i <= newPoints && i > skillPoints)
                    animator.SetTrigger("fill");
                else
                    animator.SetTrigger("filled");
            }
        }
        skillPoints = newPoints;
    }

}
