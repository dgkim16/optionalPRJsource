using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragPanel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public Settings settings;
    public BoxCollider boxCollider;
    public int charId;
    // Start is called before the first frame update
    void Start()
    {
        settings = GameObject.Find("AllSettings").GetComponent<Settings>();
        boxCollider = this.gameObject.GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    

    public void OnPointerDown(PointerEventData eventData) {
        //Debug.Log("Mouse Down from "+ this.gameObject.name);
        Debug.Log("Mouse Down from "+ this.gameObject.transform.GetSiblingIndex());
        settings.draggedPanel = this.gameObject;
        this.gameObject.GetComponent<CharPanelUpdatePos>().enabled = false;
        boxCollider.isTrigger = false;

    }

    public void OnPointerUp(PointerEventData eventData) {
        //Debug.Log("Mouse Up from "+ this.gameObject.name);
        settings.draggedPanel = null;
        this.gameObject.GetComponent<CharPanelUpdatePos>().enabled = true;
        boxCollider.isTrigger = true;
    }

    public void OnDrag(PointerEventData eventData) {

        
        this.gameObject.GetComponent<RectTransform>().position += new Vector3(0,eventData.delta.y);
        
    }

    public void OnTriggerEnter(Collider other) {
        if(settings.draggedPanel == this.gameObject) return;
        Debug.Log("Collided with "+ other.gameObject.transform.GetSiblingIndex());
        GameObject temp = other.gameObject.GetComponent<CharPanelUpdatePos>().target;
        other.gameObject.GetComponent<CharPanelUpdatePos>().target = this.gameObject.GetComponent<CharPanelUpdatePos>().target;
        this.gameObject.GetComponent<CharPanelUpdatePos>().target = temp;
    }


}
