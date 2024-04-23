using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 배틀에서 GUI를 따로 관리하는 클래스들을 하나로 묶어야 하나?
// (현재는 안되어있음. CharacterBattle.cs에서 indicator들을 생성하고, Battle.cs에서 이를 관리중임)
// Using a tree structure was a terrible idea.

public class BattleEnemyHpGUI
{
    Camera mCamera;
    public RectTransformGrouper root;

    public BattleEnemyHpGUI(Camera mCamera) {
        this.mCamera = mCamera;
    }
    
    public class RectTransformGrouper {
        public RectTransform rt;
        public Transform target;
        public RectTransformGrouper succ;
        public RectTransformGrouper pred;
        public float y = 0f;
        public RectTransformGrouper(RectTransform rt, RectTransformGrouper pred, Transform tgt) {
            this.rt = rt;
            this.pred = pred;
            this.target = tgt;
        }
        public void setSucc(RectTransformGrouper succ) {
            this.succ = succ;
        }
    }

    public int selectedIndex = -1;
    public List<TargetButton> targetButtons = new List<TargetButton>();
    public class TargetButton {
        public GameObject buttonGO;
        public int index;
        public CharacterBattle target;
        public void AddEvent(CharacterBattle target) {
            this.target = target;
            buttonGO.GetComponent<Button>().onClick.AddListener(() => {
                if(target.character.getHp() <= 0) return;
                else if(!target.isPlayable) {
                    if(DataManager.UIHandler.enemyTargetGUI.selectedIndex == this.index) {
                        DataManager.UIHandler.skillButtonGrouper.ResetSkillButton();
                        BattleManager.ExecuteBattle();
                    }
                    else {
                        DataManager.UIHandler.enemyTargetGUI.selectedIndex = this.index;
                        BattleManager.battle.SetTarget(BattleManager.skillCaster, BattleManager.skill, target);
                    }
                }
                else {
                    if(DataManager.UIHandler.allyTargetGUI.selectedIndex == this.index) {
                        DataManager.UIHandler.skillButtonGrouper.ResetSkillButton();
                        BattleManager.ExecuteBattle();
                    }
                    else {
                        DataManager.UIHandler.allyTargetGUI.selectedIndex = this.index;
                        BattleManager.battle.SetTarget(BattleManager.skillCaster, BattleManager.skill, target);
                    }
                }
                BattleManager.battle.PlayButtonSound();
            });
            
        }

        public void UpdateSelected() {
            if(this.target.character.getHp() <= 0) {
                buttonGO.SetActive(false);
                return;
            }
            buttonGO.SetActive(true);
            if(!target.isPlayable) {
                if(DataManager.UIHandler.enemyTargetGUI.selectedIndex == this.index) {
                    //buttonGO.GetComponent<Image>().color = new Color(1f, 0.2f, 0.2f, 0.5f);
                    buttonGO.transform.GetChild(0).GetComponent<Animator>().SetFloat("spd",1f);
                    buttonGO.transform.GetChild(0).transform.localScale = new Vector3(1f, 1f, 1f);
                } else {
                    //buttonGO.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.1f);
                    buttonGO.transform.GetChild(0).GetComponent<Animator>().SetFloat("spd",0f);
                    buttonGO.transform.GetChild(0).GetComponent<Animator>().Rebind();
                    buttonGO.transform.GetChild(0).GetComponent<Animator>().Update(0f);
                    buttonGO.transform.GetChild(0).transform.localScale = new Vector3(.3f, .3f, .3f);
                }
            }
            else {
                if(DataManager.UIHandler.allyTargetGUI.selectedIndex == this.index) {
                    //buttonGO.GetComponent<Image>().color = new Color(1f, 0.2f, 0.2f, 0.5f);
                    buttonGO.transform.GetChild(0).GetComponent<Animator>().SetFloat("spd",1f);
                    buttonGO.transform.GetChild(0).transform.localScale = new Vector3(1f, 1f, 1f);
                } else {
                    //buttonGO.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.1f);
                    buttonGO.transform.GetChild(0).GetComponent<Animator>().SetFloat("spd",0f);
                    buttonGO.transform.GetChild(0).GetComponent<Animator>().Rebind();
                    buttonGO.transform.GetChild(0).GetComponent<Animator>().Update(0f);
                    buttonGO.transform.GetChild(0).transform.localScale = new Vector3(.3f, .3f, .3f);
                }
            }
        }

