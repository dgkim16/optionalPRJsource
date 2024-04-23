using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton
{
    public int selected;
    public SkillButtonUnit[] skillButtonUnits;
    public UltButtonUnit[] ultButtons;
    public Button ultActiveButton;

    
    public SkillButton(Button[] buttons) {
        this.skillButtonUnits = new SkillButtonUnit[2];
        this.skillButtonUnits[0] = new SkillButtonUnit(buttons[0], 0);
        this.skillButtonUnits[1] = new SkillButtonUnit(buttons[1], 1);
        this.skillButtonUnits[0].otherButton = this.skillButtonUnits[1];
        this.skillButtonUnits[1].otherButton = this.skillButtonUnits[0];
        this.selected = 0;
    }

    public class SkillButtonUnit {
        public Button button;
        public int index;
        public SkillInterface unitSkill;
        public SkillButtonUnit(Button button, int index) {
            this.button = button;
            this.index = index;
        }
        public SkillButtonUnit(){}
        public SkillButtonUnit otherButton;
    }

    public class UltButtonUnit : SkillButtonUnit {
        public CharacterBattle charBattle;
        public bool isFull;
        public bool isActivated;
        public UltButtonUnit(Button button, int index) : base(button, index) {
        }
        public UltButtonUnit(Button button, CharacterBattle charBattle){
            this.button = button;
            this.charBattle = charBattle;
            this.isFull = false;
            this.unitSkill = charBattle.character.getSkillUlt();
            this.isActivated = false;
        }
    }

    public void SetUpUltButtons(List<CharacterBattle> characters) {
        ultButtons = new UltButtonUnit[characters.Count];
        int counter = 0;
        foreach(CharacterBattle charBattle in characters) {
            Button ultButton = charBattle.statusPanel.transform.Find("ult").GetComponent<Button>();
            
            UltButtonUnit newUltUnit = new UltButtonUnit(ultButton, charBattle);
            ultButtons[counter] = newUltUnit;
            counter++;
            newUltUnit.button.onClick.AddListener( () => {
                UltSelected(newUltUnit);
            });
        }
        UpdateUltButton();
    }

    public void UpdateUltButton() {
        //Debug.Log("Upding Ult buttons");
        foreach(UltButtonUnit unit in ultButtons) {
            unit.button.GetComponent<Image>().material.SetFloat("_Energy", unit.charBattle.character.getEnergy());
            if(unit.charBattle.character.getEnergy() >= 100 && !unit.isActivated) {
                //Debug.Log("Ult button available!");
                unit.isFull = true;
                unit.button.interactable = true;
            }

            /*
            if(unit.charBattle.character.getEnergy() >= 100 && !unit.isFull && !unit.isActivated) {
                unit.isFull = true;
                unit.button.interactable = true;
                unit.button.onClick.AddListener( () => {
                    Debug.Log("Ult Button Clicked");
                    unit.isActivated = true;
                    unit.button.interactable = false;
                    UltSelected(unit);
                });
            }
            else {
                Debug.Log("Ult Button Not Clicked" + unit.charBattle.character.getEnergy() + " " + unit.isFull + " " + unit.isActivated);
                unit.button.onClick.RemoveAllListeners();
                unit.isFull = false;
            }
            */
        }

    }

    public void HideSkillButtons() {
        foreach(SkillButtonUnit unit in skillButtonUnits)
            FlipButtonGO(unit, false);
    }

    public void ChangeInteractableAll(bool isInteractable) {
        foreach(SkillButtonUnit unit in skillButtonUnits)
            unit.button.interactable = isInteractable;
        foreach(UltButtonUnit unit in ultButtons)
            unit.button.interactable = isInteractable;
    }

    public void SetUpSkillButton(SkillInterface[] skills) {
        selected = 0;
        ultActiveButton.gameObject.SetActive(false);
        BattleManager.skillCaster.charGO.GetComponent<Animator>().SetBool("isSkillOne", true);
        foreach (SkillButtonUnit unit in skillButtonUnits) {
            FlipButtonGO(unit, true);
            unit.unitSkill = skills[unit.index + 1];
            unit.button.GetComponent<Image>().material.SetTexture("_MainTex", Resources.Load<Texture>("Images/SkillIcons/Norm/" + skills[unit.index].skillID));
            if(unit.index == 0) {
                SelectSkill(unit, false);
                unit.button.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/SkillIcons/Norm/" + skills[1].skillID);
                //Debug.Log("Skill ID: " + skills[1].skillID);
            }
            else {
                DeselectSkill(unit);
                unit.button.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/SkillIcons/Norm/" + skills[2].skillID);
                //Debug.Log("Skill ID: " + skills[2].skillID);
            }
            unit.button.onClick.AddListener( () => {
                if(unit.index == selected)
                    ExecuteSkill(unit.unitSkill);
                else {
                    if(BattleManager.skillPointsManager.CanUseSkillPoints(unit.unitSkill.skillCost)) {
                        selected = unit.index;
                        SelectSkill(unit, true);
                        DeselectSkill(unit.otherButton);
                    }
                }
            });
        }
        UpdateSkillButtonBySkillPoints();
    }

    private void UltSelected(UltButtonUnit unit) {
        Debug.Log("Ult selected!");
        CharacterBattle ultChar = new CharacterBattle(unit.charBattle, true);
        BattleManager.battle.AddToTurnQueue(ultChar);
        unit.isActivated = true;
        unit.button.interactable = false;
    }

    public void SetUpUlt(SkillInterface skill, CharacterBattle characterBattle) {
        foreach(SkillButtonUnit skillUnit in skillButtonUnits)
            FlipButtonGO(skillUnit, false);
        //ultActiveButton.GetComponent<Image>().material = unit.button.GetComponent<Image>().material;
        ultActiveButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/SkillIcons/Ults/" + skill);
        ultActiveButton.gameObject.SetActive(true);
        ultActiveButton.onClick.RemoveAllListeners();
        ultActiveButton.interactable = true;
        
        ultActiveButton.onClick.AddListener( () => {
            ExecuteSkill(skill);
            ultActiveButton.interactable = false;          
            characterBattle.character.resetEnergy();
            foreach(UltButtonUnit unit in ultButtons) {
                if(unit.charBattle == characterBattle) {
                    unit.isActivated = false;
                }
            }
            UpdateUltButton();
        });
    }

    private void SelectSkill(SkillButtonUnit unit, bool isSetup) {
        // Skill Selected
        // change button material to selected
        unit.button.GetComponent<Image>().material.SetFloat("_isFull", 1);
        BattleManager.skillCaster.charGO.GetComponent<Animator>().SetBool("isSkillOne", unit.index == 0);
        if(isSetup) {
            if(unit.unitSkill.skillType.Equals(unit.otherButton.unitSkill.skillType)) {
                BattleManager.battle.SetTarget(BattleManager.skillCaster, unit.unitSkill, BattleManager.skillMainTarget);
            } else {
                BattleManager.battle.SetTarget(BattleManager.skillCaster, unit.unitSkill, null);
            }
        }
        else {
            BattleManager.battle.SetTarget(BattleManager.skillCaster, unit.unitSkill, BattleManager.skillMainTarget);
        }
        BattleManager.skill = unit.unitSkill;
        unit.button.gameObject.GetComponent<RectTransform>().localScale = new Vector3(1.3f, 1.3f, 1.3f);
        //Debug.Log("Skill Selected: " + unit.unitSkill.skillID);
        BattleManager.skillPointsManager.showsChangeOnMove(unit.unitSkill.skillCost);
        //Debug.Log(selected + " Skill Selected");
    }

    private void DeselectSkill(SkillButtonUnit unit) {
        unit.button.gameObject.GetComponent<RectTransform>().localScale = new Vector3(.8f, .8f, .8f);
        //Debug.Log("Skill Deselected");
    }

    public void UpdateSkillButtonBySkillPoints() {
        foreach(SkillButtonUnit unit in skillButtonUnits) {
            if(BattleManager.skillPointsManager.CanUseSkillPoints(unit.unitSkill.skillCost)) {
                unit.button.GetComponent<Image>().material.SetFloat("_isFull", 1);
                unit.button.interactable = true;
            }
            else {
                unit.button.GetComponent<Image>().material.SetFloat("_isFull", 0);
                unit.button.interactable = false;
            }
        }
    }

    private void FlipButtonGO(SkillButtonUnit unit, bool isOn) {
        unit.button.gameObject.SetActive(isOn);
    }

    public void ResetSkillButton() {
        foreach(SkillButtonUnit unit in skillButtonUnits)
            unit.button.onClick.RemoveAllListeners();
        ultActiveButton.onClick.RemoveAllListeners();
    }

    void ExecuteSkill(SkillInterface skill) {
        //Debug.Log("Skill Executing");
        ResetSkillButton();
        BattleManager.ExecuteBattle();
        //Debug.Log("Skill executed");
    }
    

}
