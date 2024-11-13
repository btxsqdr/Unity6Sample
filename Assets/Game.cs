using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Unity6Sample
{
  [Serializable]
  public class User
  {
    public string id;
    public string name;
    public string createdAt;
    public int attack;
    public int defense;
    public int hp;
  }

  public class Game : MonoBehaviour
  {
    private const string BASE_URL = "https://6731b5a67aaf2a9aff11acdf.mockapi.io/api/v1";

    List<int> pingList = new List<int>();

    void Start()
    {
      string url = BASE_URL + "/Users";
      StartCoroutine(FetchData(url));

      // StartCoroutine(PingUpdate());
    }

    IEnumerator FetchData(string url)
    {
      using (UnityWebRequest request = UnityWebRequest.Get(url))
      {
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
          Debug.LogError(request.error);
        }
        else
        {
          try
          {
            Debug.Log(request.downloadHandler.text);
            
            User user = JsonUtility.FromJson<User>(request.downloadHandler.text);

            Debug.Log($"user: name: {user.name}");
          }
          catch (Exception e)
          {
            Debug.LogError(e);
          }
        }
      }
    }

    IEnumerator PingUpdate()
    {
      yield return new WaitForSeconds(1f);
      var ping = new Ping("142.250.191.46");
      yield return new WaitForSeconds(1f);
      while (!ping.isDone) yield return null;

      Debug.Log(ping.time);
      pingList.Add(ping.time);
    }
  }
}