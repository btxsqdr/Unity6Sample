using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Unity6Sample {
  
  public class Game : MonoBehaviour {
    
    void Start() {
      EpicStoreClient.FetchEpicStoreRaw((data) => {
        Debug.Log($"Data fetched from Epic Store: {data?.Count}");
      });
    }
  }
}