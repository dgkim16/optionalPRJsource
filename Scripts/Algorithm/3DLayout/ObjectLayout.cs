using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


// {e1} [fixed] current version problem : foes are facing the other way.

//[ExecuteAlways]
public class ObjectLayout : MonoBehaviour
{
    public Transform[] childTransforms;
    public Transform PositionParent;
    public Transform IndicatorParent;
    public Transform otherPos;
    //public Vector3[] camPositions;
    //public bool debug = false;
    //public int debugInt = 0;
    //public GameObject cam;

    public int count;
    [SerializeField] public float boxWidth;
    [SerializeField] public float padding;
    [SerializeField] public float width;
    private Vector3 pos1;
    private Vector3 pos2;

    public void SetUp(int count, Transform otherPos) {
        this.count = count;
        this.otherPos = otherPos;
        
        PositionParent = this.transform.Find("Party Positions");
        IndicatorParent = this.transform.Find("Party Indicators (debug)");
        if(count == 0) count = 1;
        if(count > 1) {
            for(int i = 1; i < count; i++) {
                GameObject newPos = Instantiate(PositionParent.Find("0").gameObject);
                GameObject newInd = Instantiate(IndicatorParent.Find("0").gameObject);
                newPos.transform.SetParent(PositionParent);
                newInd.transform.SetParent(IndicatorParent);
                newPos.name = i.ToString();
                newInd.name = i.ToString();
                newPos.transform.localScale = new Vector3(1,1,1);
                UnityEngine.Animations.ConstraintSource source = new UnityEngine.Animations.ConstraintSource();
                source.sourceTransform = newPos.transform;
                source.weight = 1;
                newInd.GetComponent<UnityEngine.Animations.PositionConstraint>().SetSource(0, source);
            }
        }

        if(width / count <= boxWidth)
            width = (boxWidth + 0.3f) * count;

        Transform pos = PositionParent;
        pos1 = pos.localPosition + pos.right * (width/2 + padding);
        pos2 = pos.localPosition - pos.right * (width/2 + padding);
        childTransforms = PositionParent.gameObject.GetComponentsInChildren<Transform>();
        for(int i = 0; i < childTransforms.Length; i++) {
            if(childTransforms[i] == pos) continue;
            childTransforms[i].localPosition = Vector3.Lerp(pos1, pos2, (float)i / (float)(childTransforms.Length));
            childTransforms[i].localRotation = Quaternion.Euler(0, 0, 0);
            // {e1} fixed by adding this line:
        }
        /*
        camPositions = new Vector3[count];
        for(int i = 0; i < count; i++) {
            Transform posT = childTransforms[i+1];
            camPositions[i] = posT.position - posT.forward * 3.5f - (posT.forward * 0.08f * i) + (posT.right * (2.3f - i * 0.4f)) + posT.up * 1.02f;
            GameObject camIndicator = new GameObject("camIndicator_"+i);
            camIndicator.transform.position = camPositions[i];
            camIndicator.transform.SetParent(this.transform.Find("CamFollow"));
        }
        */
    }
/*
    void Update() {
        if(debug) CamChange();
    }

    void CamChange() {
        if(debugInt >= 0 && debugInt < count) {
            cam.transform.position = camPositions[debugInt];
            debug = false;
        }
    }
    */
}
