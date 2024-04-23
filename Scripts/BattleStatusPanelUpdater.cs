using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class BattleStatusPanelUpdater : MonoBehaviour
{
    CharacterInterface character;
    [SerializeField] public UnityEngine.UI.Slider hpSlider;
    [SerializeField] public UnityEngine.UI.Slider shieldSlider;
    [SerializeField] public UnityEngine.UI.Button ultButton;
    public bool canUpdate;

    // Update is called once per frame
    void Update()
    {
        UpdateStatus();
    }

    void UpdateStatus() {
        if(!canUpdate) return;
        hpSlider.value = character.getHp() / character.getMaxHp();
        shieldSlider.value = character.getShield() / character.getMaxHp();
    }
}