        public void Disable() {
            buttonGO.SetActive(false);
        }
    }

    public void SetUpNewRectTransformGrouper(RectTransform nrt, Transform tgt) {
        RectTransformGrouper newrtg = new RectTransformGrouper(nrt, GetLeaf(), tgt);
        if(root == null)
            root = newrtg;
        else
            newrtg.pred.setSucc(newrtg);
    }

    public void ChangeInteractableAll(bool isInteractable) {
        targetButtons.ForEach(tb => tb.buttonGO.GetComponent<Button>().interactable = isInteractable);
    }

    public void SetUpNewRectTransformGrouper(Transform tgt, Transform tgt2, CharacterBattle charBattle) {
        GameObject targetButton = Resources.Load<GameObject>("Placeholders/TargetButtonPlaceholder");
        RectTransform nrt = GameObject.Instantiate(targetButton, GameObject.Find("BattleCanvas").transform.Find("TargetGroupHolder")).GetComponent<RectTransform>();
        //RectTransform nrt = GameObject.Instantiate(Resources.Load<GameObject>("Placeholders/TargetButtonPlaceholder"), GameObject.Find("BattleCanvas").transform.Find("TargetGroupHolder")).GetComponent<RectTransform>();
        
        TargetButton tb = new TargetButton();
        tb.buttonGO = nrt.gameObject;
        tb.index = charBattle.count;
        targetButtons.Add(tb);
        //charBattle.SetUIPos(nrt);
        tb.AddEvent(charBattle);

    
        RectTransformGrouper newrtg = new RectTransformGrouper(nrt, GetLeaf(), tgt);
        if(root == null)
            root = newrtg;
        else
            newrtg.pred.setSucc(newrtg);

        GameObject affectIndicatorPanel = Resources.Load<GameObject>("Placeholders/AffectedIndicatorPanelPlaceholder");
        RectTransform nrt2 = GameObject.Instantiate(affectIndicatorPanel, GameObject.Find("BattleCanvas").transform.Find("IndicatorGroupHolder")).GetComponent<RectTransform>();
        charBattle.SetUIPos(nrt2);
        RectTransformGrouper newrtg2 = new RectTransformGrouper(nrt2, GetLeaf(), tgt2);
        float y = tgt.position.y;
        newrtg2.y = y;
        newrtg2.pred.setSucc(newrtg2);
        
    }

    RectTransformGrouper GetLeaf() {
        if(root == null)
            return null;
        RectTransformGrouper temp = root;
        while(temp.succ != null) {
            temp = temp.succ;
        }
        return temp;
    }

    public void RemoveRectTransformGrouper(RectTransformGrouper rtg) {

        if(rtg.pred == null) {
            root = rtg.succ;
            if(root != null)
                root.pred = null;
        } else {
            rtg.pred.setSucc(rtg.succ);
        }
        if(rtg.succ != null) {
            rtg.succ.pred = rtg.pred;
        }
    }

    public void UpdateGUICam(){
        Vector3 camForward = mCamera.transform.forward;
        Vector3 camPos = mCamera.transform.position + camForward;
 

        RectTransformGrouper temp = root;
        while(temp != null) {
            float dist = Vector3.Dot(temp.target.position - camPos + new Vector3(0,temp.y,0), camForward);
            Vector3 targetPos = dist < 0f ? temp.target.position - camForward * dist : temp.target.position;
            targetPos += new Vector3(0,temp.y,0);
            temp.rt.position = RectTransformUtility.WorldToScreenPoint (mCamera, targetPos);
            temp = temp.succ;
        }
    }
}
