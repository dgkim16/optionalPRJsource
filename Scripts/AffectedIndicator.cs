using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AffectedIndicator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Initialize(int factor, Color color) {
        this.Initialize(factor, 1f, color);
    }

    public void Initialize(float factor, Color color) {
        this.Initialize((int)(factor), 1f, color);
    }

    public void Initialize(int factor, float duration, Color color) {
        this.factor = factor;
        this.duration = duration;
        this.color = color;
        this.textObject = this.transform.Find("text").gameObject;
        this.textObject.transform.localPosition += new Vector3(20.0f * Mathf.Sin(Random.Range(0, 360)), 20.0f * Mathf.Cos(Random.Range(0, 360)), 0);
        this.textObject.GetComponent<TextMeshProUGUI>().text = factor.ToString();
        this.textObject.GetComponent<TextMeshProUGUI>().fontSize = Random.Range(15,40);
        this.textObject.GetComponent<TextMeshProUGUI>().color = color;
        this.stop = false;
        BattleManager.battle.destroyListTurnEnd.Add(this.gameObject);
    }

    private int factor;
    private float duration;
    private Color color;
    private GameObject textObject;
    private float timeElapsed = 0;
    private bool stop = true;

    // Update is called once per frame
    void Update()
    {
        if(stop) return;
        timeElapsed += Time.fixedDeltaTime;
        if(timeElapsed >= duration) {
            color.a -= 0.01f;
            this.textObject.GetComponent<TextMeshProUGUI>().color = color;
        }
        if(color.a <= 0) {
            stop = true;
            //BattleManager.destroyListTurnEnd.Remove(this.gameObject);
            //GameObject.Destroy(this.gameObject);
        }
    }
}
