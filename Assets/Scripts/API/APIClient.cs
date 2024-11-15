using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using Proyecto26;

namespace Unity6Sample {

  public class EpicStoreNoResultsException : Exception {
    public EpicStoreNoResultsException() {
      Debug.LogError("Epic Store does not return results.");
    }
  }

  public class APIClient {
    private static readonly string EPIC_STORE_URL = "https://store.epicgames.com/en-US/";
    
    public static async Task<Dictionary<string, EpicProduct>> FetchEpicStoreRaw() {
      string url = EPIC_STORE_URL;

      RestClient.Request(new RequestHelper {
        Uri = EPIC_STORE_URL,
        Method = "GET",
        Timeout = 5,
        ContentType = "text/plain",
        Headers = {{ "User-Agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_11_6) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 Safari/537.36" }}
      }).Then(response => {
        return ParseEpicContentForProducts(response.Text);
      }).Catch(err => {
        Debug.LogError(err);
      });
      return null;
    }

    private static Dictionary<string, EpicProduct> ParseEpicContentForProducts(string content) {
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
          
          Debug.Log($"title {title}");
          
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