using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PracticeScript : MonoBehaviour
{
    [Range(0.1f, 1f)] public float smoothing = 1f;
    public Vector3 Target {
        get {return target;}
        set {
            target = value;
            StopCoroutine("MyCoroutine");
            StartCoroutine("MyCoroutine", target);
        }
    }
    private Vector3 target;

    // yield return null 으로 다음 프레임에서 이 함수가 다시 실행됨
    IEnumerator MyCoroutine(Vector3 target) {
        while (Vector3.Distance(transform.position, target) > 0.05f) {
            transform.position = Vector3.Lerp(transform.position, target, smoothing * Time.deltaTime);
            yield return null; // if return null, coroutine will be executed in the next frame
        }

        print("Reached the target.");
        //yield return new WaitForSeconds(3f);
        //print("MyCoroutine is now finished.");
        
    }
}
