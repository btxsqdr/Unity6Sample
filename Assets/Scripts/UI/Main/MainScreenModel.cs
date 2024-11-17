using UnityEngine;

namespace Unity6Sample {
  public class MainScreenModel {
    private void FetchEpicStoreData() {
      EpicStoreClient.FetchEpicStoreRaw((data) => {
        Debug.Log($"Data fetched from Epic Store: {data?.Count}");
      });
    }
  }
}