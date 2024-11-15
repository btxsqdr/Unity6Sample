using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Proyecto26;
using UnityEngine;
using RestSharp;
using RestClient = RestSharp.RestClient;

namespace Unity6Sample {
  public class EpicStoreNoResultsException : Exception {
    public EpicStoreNoResultsException() {
      Debug.LogError("Epic Store does not return results.");
    }
  }

  public class EpicStoreClient {
    private static readonly string EPIC_STORE_URL = "https://store.epicgames.com/en-US/";
    
    public static void FetchEpicStoreRaw(Action<Dictionary<string, EpicProduct>> callback) {
      Proyecto26.RestClient.Request(new RequestHelper {
        Uri = EPIC_STORE_URL,
        Method = "GET",
        Timeout = 5,
        ContentType = "text/plain",
        Headers = {{ "User-Agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_11_6) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 Safari/537.36" }}
      }).Then(response => {
        callback?.Invoke(ParseEpicContentForProducts(response.Text));
      }).Catch(err => {
        Debug.LogError(err);
        callback?.Invoke(null);
      });
      
      // var client = new RestClient(new RestClientOptions(EPIC_STORE_URL) {
      //   Timeout = TimeSpan.FromSeconds(5),
      //   UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_11_6) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 Safari/537.36"
      // });
      // var request = new RestRequest();
      // request.Method = Method.Get;
      // request.AddHeader("Accept", "text/plain");
      // request.AddHeader("Content-Type", "text/plain");
      //
      // var response = client.ExecuteGet(request);
      //
      // Debug.Log($"response: {response.StatusCode}: {response.Content}");
      //
      // ParseEpicContentForProducts(response.Content);
      //
      // Debug.Log("FetchEpicStoreRaw end");

    }

    private static Dictionary<string, EpicProduct> ParseEpicContentForProducts(string content) {
      if (string.IsNullOrEmpty(content)) throw new NullReferenceException("content cannot be null");

      string pattern = @"""offer"":{""title"":""(?'title'.*?)"".*?""id"":""(?'id'[\w\d]*?)"".*?""offerType"":""(?'offerType'.*?)"".*?""description"":""(?'description'.*?)"".*?""OfferImageWide"",""url"":""(?'OfferImageWide'.*?)""}";
      RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Multiline;

      try {
        Dictionary<string, EpicProduct> products = new Dictionary<string, EpicProduct>();
        foreach (Match m in Regex.Matches(@content, pattern, options)) {
          string title = m.Groups["title"].Value;
          string id = m.Groups["id"].Value;
          string description = m.Groups["description"].Value;
          string offerType = m.Groups["offerType"].Value;
          string offerImageWide = m.Groups["OfferImageWide"].Value;

          EpicProduct product = new EpicProduct {
            id = id,
            title = title,
            description = description,
            offerType = offerType,
            offerImageWide = offerImageWide
          };
          
          // Debug.Log($"title {title}");
          
          products.Add(id, product);
        }
        return products;  
      } catch (Exception e) {
        Debug.LogError(e);
      }

      return null;
    }
  }

  [Serializable]
  public struct EpicProduct {
    public string id;
    public string title;
    public string description;
    public string offerType;
    public string offerImageWide;
  }
}