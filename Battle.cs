using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using TMPro;


[SerializeField]
public class Battle : MonoBehaviour
{
    [SerializeField] public GameObject battleCanvas;
    [SerializeField] public GameObject[] positions;
    [SerializeField] public AudioSource bgm;
    [SerializeField] public AudioSource sfx;
    [SerializeField] public AudioSource buttonSound;


    public GameObject Map { 
        get { return Map; }
        set { Map = value; }
    }

    public CharacterInterface[] characters;

    public CharacterInterface[] enemies;

    public PartyBuffInterface PartyBuff {
        get { return PartyBuff; }
        set { PartyBuff = value; }
    }

    public PartyBuffInterface EnemyBuff {
        get { return EnemyBuff; }
        set { EnemyBuff = value; }
    }

    public int maxTurns;

    public int GameCurrentTurn {
        get { return gameCurrentTurn; }
    }

    private int gameCurrentTurn;
    
    private RBT<CharacterBattle> turnTree; // bad name... I probably should change it
    private List<CharacterBattle> turnOrder;
    private Queue<CharacterBattle> turnQueue;
    private List<CharacterBattle> deadCharacters = new List<CharacterBattle>();
    private GameObject turn;
    private GameObject turnPanelTargets;
    private GameObject turnPanelSample;
    public GameObject turnPanelParent;
    private GameObject statusPanelParent;
    private GameObject skillButtonsParent;
    
    bool isInBattle;

    private Camera mCamera;
    public CinemachineTargetGroup cmTG;
    public CinemachineTargetGroup cmTGallies;
    public BattleEnemyHpGUI enemyHpGUI;
    public BattleEnemyHpGUI enemyTargetGUI;
    public BattleEnemyHpGUI allyTargetGUI;
    public bool canUpdateStatusPanel = false;

    public bool isTurnOver;
    private int turnOverResponses = 0;
    private int expectedResponse = 0;
    private GameObject gameOverPanel;
    
    private int skillPoints = 0;
    private int maxSkillPoints = 5;
    private GameObject skillPointsHolder;

    public bool awaitInitialisation = true;
    public CharacterInterface[] initialAllyParty;
    public CharacterInterface[] initialEnemyParty;
    public List<GameObject> destroyListTurnEnd;
    public int gameSpeed = 1;
    public bool auto = false;
    public bool debug;

    void Start() {
        Debug.Log("Battle Start");
        gameOverPanel = battleCanvas.transform.Find("GameOverPanel").gameObject;
        gameOverPanel.transform.Find("ButtonGroup").Find("ReturnButton").GetComponent<Button>().onClick.AddListener(delegate {
            ReturnToStart();
        });
        gameOverPanel.transform.Find("ButtonGroup").Find("RetryButton").GetComponent<Button>().onClick.AddListener(delegate {
            RetryBattle();
        });
        battleCanvas.transform.Find("SpeedPauseAutoPanel").Find("DoubleSpeed").Find("Text").GetComponent<Text>().text =""+gameSpeed+"x";
        battleCanvas.transform.Find("SpeedPauseAutoPanel").Find("DoubleSpeed").GetComponent<Button>().onClick.AddListener(delegate {
            GameSpeedSwitch();
            battleCanvas.transform.Find("SpeedPauseAutoPanel").Find("DoubleSpeed").Find("Text").GetComponent<Text>().text =""+gameSpeed+"x";
        });
        battleCanvas.transform.Find("SpeedPauseAutoPanel").Find("Auto").GetComponent<Button>().onClick.AddListener(delegate {
            auto = !auto;
            battleCanvas.transform.Find("SpeedPauseAutoPanel").Find("Auto").Find("Text").GetComponent<Text>().text = (auto) ? "Auto" : "Manual";
        });
        
        awaitInitialisation = true;
        gameOverPanel.SetActive(false);
        //if(DataManager.allyParties == null)
        if(DataManager.debug)
            DebugInstantiateBattle(4,4);
        InstantiateBattle();
        Initialize();
        StartCoroutine(CoLoopTurn());
        //StartCoroutine(CoUpdateUI());
    }

    

    public void GameSpeedSwitch() {
        gameSpeed = Mathf.Max(1, (gameSpeed + 1)%4);
        Time.timeScale = gameSpeed;
    }

    public void PauseGame() {
        Time.timeScale = Time.timeScale == 0 ? gameSpeed : 0;
        bool isInteractable = Time.timeScale == 0 ? false : true;
        battleCanvas.transform.Find("PausePanel").gameObject.SetActive(!isInteractable);
        StopInteractable(isInteractable);
    }

