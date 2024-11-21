using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity6Sample {

    public enum UIScreenEnum {
        MainScreen,
        ProductDetailScreen,
        SettingsScreen
    }

    public interface IViewController {
        void ConnectModel();
        void ConnectView();
        void Router(UIScreenEnum screen);
    }

    public class MainScreenController : IViewController {
        private readonly MainScreenView view;
        private readonly MainScreenModel model;

        MainScreenController(MainScreenView view, MainScreenModel model) {
            this.view = view;
            this.model = model;
            
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

        public void Update(float deltaTime) {
            
        }

        public void Router(UIScreenEnum screen) {
            
        }

        private void OnProductClick(EpicProduct product) {
            throw new NotImplementedException();
        }

        private void UpdateView(IList<EpicProduct> list) {
            view.UpdateList(list);
        }

        public class Builder {
            readonly MainScreenModel model = new();
        
            public MainScreenController Build(MainScreenView view) {
                return new MainScreenController(view, model);
            }
        }
    }
}