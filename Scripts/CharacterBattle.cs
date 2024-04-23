using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using TMPro;

public class CharacterBattle : MyComparableInterface{
    // 파티 버프가 없을 경우.
    // When there is no party buff
    public CharacterBattle(int count, CharacterInterface character, GameObject turnPanel, GameObject turnPanelParent, GameObject statusPanelParent, bool playable) {
        this.count = count;
        this.character = character;
        this.basespd = (int)character.getSpeed();
        this.spd = (int)(basespd * (1) + 0);
        this.bav = (float)(10000f / this.spd);
        this.turnIndicator = GameObject.Instantiate(Resources.Load<GameObject>("Placeholders/CharTurnSample"), turnPanelParent.transform, false);
        turnIndicator.transform.Find("CharImgSlot").Find("CharImg").GetComponent<UnityEngine.UI.RawImage>().texture = character.getTexture(1);
        this.charMat = Resources.Load<Material>("Materials/CharBattle/"+playable.ToString()+"/FadeCharBattle_"+(count+1).ToString());
        this.charMat.mainTexture = character.getTexture(1);
        this.statusPanel = GameObject.Instantiate(Resources.Load<GameObject>("Placeholders/CharStatusSample_"+playable.ToString()), statusPanelParent.transform, false);
        this.statusPanel.transform.Find("CharImg").GetComponent<UnityEngine.UI.RawImage>().material = this.charMat;
        this.statusPanel.name = character.getName() + " Status Panel";
        this.isPlayable = playable;
        this.lerperBar = statusPanel.transform.Find("HpShield").Find("ChangeHP slider").gameObject;
        this.insantBar = statusPanel.transform.Find("HpShield").Find("HP slider").gameObject;
    }

    // 파티 버프가 있을 경우. 미구현.
    // When there is a party buff. Not implemented yet.
    public CharacterBattle(int count, PartyBuffInterface buff, CharacterInterface character, GameObject turnPanel,GameObject turnPanelParent, GameObject statusPanelParent, bool playable) {
        this.count = count;
        this.character = character;
        this.basespd = (int)character.getSpeed();
        this.buff = buff;
        this.spd = (int)(basespd * (1 + this.buff.getPerSPD()) + this.buff.getFlatSPD());
        this.bav = (float)(10000f / this.spd);
        this.turnIndicator = GameObject.Instantiate(Resources.Load<GameObject>("Placeholders/CharTurnSample"), turnPanelParent.transform, false);
        turnIndicator.transform.Find("CharImgSlot").Find("CharImg").GetComponent<UnityEngine.UI.RawImage>().texture = character.getTexture(1);
        this.charMat = Resources.Load<Material>("Materials/CharBattle/"+playable.ToString()+"/FadeCharBattle_"+(count+1).ToString());
        this.charMat.mainTexture = character.getTexture(1);
        this.statusPanel = GameObject.Instantiate(Resources.Load<GameObject>("Placeholders/CharStatusSample_"+playable.ToString()), statusPanelParent.transform, false);
        this.statusPanel.transform.Find("CharImg").GetComponent<UnityEngine.UI.RawImage>().material.mainTexture = character.getTexture(1);
        this.statusPanel.name = character.getName() + " Status Panel";
        this.isPlayable = playable;
        this.lerperBar = statusPanel.transform.Find("HpShield").Find("ChangeHP slider").gameObject;
        this.insantBar = statusPanel.transform.Find("HpShield").Find("HP slider").gameObject;
    }

    // I forgot what this was for
    public CharacterBattle(bool isPlaceholder, PartyBuffInterface buff, CharacterInterface character, GameObject turnPanel,GameObject turnPanelParent, GameObject statusPanelParent, bool playable) {
        this.character = character;
        this.basespd = (int)character.getSpeed();
        this.buff = buff;
        this.spd = (int)(basespd * (1 + this.buff.getPerSPD()) + this.buff.getFlatSPD());
        this.bav = (int)(10000f / this.spd);
        this.turnIndicator = GameObject.Instantiate(Resources.Load<GameObject>("Placeholders/CharTurnSample"), turnPanelParent.transform, false);
        turnIndicator.transform.GetChild(1).transform.GetChild(0).GetComponent<UnityEngine.UI.RawImage>().texture = character.getTexture(1);
        this.statusPanel = statusPanelParent.transform.Find(character.getName() + " Status Panel").gameObject;
        this.isPlayable = playable;
        this.lerperBar = statusPanel.transform.Find("HpShield").Find("ChangeHP slider").gameObject;
        this.insantBar = statusPanel.transform.Find("HpShield").Find("HP slider").gameObject;
    }

