using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    private RenderTexture rT;
    [SerializeField] private GameObject charModelHolder;
    [SerializeField] private RawImage rTrawImage;
    [SerializeField] private Dropdown drDownChar;
    [SerializeField] private Camera rTcam;
    [SerializeField] private Text statsText;
    [SerializeField] private Button addToTeam;
    [SerializeField] private GameObject panelB;
    [SerializeField] private Text skillTextBox;
    [SerializeField] private Button closeTeamSettingsButton;
    [SerializeField] private GameObject TeamSettingsPanel;
    [SerializeField] private Button startGameButton;
    private GameObject memberPanelPlaceHolder;
    private GameObject panelPartyHolder;
    private GameObject panelTargetHolder;
    private Text debugText;
    public GameObject draggedPanel;
    private int teamMaxSize = 4;
    private int teamSize = 0;
    private List<int> teamIds = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        DataManager.readJson();
        DataManager.ReadSkillJson();



        

        DataManager.allyParties[0] = new Party();

        memberPanelPlaceHolder = Resources.Load<GameObject>("Placeholders/Menu UI placeholders/TeamSetup_MemberPlaceholder");
        panelPartyHolder = panelB.transform.Find("ForCharPanels").Find("panelPartyHolder").gameObject;
        panelTargetHolder = panelB.transform.Find("ForCharPanels").Find("panelTargetHolder").gameObject;
        Button saveButton = panelB.transform.Find("Save Team button").GetComponent<Button>();
        debugText = panelB.transform.Find("DebugPanel").Find("Scroll View").Find("Viewport").Find("Text").GetComponent<Text>();
        
        saveButton.onClick.AddListener(delegate {
            //debugText.text = "";
            int pS = panelPartyHolder.transform.childCount;
            if(pS < 4) {
                debugText.text += "You Must have 4 characters in your party.\n";
                return;
            }
            CharacterInterface[] playableAgents = new CharacterInterface[pS];
            for(int i = 0; i < pS; i++) {
                int id = panelPartyHolder.transform.GetChild(i).GetComponent<DragPanel>().charId;
                debugText.text += "- adding "+DataManager.userInfo.userChars[id].charName + " to party...\n";
                int partyNumber = panelPartyHolder.transform.GetChild(i).GetComponent<CharPanelUpdatePos>().target.transform.GetSiblingIndex();
                playableAgents[partyNumber] = new Character(DataManager.userInfo.userChars[id].Clone());
                debugText.text += "- Success!\n";
                
            }
            Party allyParty = new Party();
            allyParty.SetParty(playableAgents);
            DataManager.currentParty = 0;
            DataManager.allyParties[DataManager.currentParty] = allyParty;
            debugText.text += "- Party saved! You may exit now.\n";
            startGameButton.interactable = true;
        });

        startGameButton.onClick.AddListener(delegate {
            CharacterInterface[] tenemies = new CharacterInterface[4];
            tenemies[0] = new Character(DataManager.userInfo.enemyChars[0].Clone());
            tenemies[1] = new Character(DataManager.userInfo.enemyChars[1].Clone());
            tenemies[2] = new Character(DataManager.userInfo.enemyChars[2].Clone());
            tenemies[3] = new Character(DataManager.userInfo.enemyChars[0].Clone());            
            tenemies[3].addSpeed(-.1f);
            Party enemyParty = new Party();
            enemyParty.SetParty(tenemies);
            DataManager.enemyParty = enemyParty;
            DataManager.debug = false;
            UnityEngine.SceneManagement.SceneManager.LoadScene("BattleScene");
        });
        
        rT = new RenderTexture(450, 550, 24);
        rTcam.orthographicSize = .9f;
        rTcam.targetTexture = rT;
        rTrawImage.texture = rT;
        drDownChar.onValueChanged.AddListener(delegate {
            ChangeCharacter();
            if(teamIds.Contains(drDownChar.value) || teamSize >= teamMaxSize) addToTeam.interactable = false;
            else addToTeam.interactable = true;

        });
        addToTeam.onClick.AddListener(delegate {
            teamIds.Add(drDownChar.value);
            GameObject memberPanelP = Instantiate(memberPanelPlaceHolder, panelPartyHolder.transform);
            memberPanelP.GetComponent<DragPanel>().charId = drDownChar.value;
            memberPanelP.transform.Find("CharacterImage").Find("RawImage").GetComponent<RawImage>().texture = Resources.Load<Texture>("Images/Characters/"+DataManager.userInfo.userChars[drDownChar.value % DataManager.userInfo.userChars.Length].charName+"_nobg");
            GameObject memberPanelTarget = new GameObject("target");
            memberPanelTarget.transform.SetParent(panelTargetHolder.transform);
            memberPanelTarget.AddComponent<RectTransform>();
            memberPanelTarget.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 100);
            memberPanelP.GetComponent<CharPanelUpdatePos>().target = memberPanelTarget;
            teamSize++;
            // if(teamSize >= teamMaxSize) addToTeam.interactable = false;
            addToTeam.interactable = false;

            memberPanelP.transform.Find("Remove From Team Button").GetComponent<Button>().onClick.AddListener(delegate {
                teamIds.Remove(memberPanelP.GetComponent<DragPanel>().charId);
                Destroy(memberPanelP.GetComponent<CharPanelUpdatePos>().target);
                Destroy(memberPanelP);
                teamSize--;
                if(teamSize < teamMaxSize && !teamIds.Contains(drDownChar.value) ) addToTeam.interactable = true;
            });

        });

        if (closeTeamSettingsButton == null) debugText.text += "-<!>- closeTeamSettingsButton is null!!!!\n";
        closeTeamSettingsButton.onClick.AddListener(delegate {
            if(charModelHolder.transform.childCount > 0)
                Destroy(charModelHolder.transform.GetChild(0).gameObject);
            TeamSettingsPanel.SetActive(false);
        });
    }


    public void ChangeCharacter() {
        
        if(charModelHolder.transform.childCount > 0)
            Destroy(charModelHolder.transform.GetChild(0).gameObject);
        CharStats charSt = DataManager.userInfo.userChars[drDownChar.value % DataManager.userInfo.userChars.Length];
        string name = charSt.charName;
        GameObject charModel = Resources.Load<GameObject>("Prefabs/Characters/"+name);
        GameObject charModelInstance = Instantiate(charModel, charModelHolder.transform);
        charModelInstance.transform.localPosition = new Vector3(0, 0, 0);
        
        
        try {
            rTcam.transform.position = new Vector3(0, 0, 1) + charModelInstance.transform.Find("Armature").transform.Find("Hips").transform.position;
            rTcam.transform.LookAt(charModelInstance.transform.Find("Armature").transform.Find("Hips").transform.position);
        } catch {
            rTcam.transform.position = new Vector3(0, 0, 1) + charModelInstance.transform.Find("TargetHP").transform.position.y/2 * Vector3.up;
            rTcam.transform.LookAt(charModelInstance.transform.GetChild(0).transform.Find("Hip").transform.position);
        }
        rTcam.orthographicSize = .2f + charModelInstance.transform.Find("StateDrivenCamera").transform.position.y;
        if(statsText == null) return;
        statsText.text = "Name: " + name + "\n" +
                        "Level: " + charSt.charLevel + "\n" +
                        "Rank: " + charSt.charRank + "\n" +
                        "Speed: " + charSt.speed + "\n" +
                        "Attack: " + charSt.atk + "\n" +
                        "Defense: " + charSt.def + "\n" +
                        "Max HP: " + charSt.maxHP + "\n" +
                        "type: " + charSt.type;
        skillTextBox.text = "";
        for(int i = 1; i < charSt.charSkills.Length; i++) {
            skillTextBox.text += "Ultimate Skill : "+ DataManager.allSkills.ults[charSt.charSkills[0]].skillType+ "\nult factor: "+DataManager.allSkills.ults[charSt.charSkills[0]].skillFactor+"\n";
            skillTextBox.text += "Normal Skill 1 : "+ DataManager.allSkills.normals[charSt.charSkills[1]].normgroup[0].skillType+"\nskill1 factor: "+DataManager.allSkills.normals[charSt.charSkills[1]].normgroup[0].skillFactor+"\n";
            skillTextBox.text += "Normal Skill 2 : "+ DataManager.allSkills.normals[charSt.charSkills[1]].normgroup[1].skillType+"\nskill2 factor: "+DataManager.allSkills.normals[charSt.charSkills[1]].normgroup[1].skillFactor+"\n";
        }

    }

    // Update is called once per frame
    void Update()
    {
        rTcam.transform.RotateAround(charModelHolder.transform.position, Vector3.up, 20 * Time.deltaTime);
    }
}
