using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickSetPosition : MonoBehaviour
{
    public PracticeScript coroutineScript;
    public GameObject targetIndicator;
    // Start is called before the first frame update
    void OnMouseDown()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Physics.Raycast(ray, out hit);
        if (hit.collider.gameObject == gameObject) {
            targetIndicator.transform.position = hit.point + new Vector3(0,.5f, 0);
            Vector3 newTarget = hit.point + new Vector3(0,.5f, 0);
            coroutineScript.Target = newTarget;
        }
    }
}
