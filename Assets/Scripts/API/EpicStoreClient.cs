using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
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
  
  [Serializable]
  public struct EpicProduct {
    public string id;
    public string title;
    public string description;
    public string offerType;
    public string offerImageWide;
    public string offerImageTall;
    public string thumbnail;
  }

  public class EpicStoreClient {
    private static readonly string EPIC_STORE_URL = "https://store.epicgames.com/en-US/";
    
    public static void FetchEpicStoreRaw(Action<ObservableList<EpicProduct>> callback) {
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
    }

    private static ObservableList<EpicProduct> ParseEpicContentForProducts(string content) {
      if (string.IsNullOrEmpty(content)) throw new NullReferenceException("content cannot be null");

      string pattern = @"""offer"":{""title"":""(?'title'.*?)"".*?""id"":""(?'id'[\w\d]*?)"".*?""offerType"":""(?'offerType'.*?)"".*?""description"":""(?'description'.*?)"".*?""OfferImageWide"",""url"":""(?'OfferImageWide'.*?)""}.*?""OfferImageTall"",""url"":""(?'OfferImageTall'.*?)""}.*?""Thumbnail"",""url"":""(?'Thumbnail'.*?)""}";
      RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Multiline;

      try {
        ObservableList<EpicProduct> products = new ();
        foreach (Match m in Regex.Matches(@content, pattern, options)) {
          string title = m.Groups["title"].Value;
          string id = m.Groups["id"].Value;
          string description = m.Groups["description"].Value;
          string offerType = m.Groups["offerType"].Value;
          string offerImageWide = m.Groups["OfferImageWide"].Value;
          string offerImageTall = m.Groups["OfferImageTall"].Value;
          string thumbnail = m.Groups["Thumbnail"].Value;

          // take only BASE GAME
          if (!offerType.Contains("BASE_GAME")) continue;
          
          thumbnail = HttpUtility.UrlDecode(offerImageWide);
          offerImageTall = HttpUtility.UrlDecode(offerImageTall);
          offerImageWide = HttpUtility.UrlDecode(thumbnail);
          
          EpicProduct product = new EpicProduct {
            id = Regex.Unescape(id),
            title = Regex.Unescape(title),
            description = Regex.Unescape(description),
            offerType = Regex.Unescape(offerType),
            offerImageWide = Regex.Unescape(offerImageWide),
            offerImageTall = Regex.Unescape(offerImageTall),
            thumbnail = Regex.Unescape(thumbnail)
          };
          
          // Debug.Log($"title {title}");
          
          products.Add(product);
        }
        return products;  
      } catch (Exception e) {
        Debug.LogError(e);
      }

      return null;
    }
  }

}