    // 복제되었을 경우, 원본의 데이터를 일부 그대로 계승한다
    // If this is a clone, inherit some data from the original
    public CharacterBattle(CharacterBattle toClone) {
        this.reference = toClone;
        this.count = toClone.count;
        this.character = toClone.character;
        this.basespd = toClone.basespd;
        this.buff = toClone.buff;
        if(this.buff != null)
            this.spd = (int)(basespd * (1 + this.buff.getPerSPD()) + this.buff.getFlatSPD());
        else
            this.spd = toClone.spd;
        this.bav = (int)(10000f / this.spd);
        this.charMat = toClone.charMat;
        this.turnIndicator = GameObject.Instantiate(Resources.Load<GameObject>("Placeholders/CharTurnSample"), toClone.turnIndicator.transform.parent, false);
        //this.turnIndicator = GameObject.Instantiate(toClone.turnIndicator, toClone.turnIndicator.transform.parent, false);
        turnIndicator.transform.Find("CharImgSlot").Find("CharImg").GetComponent<UnityEngine.UI.RawImage>().texture = character.getTexture(1);
        
        this.statusPanel = toClone.statusPanel;
        this.isPlayable = toClone.isPlayable;
        this.charGO = toClone.charGO;
        this.lerperBar = toClone.lerperBar;
        this.insantBar = toClone.insantBar;
        SetTurnPanelColor();
    }

    public CharacterBattle(CharacterBattle toClone, bool isUlt) {
        this.reference = toClone;
        this.count = toClone.count;
        this.character = toClone.character;
        this.basespd = toClone.basespd;
        this.buff = toClone.buff;
        if(this.buff != null)
            this.spd = (int)(basespd * (1 + this.buff.getPerSPD()) + this.buff.getFlatSPD());
        else
            this.spd = toClone.spd;
        this.bav = (int)(10000f / this.spd);
        this.charMat = toClone.charMat;
        this.turnIndicator = GameObject.Instantiate(Resources.Load<GameObject>("Placeholders/CharTurnSample"), toClone.turnIndicator.transform.parent, false);
        //this.turnIndicator = GameObject.Instantiate(toClone.turnIndicator, toClone.turnIndicator.transform.parent, false);
        turnIndicator.transform.Find("CharImgSlot").Find("CharImg").GetComponent<UnityEngine.UI.RawImage>().texture = character.getTexture(1);
        this.statusPanel = toClone.statusPanel;
        this.isPlayable = toClone.isPlayable;
        this.charGO = toClone.charGO;
        this.isUlt = isUlt;
        this.lerperBar = toClone.lerperBar;
        this.insantBar = toClone.insantBar;
        SetTurnPanelColor();
    }

    // party buff
    public PartyBuffInterface buff;
    public CharacterInterface character;
    public CharacterBattle reference;
    public bool isUlt;
    // SPD = Base SPD x (1+SPD%) + Flat SPD
    // 최종 속도 = 기초 속도 * (1+속도%) + 깡 속도
    // Base Action Value = 10000/SPD
    public int count;
    public float basespd;
    public int spd;
    public float bav;

    // agro = 기본 어그로 x (1 + 어그로계수)
    // agro = defaultAggro x (1 + aggroFactor)
    // 피격 확률 = 캐릭터 어그로 / 팀 총 어그로
    // Hit chance = Character Aggro / Total Team Aggro
    public float aggro;
    public float aggroFactor = 0f;
    public float Aggro {
        get { 
            switch (character.getType())
            {
                case 1: // 수렵
                    aggro = 75f;
                    break;
                case 2: // 지식
                    aggro = 75f;
                    break;
                case 3: // 화합
                    aggro = 100f;
                    break;
                case 4: // 공허
                    aggro = 100f;
                    break;
                case 5: //풍요
                    aggro = 100f;
                    break;
                case 6: //파멸
                    aggro = 125f;
                    break;
                case 7: //보존
                    aggro = 150f;
                    break;
                default :
                    aggro = 100f;
                    break;
            }
            return aggro * (1 + aggroFactor); 
        }
        set { aggro = value; }
    }


