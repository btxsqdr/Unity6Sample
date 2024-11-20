using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Unity6Sample {
  public class MainScreenModel {
    private ObservableList<EpicProduct> Products {
      get => _products;
      set {
        _products = value;
        OnProductListChange?.Invoke(_products);
      }
    }
    private ObservableList<EpicProduct> _products = new();

    public event Action<IList<EpicProduct>> OnProductListChange;

    public MainScreenModel() {
      Products.AnyValueChanged += (list) => OnProductListChange?.Invoke(list);
    }
    
    public void FetchEpicStoreData() {
      EpicStoreClient.FetchEpicStoreRaw((data) => {
        Debug.Log($"FetchEpicStoreData... {data.Count}");
        
        Products = data;
        
        // foreach (var item in data) _products.Add(item);
      });
    }
  }
}