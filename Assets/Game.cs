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
    
    void Start() {
      StartCoroutine(FetchEpicStoreData());
    }

    IEnumerator FetchEpicStoreData() {
        // data scrap epic store 
        EpicStoreClient.FetchEpicStoreRaw((data) => {
            Debug.Log($"Data fetched from Epic Store: {data?.Count}");
        });

        yield return null;
    }
    
    
  }
}