    // instantiated characterModel
    public GameObject charGO;
    // camera group for this character
    public GameObject camGroup;
    // material for character panel image & turn indicator
    public Material charMat;
    // material for ult
    public Material ultMat;
    // turnIndicator is instantiated from a prefab
    public GameObject turnIndicator;
    // status panel shows HP, Shield, Buffs, Ult
    public GameObject statusPanel;
    // is Enenmy or Player party
    public bool isPlayable;
    // panel bars for instant & lerping
    public GameObject lerperBar;
    public GameObject insantBar;
    public RectTransform UIpos;
    public bool canLerpBar = false;

    public void SetUIPos(RectTransform pos) {
        UIpos = pos;
    }

    public void AddListener() {

    }

    // need to implement. This should be used to show where the next turn panel will be if the action currently chosen takes place
    public void SetNextTurnPanel() {
        
    }

    public void ResetBAV() {
        bav = Mathf.Abs(10000f / spd);
    }
    
    
    public float GetBAV() {
        return bav;
    }

    public void AddBAV(float factor) {
        bav += factor;
    }
    
    
    public Transform Initialize(GameObject spawnPos) {
        // load model into scene and place at spawn Pos
        // Debug.Log("[Attempting to instantiate...] : [" + character.getName() + "]");
        
        string name = "" + count;
        Transform charPos = spawnPos.transform.Find("Party Positions").transform.Find(name).transform;
        charGO = Object.Instantiate(character.getCharModel(), charPos);
        camGroup = charGO.transform.Find("StateDrivenCamera").gameObject;
        camGroup.SetActive(false);
        camGroup.transform.SetParent(charGO.transform.parent);
        camGroup.transform.localPosition += new Vector3(-0.2f*count, 0, -0.24f*count);

        camGroup.transform.Find("CM vcam ally").transform.position = charGO.transform.parent.parent.parent.Find("lookAtPos").position;
        //camGroup.transform.Find("CM vcam ally").LookAt(charGO.transform.parent.parent);
        if(character.getSkillOne().skillType == "heal") {
            camGroup.transform.Find("CM vcam skill1").transform.position = camGroup.transform.Find("CM vcam ally").transform.position;
            camGroup.transform.Find("CM vcam skill1").GetComponent<CinemachineVirtualCamera>().LookAt = BattleManager.battle.cmTGallies.GetComponent<CinemachineTargetGroup>().transform;
        }
        else {
            camGroup.transform.Find("CM vcam skill1").GetComponent<CinemachineVirtualCamera>().LookAt = BattleManager.battle.cmTG.GetComponent<CinemachineTargetGroup>().transform;
        }
            camGroup.transform.Find("CM vcam skill1").GetComponent<Cinemachine.CinemachineVirtualCamera>().m_Lens.FieldOfView = 35f;
        if(character.getSkillTwo().skillType == "heal") {
            camGroup.transform.Find("CM vcam skill2").transform.position = camGroup.transform.Find("CM vcam ally").transform.position;
            camGroup.transform.Find("CM vcam skill2").GetComponent<CinemachineVirtualCamera>().LookAt = BattleManager.battle.cmTGallies.GetComponent<CinemachineTargetGroup>().transform;
        }
        else {
            camGroup.transform.Find("CM vcam skill2").GetComponent<CinemachineVirtualCamera>().LookAt = BattleManager.battle.cmTG.GetComponent<CinemachineTargetGroup>().transform;
            camGroup.transform.Find("CM vcam skill2").transform.position += new Vector3(0.25f,-0.05f,0);
        }
            camGroup.transform.Find("CM vcam skill2").GetComponent<Cinemachine.CinemachineVirtualCamera>().m_Lens.FieldOfView = 35f;
        if(character.getSkillUlt().skillType == "heal") {
            camGroup.transform.Find("CM vcam ultFoe").transform.position = camGroup.transform.Find("CM vcam ally").transform.position;
            camGroup.transform.Find("CM vcam ultFoe").GetComponent<CinemachineVirtualCamera>().LookAt = BattleManager.battle.cmTGallies.GetComponent<CinemachineTargetGroup>().transform;
        }
        else {
            camGroup.transform.Find("CM vcam ultFoe").GetComponent<CinemachineVirtualCamera>().LookAt = BattleManager.battle.cmTG.GetComponent<CinemachineTargetGroup>().transform;
            camGroup.transform.Find("CM vcam ultFoe").transform.position += new Vector3(.25f,-0.05f,0);
        }
            camGroup.transform.Find("CM vcam ultFoe").GetComponent<Cinemachine.CinemachineVirtualCamera>().m_Lens.FieldOfView = 35f;
        


        Transform targetPos;
        try {
            targetPos = charGO.transform.Find("TargetHP").transform;
        } catch (System.NullReferenceException) {
            targetPos = charGO.transform.Find("Armature").Find("Hips").Find("Spine").Find("Chest").Find("Neck").Find("Head");
            targetPos.position += targetPos.up * 0.1f;
        }
         

        if(charGO.GetComponent<Animator>() == null)
            charGO.AddComponent<Animator>();
        charGO.GetComponent<Animator>().runtimeAnimatorController = Resources.Load<AnimatorOverrideController>("Animator/Battle/"+character.getName()+"Battle");
        /*
        if(character.getName() == "Kyle") {
            Debug.Log("Kyle animator attempting");
            charGO.GetComponent<Animator>().runtimeAnimatorController = Resources.Load<AnimatorOverrideController>("Animator/Battle/"+isPlayable+"/KyleBattle_"+isPlayable);
        }
        else  {
            //charGO.GetComponent<Animator>().runtimeAnimatorController = Resources.Load<AnimatorOverrideController>("Animator/Battle/"+isPlayable+"/"+character.getName()+"Battle_"+isPlayable);
            charGO.GetComponent<Animator>().runtimeAnimatorController = Resources.Load<AnimatorOverrideController>("Animator/"+character.getName()+"Battle");
        }*/
        



        if(charGO.GetComponent<EventReciever>() == null)
            charGO.AddComponent<EventReciever>();
        SetTurnPanelColor();
        if(!isPlayable) {
            return targetPos;
        }
        else { // playable
            // set indicator color to blue by default
            turnIndicator.transform.Find("CharImgSlot").GetComponent<UnityEngine.UI.Image>().color = new Color(0.5132076f,0.7934105f,1f,0.6f);
            // set ult material
            statusPanel.transform.Find("ult").GetComponent<UnityEngine.UI.Image>().material = Resources.Load<Material>("Materials/UltButton/ultMat_"+this.count);
            // change layer of party position parent to correspoding allyPos(posNumb)
            foreach (Transform child in charPos) {
                if(camGroup == child.gameObject) continue;
                SetGameLayerRecursive(child.gameObject, LayerMask.NameToLayer("allyPos"+(count)));
            }
            return null;
        }

        // load animator controller
    }

