using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class GetApiData : MonoBehaviour
{
    public string URL;
    UserInfo userInfo;

    public UserInfo GetData(string id, string pw) {
        StartCoroutine(FetchUserData(id, pw));
        return userInfo;
    }
    public IEnumerator FetchUserData(string id, string pw) {  
        using (UnityWebRequest request = UnityWebRequest.Get(URL + "id=" + id + "&pw=" + pw)) {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError) {
                Debug.Log(request.error);
            } else {
                this.userInfo = JsonUtility.FromJson<UserInfo>(request.downloadHandler.text);
            }
        }
    }
}
