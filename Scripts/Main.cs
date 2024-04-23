using UnityEngine;

public class Main : MonoBehaviour
{
    // Start is called before the first frame update
    Backend be;

    void Start()
    {
        
    }

    void init() {
        be = new Backend();
        // 디버깅 목적을 위해 로드 아이디 비번은 고정
        be.load();
        be.createTree(new BST<Character>());
    }

    void debugInit() {
        be = new Backend();
        // 디버깅 목적을 위해 로드 아이디 비번은 고정
        be.load();
        be.createTree(new RBT<Character>());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
