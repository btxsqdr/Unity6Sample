using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Unity6Sample {
  [Serializable]
  public class User {
    public string id;
    public string name;
    public string createdAt;
    public int attack;
    public int defense;
    public int hp;
  }

  public class Game : MonoBehaviour {
    private readonly string MOCKAPI_BASE_URL = "https://6731b5a67aaf2a9aff11acdf.mockapi.io/api/v1";
    
    void Start() {
      StartCoroutine(FetchEpicStoreData());
      
      // string url = MOCKAPI_BASE_URL + "/Users";
      // StartCoroutine(FetchData(url));
    }

    IEnumerator FetchEpicStoreData() {
      // data scrap epic store 
      APIClient.FetchEpicStoreRaw();

      yield return null;
    }
    
    IEnumerator FetchData(string url) {
      using (UnityWebRequest request = UnityWebRequest.Get(url)) {
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError) {
          Debug.LogError(request.error);
        }
        else {
          try {
            User[] user = JsonConvert.DeserializeObject<User[]>(request.downloadHandler.text);

            Debug.Log($"user: name: {user?.First()?.name}");
          }
          catch (Exception e) {
            Debug.LogError(e);
          }
        }
      }
    }
  }
}