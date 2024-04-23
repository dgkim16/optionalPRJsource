using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventReciever : MonoBehaviour
{
    public Animator animator;
    public Transform returnTarget;
    public List<Transform> targets;
    public bool canMatchTarget = true;
    public bool isTurn = false;

    void Start() {
        animator = this.GetComponent<Animator>();
        Vector3 pos = animator.rootPosition;
        Quaternion rot = animator.rootRotation;
        returnTarget = new GameObject().transform;
        returnTarget.position = pos;
        returnTarget.rotation = rot;
    }

    public void MatchTargetOriginal(int mode) {
        if(mode == 0)
            animator.MatchTarget(returnTarget.position, returnTarget.rotation, AvatarTarget.Root, new MatchTargetWeightMask(Vector3.one, 1f), 0.394f, 0.590f);
        else if (mode == 1)
            animator.MatchTarget(returnTarget.position, returnTarget.rotation, AvatarTarget.Root, new MatchTargetWeightMask(Vector3.one, 1f), 0.247f, 0.746f);
        else
            animator.MatchTarget(returnTarget.position, returnTarget.rotation, AvatarTarget.Root, new MatchTargetWeightMask(Vector3.one, 1f), 0.1f, 0.3f);
    }

    public void AddTargets(List<Transform> targets) {
        this.targets = targets;
    }

    public void MatchTargetSkill(string toParse) {
        if(targets == null) return;
        int index = (int)float.Parse(toParse.Split(',')[0]);
        float start = float.Parse(toParse.Split(',')[1]);
        float end = float.Parse(toParse.Split(',')[2]);

        Vector3 direction = targets[index].position - animator.rootPosition;
        Vector3 unitDirection = direction.normalized;
        Quaternion lookRotation = new Quaternion();
        lookRotation = returnTarget.rotation;
        lookRotation.y += .45f;
        if(canMatchTarget) {
            animator.MatchTarget(targets[index].position - unitDirection * 2.0f, lookRotation , AvatarTarget.Root, new MatchTargetWeightMask(Vector3.one, 1f), start, end);
            canMatchTarget = false;
        }
    }

    public void EnableMatchTarget() {
        canMatchTarget = true;
    }

    public void ContactEffect(int scale) {
        Debug.Log("Contact Effect called by " + this.gameObject.name);
        BattleManager.AffectedBySkill(scale);
    }

    // must be called by event caller in caster's final animation (return to original position)
    public void TurnOver() {
        targets = null;
        //if(!isTurn) return;
        BattleManager.battle.TurnOver(this.gameObject.name + "_" + this.gameObject.transform.GetSiblingIndex() + "_" + this.gameObject.transform.parent.parent.parent.name);
        isTurn = false;
    }
}
