using System;
using System.Collections;
using System.Collections.Generic;
using ARB.TextureLoader;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity6Sample {
    public class MainScreenView : BaseView, IScreenView {
        [SerializeField] UIDocument _document;
        [SerializeField] StyleSheet _styleSheet;
        [SerializeField] VisualTreeAsset _mainView;
        [SerializeField] VisualTreeAsset _productCellView;
        
        public event Action<EpicProduct> OnProductClick;

        private List<EpicProduct> _productList = new();
        
        private void Start() {
            this.IsNull(_document, _styleSheet, _mainView, _productCellView);
            
            StartCoroutine(Generate());
        }

        private void OnValidate() {
            if (Application.isPlaying) return;
            
            StartCoroutine(Generate());
        }
        
        public void UpdateList(IList<EpicProduct> list) {
            Debug.Log($"UpdateList called with {list.Count} items");

            _productList = new List<EpicProduct>(list); //list as List<EpicProduct>;
            
            StartCoroutine(Generate());
        }
        
        private IEnumerator Generate() {
            yield return null;
            
            var root = _document.rootVisualElement;
            root.Clear();
            root.styleSheets.Add(_styleSheet);

            var mainView = _mainView.Instantiate();
            
            var listViewRoot = mainView.Q<ListView>("product_list");
            listViewRoot.selectionType = SelectionType.Single;
            listViewRoot.makeItem = () => _productCellView.Instantiate();
            listViewRoot.bindItem = (cell, index) => {
                try {
                    EpicProduct product = _productList[index];

                    cell.Q<Label>("product_name").text = $"{product.title}";
                    cell.Q<Button>("product_button").clickable.clicked += () => OnProductClick?.Invoke(product);
                    cell.Q<VisualElement>("cell_header").style.AsyncLoadBackgroundImage(product.thumbnail);
                    
                } catch (Exception e) {
                    Debug.LogError(e);
                }
            };
            listViewRoot.itemsSource = _productList;
            
            root.Add(mainView);
            
            listViewRoot.Rebuild();
            
            yield return null;
        }

        // private IEnumerator Generate() {
        //     yield return null;
        //
        //     var root = _document.rootVisualElement;
        //     root.Clear();
        //     root.styleSheets.Add(_styleSheet);
        //
        //     // logo, title and on the right settings button
        //     var navBar = Create("nav-bar");
        //     root.Add(navBar);
        //     
        //     // dynamic content area
        //     var contentBox = Create("content-box");
        //
        //     // scrollview
        //     var scrollBox = Create("scroll-box");
        //     
        //     // a product cell
        //     var productCell = Create("product-cell");
        //     scrollBox.Add(productCell);
        //     
        //     contentBox.Add(scrollBox);
        //
        //     root.Add(contentBox);
        // }
    }
}