    private void StopInteractable(bool isInteractable) {
        enemyTargetGUI.ChangeInteractableAll(isInteractable);
        DataManager.UIHandler.skillButtonGrouper.ChangeInteractableAll(isInteractable);
        foreach(Transform child in battleCanvas.transform.Find("SpeedPauseAutoPanel")) {
            child.GetComponent<Button>().interactable = isInteractable;
        }
        
    }

    public void ReturnToStart() {
        Time.timeScale = gameSpeed;
        UnityEngine.SceneManagement.SceneManager.LoadScene("StartScene");
    }

    public void RetryBattle() {
        awaitInitialisation = true;
        DataManager.allyParties[DataManager.currentParty].SetParty(initialAllyParty);
        DataManager.enemyParty.SetParty(initialEnemyParty);
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        Resources.UnloadUnusedAssets();
        Time.timeScale = gameSpeed;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            PauseGame();
        }
        
    }

    // UI that needs updating at specific times
    private IEnumerator CoUpdateUI() {
        if(!awaitInitialisation || updateUI != null) {
            updateUI();
        }
        yield return null;
    }

    // camera will move actively, so we need to update UI every frame
    void OnGUI() { 
        if(awaitInitialisation) return;
        enemyTargetGUI.UpdateGUICam();
        allyTargetGUI.UpdateGUICam();
        DataManager.UIHandler.skillButtonGrouper.UpdateUltButton();
        
        /*
        if(BattleManager.skillMainTarget != null && !isBattleOver()) {
            Vector3 pos = BattleManager.skillMainTarget.charGO.transform.position;
            Vector3 screenPos = RectTransformUtility.WorldToScreenPoint (mCamera, pos);
            GUI.Label(new Rect(screenPos.x, screenPos.y, 100, 100), "TARGET");
        }
        */
        //if(canUpdateStatusPanel) {
        if(true) {
            //Debug.Log("Updating Status Panel");
            foreach(CharacterBattle charBattle in allyCharacters) {
                charBattle.UpdateStatusPanel();
            }
            foreach(CharacterBattle charBattle in enemyCharacters) {
                charBattle.UpdateStatusPanel();
            }
            foreach(CharacterBattle charBattle in deadCharacters) {
                charBattle.UpdateStatusPanel();
            }
            
        }
    }
 


    delegate void UpdateUI();
    UpdateUI updateUI;


    public GameObject[] partyModels;
    public GameObject[] enemyModels;

    public List<CharacterBattle> allyCharacters;
    public List<CharacterBattle> enemyCharacters;
    

    // InstantiateBattle 메소드가 정상작동하도록 static 클래스들을 설정해둠.
    // 배틀 직전의 씬에서 스크립트가 이 메소드의 역할을 해줘야함 (set up enemies, etc)
    private void DebugInstantiateBattle(int partySize, int enemySize) {
        DataManager.readJson();
        DataManager.ReadSkillJson();
        CharacterInterface[] tcharacters = new CharacterInterface[partySize];
        tcharacters[0] = new Character(DataManager.userInfo.userChars[3].Clone());
        tcharacters[1] = new Character(DataManager.userInfo.userChars[1].Clone());
        tcharacters[2] = new Character(DataManager.userInfo.userChars[2].Clone());
        tcharacters[3] = new Character(DataManager.userInfo.userChars[0].Clone());

        CharacterInterface[] tenemies = new CharacterInterface[enemySize];
        tenemies[0] = new Character(DataManager.userInfo.enemyChars[0].Clone());
        tenemies[1] = new Character(DataManager.userInfo.enemyChars[1].Clone());
        tenemies[2] = new Character(DataManager.userInfo.enemyChars[2].Clone());
        tenemies[3] = new Character(DataManager.userInfo.enemyChars[0].Clone());
        Party allyParty = new Party();
        Party enemyParty = new Party();
        allyParty.SetParty(tcharacters);
        enemyParty.SetParty(tenemies);
        DataManager.allyParties[DataManager.currentParty] = allyParty;
        DataManager.enemyParty = enemyParty;
    }

    // UP TO DATE
    // 아군, 적군, 맵, 아군버프, 적군버프를 가져오는 메소드. new로 Insantiate 한다
    private void InstantiateBattle() {
        // battleCanvas = Instantiate(Resources.Load("Prefabs/BattleCanvas") as GameObject);
        // positions = new GameObject[2];
        // positions[0].AddComponent<ObjectLayout>();
        // positions[1].AddComponent<ObjectLayout>();
        // positions[0].GetComponent<ObjectLayout>().SetUp(characters.Length);
        // positions[1].GetComponent<ObjectLayout>().SetUp(enemies.Length);
        destroyListTurnEnd = new List<GameObject>();
        initialAllyParty = new CharacterInterface[DataManager.GetCurrentParty().GetParty().Length];
        int counter = 0;
        foreach ( Character character in DataManager.GetCurrentParty().GetParty() ) {
            initialAllyParty[counter] = new Character(character.charSt.Clone());
            counter++;
        }
        counter = 0;
        initialEnemyParty = new CharacterInterface[DataManager.enemyParty.GetParty().Length];
        foreach ( Character character in DataManager.enemyParty.GetParty() ) {
            initialEnemyParty[counter] = new Character(character.charSt.Clone());
            counter++;
        }
        
        mCamera = GameObject.FindGameObjectsWithTag("MainCamera")[0].GetComponent<Camera>();
        //enemyTargetGUI = new BattleEnemyHpGUI(mCamera);
        enemyTargetGUI = new BattleEnemyHpGUI(mCamera);
        allyTargetGUI = new BattleEnemyHpGUI(mCamera);
        BattleManager.battle = this;
        
        DataManager.UIHandler.battleEnemyHpGUI = enemyTargetGUI;
        DataManager.UIHandler.enemyTargetGUI = enemyTargetGUI;
        DataManager.UIHandler.allyTargetGUI = allyTargetGUI;
        DataManager.UIHandler.skillButtonGrouper = new SkillButton(battleCanvas.transform.Find("SkillButtons").GetComponentsInChildren<Button>());
        DataManager.UIHandler.skillButtonGrouper.ultActiveButton = battleCanvas.transform.Find("SkillButtons").Find("ultActive").GetComponent<Button>();
        if(skillPointsHolder == null)
            skillPointsHolder = battleCanvas.transform.Find("SkillPointsHolder").gameObject;
        DataManager.UIHandler.skillPointsManager = new SkillPointsManager(3, 5, skillPointsHolder);
        BattleManager.skillPointsManager = DataManager.UIHandler.skillPointsManager;

        characters = DataManager.GetCurrentParty().GetParty();  // 아군
        enemies = DataManager.enemyParty.GetParty(); // 적군
        allyCharacters = new List<CharacterBattle>();
        enemyCharacters = new List<CharacterBattle>();
        //partyModels = new GameObject[characters.Length];
        //enemyModels = new GameObject[enemies.Length];
        positions[0].GetComponent<ObjectLayout>().SetUp(characters.Length, positions[1].transform);
        positions[1].GetComponent<ObjectLayout>().SetUp(enemies.Length, positions[0].transform);
        cmTG = new GameObject("CM TargetGroup").AddComponent<CinemachineTargetGroup>();
        cmTGallies = new GameObject("CM TargetGroup Allies").AddComponent<CinemachineTargetGroup>();
    }

    // assigns variables and instantiates required objects. creat RBT and dictionaries
    void Initialize() {
        // game current turn starts at 1, but this is not used for anything yet.
        gameCurrentTurn = 1;
        // assign gameobjects to variables
        turn = battleCanvas.transform.Find("Turn").gameObject;
        turnPanelTargets = turn.transform.Find("TurnPanelTargets").gameObject;
        turnPanelParent = turn.transform.Find("TurnPanelSample").Find("TurnPanelMask").gameObject;
        turnPanelSample = turnPanelParent.transform.Find("CharTurnSample").gameObject;
        // create RBT data structure that will be used to keep track of turn order of characters
        turnTree = new RBT<CharacterBattle>();
        // count is used to indicate which turn panel or position the character will be assigned to. Materials are specific to each position.
        int count = 0;
        statusPanelParent = battleCanvas.transform.Find("CharPanels").gameObject;
        foreach(Character character in characters) {
            if(character == null) continue;
            //CharacterBattle charBattle = new CharacterBattle(PartyBuff, character, turnPanelSample, statusPanel.transform.GetChild(count).gameObject, true);
            CharacterBattle charBattle = new CharacterBattle(count, character, turnPanelSample, turnPanelParent, statusPanelParent, true);
            charBattle.Initialize(positions[0]);
            allyCharacters.Add(charBattle);
            Transform hipsTarget;
            try {
                hipsTarget = charBattle.charGO.transform.Find("Armature").Find("Hips");
            } catch {
                hipsTarget = charBattle.charGO.transform.Find("Root").Find("Hip");
            }
            cmTGallies.AddMember(hipsTarget, 0.01f, 0);
            allyTargetGUI.SetUpNewRectTransformGrouper(hipsTarget,positions[0].transform.Find("Party Positions").Find(count.ToString()), charBattle);
            count += 1;
            turnTree.insert(charBattle);
        }
        cmTGallies.AddMember(positions[0].transform, 0.01f, 0);
        cmTGallies.m_Targets[allyCharacters.Count].weight = 10.0f;
        count = 0;
        statusPanelParent = battleCanvas.transform.Find("EnemyPanels").gameObject;
        foreach(Character enemy in enemies) {
            if(enemy == null) continue;
            //CharacterBattle charBattle = new CharacterBattle(EnemyBuff, enemy, turnPanelSample, statusPanel.transform.GetChild(count).gameObject, false);
            CharacterBattle charBattle = new CharacterBattle(count, enemy, turnPanelSample, turnPanelParent, statusPanelParent, false);
            //add that enemy hp bar to the enemyHpGUI
            Transform target = charBattle.Initialize(positions[1]);
            enemyCharacters.Add(charBattle);
            // hp bar
            enemyTargetGUI.SetUpNewRectTransformGrouper(charBattle.statusPanel.GetComponent<RectTransform>(), target);
            Transform hipsTarget;
            try {
                hipsTarget = charBattle.charGO.transform.Find("Armature").Find("Hips");
            } catch {
                hipsTarget = charBattle.charGO.transform.Find("Root").Find("Hip");
            }
            // targeting button & damage text target
            enemyTargetGUI.SetUpNewRectTransformGrouper(hipsTarget,positions[1].transform.Find("Party Positions").Find(count.ToString()), charBattle);
            cmTG.AddMember(hipsTarget, 0.01f, 0);
            count += 1;
            turnTree.insert(charBattle);
        }
        //skillButtonsGroup = battleCanvas.transform.Find("SkillButtons").gameObject;

        
        
        DataManager.UIHandler.skillButtonGrouper.SetUpUltButtons(allyCharacters);
        awaitInitialisation = false;
        

    }    

    private void UpdateTurnPanels(List<CharacterBattle> inOrderTraversal) {
        //string debugmsg = "";
        foreach(CharacterBattle charBattle in inOrderTraversal) {
            charBattle.turnIndicator.GetComponent<CharPanelUpdatePos>().target = turnPanelTargets.transform.GetChild(inOrderTraversal.IndexOf(charBattle)+1).gameObject;
            charBattle.turnIndicator.transform.Find("CharImgSlot").Find("BAV").GetComponent<TMP_Text>().text = ((int)charBattle.GetBAV()).ToString();
            if(charBattle == BattleManager.skillMainTarget)
                charBattle.turnIndicator.transform.Find("CharImgSlot").position = charBattle.turnIndicator.transform.Find("targetedPos").position;
            else
                charBattle.turnIndicator.transform.Find("CharImgSlot").position = charBattle.turnIndicator.transform.Find("defaultPos").position;
            //debugmsg += charBattle.character.getName() + " [ BAV : " + charBattle.GetBAV() + " ]\n";
        }

        
        //Debug.Log(debugmsg);
    }

    public void AddToTurnQueue(CharacterBattle charBattle) {
        //Debug.Log("Adding " + charBattle.character.getName() + " to turn queue");
        charBattle.turnIndicator.transform.localScale = new Vector3(1.25f,1.25f,1.25f);
        charBattle.turnIndicator.transform.Find("CharImgSlot").GetComponent<Image>().color += new Color(0,0,0,1.0f);
        turnQueue.Enqueue(charBattle);
        AddToDeepestChild(turnPanelTargets.transform.Find("TurnQueue").GetChild(0).transform, charBattle.turnIndicator.transform);
        
    }

    public void AddToDeepestChild(Transform newParent, Transform toAdd) {
        if(newParent.Find("Interruptions").childCount == 0) {
            toAdd.parent = newParent.Find("Interruptions");
            return;
        }
        AddToDeepestChild(newParent.Find("Interruptions").GetChild(0), toAdd);
    }

    public void SetSFX() {
        if(BattleManager.skill.skillType.Equals("heal")) {
            sfx.time = 0.0f;
            sfx.clip = Resources.Load<AudioClip>("SFX/Heal");
        }
        else if(BattleManager.skill.skillType.Equals("damage")) {
            sfx.clip = Resources.Load<AudioClip>("SFX/Damage");
            sfx.time = 0.3f;
        }
    }
    public void PlayButtonSound() {
        buttonSound.Play();
    }
    public void PlaySFX() {
        if(BattleManager.skill.skillType.Equals("damage"))
            sfx.time = 0.3f;
        else
            sfx.time = 0.0f;
        sfx.Play();
    }

    private IEnumerator CoLoopTurn()
    {
        //Debug.Log("코루틴 하지메루요");
        while(!isBattleOver()) {
            isTurnOver = false;
            canUpdateStatusPanel = false;
            // create list from smallest to largest BAV (inorder traversal)
            turnOrder = turnTree.dataInOrder();
            string debugmsg = "Start of turn\n";
            foreach(CharacterBattle charBattle in turnOrder) {
                debugmsg += charBattle.character.getName() + " [ BAV : " + charBattle.GetBAV() + " ]\n";
            }
            //Debug.Log(debugmsg);
            // update turn panel positions based on increasing BAV
            // UpdateTurnPanels(inOrderTraversal);
            // subtract lowest BAV from all BAVs (lowest BAV therefore becomes 0)
            //CharacterBattle charBattle0 = turnTree.GetSmallestData();
            CharacterBattle charBattle0 = turnOrder[0];
            float lowestBAV = charBattle0.GetBAV();
            foreach(CharacterBattle charBattle in turnOrder) {
                charBattle.AddBAV(-lowestBAV);
            }

            bool removed = turnTree.remove(charBattle0);
            turnOrder = turnTree.dataInOrder();
            charBattle0.ResetBAV(); 
            turnTree.insert(charBattle0);
            charBattle0.turnIndicator.transform.Find("Outline").gameObject.SetActive(true);
            turnOrder = turnTree.dataInOrder();
            // LERP PANELS
            // set turn indicator positions (turn panel targets. panels will lerp to these targets)
            UpdateTurnPanels(turnOrder);
            CharacterBattle addToQueue = new CharacterBattle(charBattle0);
            turnQueue = new Queue<CharacterBattle>();
            turnQueue.Enqueue(addToQueue);
            addToQueue.turnIndicator.transform.parent = turnPanelTargets.transform.Find("TurnQueue");
            
            
            while(turnQueue.Count > 0 && !isBattleOver()) {
                isTurnOver = false;
                CharacterBattle currentTurn = turnQueue.Dequeue();
                currentTurn.turnIndicator.transform.localScale = new Vector3(1.2f,1.2f,1.2f);
                currentTurn.turnIndicator.transform.Find("CharImgSlot").GetComponent<Image>().color += new Color(0,0,0,1.0f);
                //currentTurn.turnIndicator.transform.localPo = new Vector3(1.2f,1.2f,1.2f);
                //Debug.Log("Current Turn: " + currentTurn.character.getName());
                //Debug.Log("TurnQueue Count: " + turnQueue.Count);
                yield return RunCharTurn(currentTurn);

            }
        }
        gameOverPanel.SetActive(true);
        gameOverPanel.transform.Find("GameOverText").GetComponent<Text>().text = (enemyCharacters.Count == 0) ? "Victory!" : "Defeat!";
        this.gameObject.GetComponent<AudioSource>().clip = (enemyCharacters.Count == 0) ? Resources.Load<AudioClip>("BGM/Victory") : Resources.Load<AudioClip>("BGM/Defeat");
        this.gameObject.GetComponent<AudioSource>().Play();
        bgm.volume *= 0.25f;
        StopInteractable(false);


    }
    bool setupCam = false;
    bool executed = false;
    private IEnumerator RunCharTurn(CharacterBattle characterBattle) {
        BattleManager.DestroyObjects();
        // add current character to cinemachine target group if playable (your turn)
        /*
        if(characterBattle.isPlayable) {
            try {
                cmTG.AddMember(characterBattle.reference.charGO.transform.Find("Armature").Find("Hips"), 1f, 0); // this can be changed to something with position constraint on the hips
            }
            catch (Exception e) {
                cmTG.AddMember(characterBattle.reference.charGO.transform.Find("Root").Find("Hip"), 1f, 0);
            }
            foreach(Transform child in characterBattle.reference.camGroup.transform)
                child.GetComponent<CinemachineVirtualCamera>().LookAt = cmTG.GetComponent<CinemachineTargetGroup>().transform;
        }
        */
        ResetCamLayerMask();


        BattleManager.skillCaster = characterBattle.reference;
        characterBattle.reference.charGO.GetComponent<Animator>().SetBool("turn", true);
        // set default target (manual targeting 또한 SetTarget 메소드에서 처리함.)
        CharacterBattle skill_target;
        BattleManager.isUlt = characterBattle.isUlt;
        if(!characterBattle.isUlt) {
            skill_target = SetTarget(characterBattle, characterBattle.character.getSkillOne(), null);
        } else {
            skill_target = SetTarget(characterBattle.reference, characterBattle.reference.character.getSkillUlt(), null);
            BattleManager.skill = characterBattle.reference.character.getSkillUlt();
            characterBattle.reference.charGO.GetComponent<Animator>().SetTrigger("u");
        }

        CharacterBattle camFocusTarget = characterBattle.reference.isPlayable ? characterBattle.reference : skill_target ;
        SetCams(camFocusTarget, true);
        if(!characterBattle.reference.isPlayable || auto) {
            DataManager.UIHandler.skillButtonGrouper.HideSkillButtons();
            BattleManager.skillCaster = characterBattle.reference;
            if(!characterBattle.isUlt) BattleManager.skill = characterBattle.reference.character.getSkillOne();
            BattleManager.ExecuteBattle();
            executed = true;
        }
        else {
            SetUI(characterBattle.reference, characterBattle.isUlt);
        }
        while(!isTurnOver)
        {
            UpdateTurnPanels(turnOrder);
            if(auto && !executed) {
                BattleManager.ExecuteBattle();
                executed = true;
            }
            // if character if found within character interface
            yield return null;
        }
        //BattleManager.ChangeCanLerpBar(true);
        SetCams(camFocusTarget, false);
        executed = false;
        if(!isBattleOver()) {
            if(characterBattle.turnIndicator.transform.Find("Interruptions").childCount > 0)
                characterBattle.turnIndicator.transform.Find("Interruptions").GetChild(0).parent = characterBattle.turnIndicator.transform.parent;
            Destroy(characterBattle.turnIndicator.gameObject);

            if(characterBattle.reference.isPlayable) {
                try {
                    cmTG.RemoveMember(characterBattle.reference.charGO.transform.Find("Armature").Find("Hips"));
                }
                catch (Exception e) {
                    cmTG.RemoveMember(characterBattle.reference.charGO.transform.Find("Root").Find("Hip"));
                }
            }
            characterBattle.reference.turnIndicator.transform.Find("Outline").gameObject.SetActive(false);
        }
    
        // check if anyone is dead(list of characters to remove from RBT)
        // currently uses brute force method of checking all characters.
        foreach(CharacterBattle charBattle in turnOrder) {
            if(charBattle.character.getHp() <= 0) {
                // remove from allyCharacters or enemyCharacters
                if(charBattle.isPlayable) {
                    allyCharacters.Remove(charBattle);
                }
                else {
                    //Debug.Log("Attempting to remove " + charBattle.character.getName() + " from enemyCharacters");
                    enemyCharacters.Remove(charBattle);
                    //Debug.Log("Removed " + charBattle.character.getName());
                }
                charBattle.charGO.GetComponent<Animator>().SetBool("dead", true);
                deadCharacters.Add(charBattle);
                charBattle.turnIndicator.SetActive(false);
                // remove from cinemachine target group
                try {
                    cmTG.RemoveMember(charBattle.charGO.transform.Find("Armature").Find("Hips"));
                }
                catch (Exception e) {
                    cmTG.RemoveMember(charBattle.charGO.transform.Find("Root").Find("Hip"));
                }
                //Debug.Log("Remaining:" + turnTree.dataInOrder().Count);                
                //Debug.Log("Attempting to remove " + charBattle.character.getName() + " from turnTree");
                turnTree.remove(charBattle);
                //Debug.Log("Removed " + charBattle.character.getName());
            }
        }

        characterBattle.AfterTurnExecute();

        turnOrder = turnTree.dataInOrder();
        UpdateTurnPanels(turnOrder);
    }

    private void SetUI(CharacterBattle characterBattle, bool isUlt) {
        // set up skill buttons
        if(isUlt)
            DataManager.UIHandler.skillButtonGrouper.SetUpUlt(characterBattle.character.getSkills()[0], characterBattle);
        else
            DataManager.UIHandler.skillButtonGrouper.SetUpSkillButton(characterBattle.character.getSkills());
        // characterBattle.camGroup.SetActive(true);
        // characterBattle.camGroup.GetComponent<CinemachineStateDrivenCamera>().enabled = true;
        // set up ult buttons
        // set up camera
        
    }

    private void SetCams(CharacterBattle characterBattle, bool isOn) {
        if(setupCam == isOn) return;
        setupCam = isOn;
        if(isOn) {
            try {
                cmTG.AddMember(characterBattle.charGO.transform.Find("Armature").Find("Hips"), 1f, 0); // this can be changed to something with position constraint on the hips
                cmTG.m_Targets[enemyCharacters.Count].weight = 10.0f;
            }
            catch (Exception e) {
                cmTG.AddMember(characterBattle.charGO.transform.Find("Root").Find("Hip"), 1f, 0);
                cmTG.m_Targets[enemyCharacters.Count].weight = 10.0f;
            }
            /*
            foreach(Transform child in characterBattle.camGroup.transform)
                child.GetComponent<CinemachineVirtualCamera>().LookAt = cmTG.GetComponent<CinemachineTargetGroup>().transform;
            */
        }
        else {
            try {
                cmTG.RemoveMember(characterBattle.charGO.transform.Find("Armature").Find("Hips"));
            }
            catch (Exception e) {
                cmTG.RemoveMember(characterBattle.charGO.transform.Find("Root").Find("Hip"));
            }
        }
        for(int i = 0; i < allyCharacters.Count; i++) {
            cmTGallies.m_Targets[i].weight = 1.0f;
        }
        characterBattle.camGroup.SetActive(isOn);
        characterBattle.camGroup.GetComponent<CinemachineStateDrivenCamera>().enabled = isOn;
    }

    // 타겟 설정 (디폴트와 수동 둘다 여기서). 마지막에 SetCamLayerMask()를 통해 카메라 레이어 마스크 설정
    public CharacterBattle SetTarget(CharacterBattle self, SkillInterface skill, CharacterBattle tgt) {
        // 디폴트 타겟 설정
        CharacterBattle mainTarget = tgt;
        int range = skill.skillRange;
        if(mainTarget == null) {
            // for future implementation of various skill types
            if(skill.skillType.Equals("EnhanceSkill")) {
                skill.target = (skill.skillID.Contains("a")) ? self.character.getSkillOne() : self.character.getSkillTwo();
                mainTarget = self;
            }
            else if(skill.skillType.Equals("heal")) {
                //Debug.Log("skill type is : [ "+skill.skillType+" ] ,and Equals heal :"+skill.skillType.Equals("heal"));
                //Debug.Log("Heal skill");
                mainTarget = this.GetRandAggro(self.isPlayable);
                //mainTarget = turnTree.dataInOrderFilter(self.isPlayable)[0];
            }
            else {
                //Debug.Log("skill type is : [ "+skill.skillType+" ] ,and Equals heal :"+skill.skillType.Equals("heal"));
                mainTarget = this.GetRandAggro(!self.isPlayable);
            }
            BattleManager.skillMainTarget = mainTarget;
            expectedResponse = range;
            if(mainTarget != self)
                expectedResponse += 1;
        }
        else { // this is for manual targeting
            BattleManager.skillMainTarget = mainTarget;
            expectedResponse = range;
            if(mainTarget != self)
                expectedResponse += 1;            
        }
        //Debug.Log("Target :" + BattleManager.skillMainTarget.character.getName() + " ally: " + BattleManager.skillMainTarget.isPlayable);

        // disable or enable ui and sets layer masks
        if(!self.isPlayable) {
            foreach(BattleEnemyHpGUI.TargetButton tb in enemyTargetGUI.targetButtons)
                tb.Disable();
            foreach(BattleEnemyHpGUI.TargetButton tbd in allyTargetGUI.targetButtons)
                tbd.Disable();
            SetCamLayerMask(skill, mainTarget.count);
        }
        else {
            if(skill.skillType.Equals("damage")) {
                enemyTargetGUI.selectedIndex = mainTarget.count;
                for(int i = 0; i < enemyCharacters.Count; i++) {
                    cmTG.m_Targets[i].weight = (i == mainTarget.count) ? 0.07f : 0.01f;
                }
                foreach(BattleEnemyHpGUI.TargetButton tb in enemyTargetGUI.targetButtons)
                    tb.UpdateSelected();
                foreach(BattleEnemyHpGUI.TargetButton tbd in allyTargetGUI.targetButtons)
                    tbd.Disable();
                SetCamLayerMask(skill, self.count);
            }
            else if (skill.skillType.Equals("heal")) {
                allyTargetGUI.selectedIndex = mainTarget.count;
                for(int i = 0; i < allyCharacters.Count; i++) {
                    cmTGallies.m_Targets[i].weight = (i == mainTarget.count) ? 5f : 0.01f;
                }
                cmTGallies.m_Targets[0].weight = 33f;
                foreach(BattleEnemyHpGUI.TargetButton tb in allyTargetGUI.targetButtons)
                    tb.UpdateSelected();
                foreach(BattleEnemyHpGUI.TargetButton tbd in enemyTargetGUI.targetButtons)
                    tbd.Disable();
                //SetCamLayerMask(skill, 0);
                ResetCamLayerMask();
            }
            
        }
        SetCamPosition();
        return mainTarget;
    }

    public void DisableTargets() {
        foreach(BattleEnemyHpGUI.TargetButton tb in enemyTargetGUI.targetButtons)
            tb.Disable();
        foreach(BattleEnemyHpGUI.TargetButton tb in allyTargetGUI.targetButtons)
            tb.Disable();
    }

    // 카메라 LayerMask 설정
    private void SetCamLayerMask(SkillInterface skill, int charPosNumb) {
        if(skill.skillType.Equals("damage")) {
            // 오른쪽에 있는 캐릭터들 전부 렌더링에서 제외
            int posToSwitch = charPosNumb - 1;
            while(posToSwitch >= 0) {
                String off = "allyPos"+posToSwitch;
                // Turn off the bit using an AND operation with the complement of the shifted int:
                mCamera.cullingMask &=  ~(1 << LayerMask.NameToLayer(off));
                posToSwitch--;
            }
        }
        // heal & reset
        else {
            for(int i = 0; i<4; i++) {
                String on = "allyPos"+i;
                // Turn on the bit using an OR operation:
                mCamera.cullingMask |= 1 << LayerMask.NameToLayer(on);
            }
        }
    }

    void ResetCamLayerMask() {
        for(int i = 0; i<4; i++) {
                String on = "allyPos"+i;
                // Turn on the bit using an OR operation:
                mCamera.cullingMask |= 1 << LayerMask.NameToLayer(on);
            }
    }

    // 카메라 위치 설정
    // 카메라가 바라보는 부분이 캐릭터와 타겟의 중간지점보다 캐릭터에 조금더 가깝게 설정.
    private void SetCamPosition() {

    }

    // 어그로에 따라 타겟 설정
    public CharacterBattle GetRandAggro(bool isAllyTarget) {
        List<CharacterBattle> targetList = isAllyTarget ? allyCharacters : enemyCharacters;
        float[] aggroList = new float[targetList.Count + 1];
        aggroList[0] = 0;
        foreach(CharacterBattle target in targetList)
            aggroList[targetList.IndexOf(target)+1] = aggroList[targetList.IndexOf(target)] + target.Aggro;
        float randVal = UnityEngine.Random.Range(0, aggroList[aggroList.Length-1]);
        return targetList[MidBinarySearch(aggroList, randVal)];
    }

    // mid point binary search
    public int MidBinarySearch(float[] aggroList, float val) {
        foreach(float f in aggroList) {
            if(f > val) return Array.IndexOf(aggroList, f)-1;
        }
        return -1;
    }
    
    public void TurnOver(string caller) {
        //Debug.Log("called by " + caller);
        turnOverResponses++;
        if(turnOverResponses == expectedResponse) {
            BattleManager.skillCaster.charGO.GetComponent<Animator>().SetBool("turn", false);
            //Debug.Log("Turn Over! Ended Turn: " + gameCurrentTurn + ". Response recieved: " + turnOverResponses + " Expected: " + expectedResponse);
            isTurnOver = true;
            BattleManager.ChangeCanLerpBar(true);
            turnOverResponses = 0;
            gameCurrentTurn++;
        }
    }

    public bool isBattleOver() {
        //return false;
        if(enemyCharacters.Count == 0 || allyCharacters.Count == 0) {
            //Debug.Log("Battle Over!");
            return true;
        }
        return false;
    }
}