    private void SetTurnPanelColor() {
        turnIndicator.transform.Find("CharImgSlot").GetComponent<UnityEngine.UI.Image>().color = isPlayable ? new Color(0.5132076f,0.7934105f,1f,0.6f) : new Color(1f,0f,0f,0.6f);
    }

    private void SetGameLayerRecursive(GameObject _go, int _layer) {
            _go.layer = _layer;
            foreach (Transform child in _go.transform)
            {
                child.gameObject.layer = _layer;
 
                Transform _HasChildren = child.GetComponentInChildren<Transform>();
                if (_HasChildren != null)
                    SetGameLayerRecursive(child.gameObject, _layer);
             
            }
    }

    public void UpdateStatusPanel() {
        if(isUlt) return;
        insantBar.GetComponent<Slider>().value = character.getHp() / character.getMaxHp();
        if(this.canLerpBar) {
            float f = Mathf.Lerp(lerperBar.GetComponent<Slider>().value, insantBar.GetComponent<Slider>().value, 0.0125f * BattleManager.battle.gameSpeed);
            lerperBar.GetComponent<Slider>().value = f;
        }
        statusPanel.transform.Find("HpShield").Find("Shield slider").GetComponent<Slider>().value = character.getShield() / character.getMaxHp();
        try {
            statusPanel.transform.Find("HpShield").Find("HPtextSMP").GetComponent<TextMeshProUGUI>().text = Mathf.Max(0,(int)(character.getHp())).ToString();
        } catch {
            statusPanel.transform.Find("HPtextSMP").GetComponent<TextMeshProUGUI>().text = Mathf.Max(0,(int)(character.getHp())).ToString();
        }
    }

