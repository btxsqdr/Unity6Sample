using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity6Sample {

    public interface IViewController {
        void ConnectModel();
        void ConnectView();
    }

    public class MainScreenController : MonoBehaviour, IViewController {
        [SerializeField] MainScreenView view;
        private readonly MainScreenModel model = new();

        private void Start() {
            this.IsNull(view, model);
            
            ConnectModel();
            ConnectView();
        }

        public void ConnectModel() {
            model.OnProductListChange += list => UpdateView(list);
            model.FetchEpicStoreData();
        }
        
        public void ConnectView() {
            view.OnProductClick += OnProductClick;
        }

        private void OnProductClick(EpicProduct product) {
            throw new NotImplementedException();
        }

        private void UpdateView(IList<EpicProduct> list) {
            view.UpdateList(list);
        }

        // public class Builder {
        //     private readonly MainScreenModel model = new MainScreenModel();
        //
        //     public Builder Builder() {
        //         return this;
        //     }
        //     
        //     public MainScreenController Build() {
        //         
        //     }
        // }
    }
}