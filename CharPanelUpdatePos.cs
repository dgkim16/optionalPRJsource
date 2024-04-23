using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteAlways]
public class CharPanelUpdatePos : MonoBehaviour
{
    public GameObject target;

    // Update is called once per frame
    void Update()
    {
        
        float t = 0.1f;
        if(target != null) {
            this.GetComponent<RectTransform>().position = new Vector3(
                Mathf.Lerp(this.GetComponent<RectTransform>().position.x, target.GetComponent<RectTransform>().position.x, t),
            Mathf.Lerp(this.GetComponent<RectTransform>().position.y, target.GetComponent<RectTransform>().position.y, t),
            Mathf.Lerp(this.GetComponent<RectTransform>().position.z, target.GetComponent<RectTransform>().position.z, t));
            t += Time.deltaTime;
        }

    }
}