    public void AfterTurnExecute() {
        if(isUlt && reference != null)
            this.reference.character.resetEnergy();
        else
            return;
    }

    public void BeforeTurnExecute() {
        return;
    }
    public float lerpEnd = 1.0f;

    public void ChangeHP(float factor) {
        this.character.addHp(factor);
        Color colorChange = factor > 0f ? new Color(.45f,1f,.55f,1f) : new Color(1f,0.7f,0.7f,1f);
        statusPanel.transform.Find("HpShield").Find("ChangeHP slider").Find("Fill Area").Find("HP Fill").GetComponent<Image>().color = colorChange;
        lerperBar = factor < 0 ? statusPanel.transform.Find("HpShield").Find("ChangeHP slider").gameObject : statusPanel.transform.Find("HpShield").Find("HP slider").gameObject;
        insantBar = factor < 0 ? statusPanel.transform.Find("HpShield").Find("HP slider").gameObject : statusPanel.transform.Find("HpShield").Find("ChangeHP slider").gameObject;
        lerpEnd = insantBar.GetComponent<Slider>().value;
        if(UIpos != null) {
            GameObject dmgText = GameObject.Instantiate(Resources.Load<GameObject>("Placeholders/AffectedIndicator"), UIpos, false);
            dmgText.GetComponent<AffectedIndicator>().Initialize(factor, new Color(1f,1f,1f,1f));
        }
    }

    public void Heal(float factor) {
        character.addHp(factor);
    }

    public void Damage(float factor) {
        character.addHp(-factor);
    }

    

    public int CompareTo(object obj, int mode)
    {
        if(mode == 1) {
            //Debug.Log("mode is 1");
            if(obj == this) return 0;
            else    return this.character.getHp().CompareTo(((CharacterBattle)obj).character.getHp());
        }
        else if(mode == 2) {
            //Debug.Log("mode is 2");
            return this.spd.CompareTo(((CharacterBattle)obj).spd);
        }
        else if (mode == 3) {
            //Debug.Log("mode is 2");
            return this.character.CompareTo(((CharacterBattle)obj).character, 1);
        }
        else {
            if(obj == null)
                throw new System.ArgumentException("null Obj [CharacterBattle class CompareTo method]");
            if(!(obj is CharacterBattle))
                throw new System.ArgumentException("compared is not CharacterBattle class [CharacterBattle class CompareTo method]");
            CharacterBattle other = (CharacterBattle)obj;
            //Debug.Log("Comparing " + this.character.getName() + " with " + other.character.getName());
            //Debug.Log("Comparing " + this.GetBAV() + " with " + other.GetBAV() + " [CharacterBattle class CompareTo method]");
            // 비교대상과 bav가 같은 경우 자기 자신이라고 판단
            //Debug.Log("Comparing " + this.GetBAV() + " with " + other.GetBAV() + " [CharacterBattle class CompareTo method]");
            if(other.GetBAV() == this.GetBAV()) {
                return 1;
                /*
                Debug.Log("FOUND bav same!");
                if(other == this) return 0;
                else    return 1;
                */
            }
            else if (other.GetBAV() < this.GetBAV())
                return 1;
            else if (other.GetBAV() > this.GetBAV())
                return -1;
            else
                throw new System.ArgumentException("other.currSpeed is not comparable float [CharacterBattle class CompareTo method]");
        }
        
    }

    public int CompareTo(object obj)
    {
        return this.CompareTo(obj, 0);
    }
}