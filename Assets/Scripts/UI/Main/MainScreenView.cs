using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity6Sample {
    public class MainScreenView : BaseView, IScreenView {
        [SerializeField] UIDocument _document;
        [SerializeField] StyleSheet _styleSheet;
        [SerializeField] VisualTreeAsset _mainView;
        [SerializeField] VisualTreeAsset _productCellView;
        
        private void Start() {
            Preconditions.CheckNotNull(_document);
            Preconditions.CheckNotNull(_styleSheet);
            Preconditions.CheckNotNull(_mainView);
            Preconditions.CheckNotNull(_productCellView);
            
            StartCoroutine(Generate());
        }

        private void OnValidate() {
            if (Application.isPlaying) return;
            
            StartCoroutine(Generate());
        }
        
        private IEnumerator Generate() {
            yield return null;
            
            var root = _document.rootVisualElement;
            root.Clear();
            root.styleSheets.Add(_styleSheet);

            var mainView = _mainView.Instantiate();
            
            var listViewRoot = mainView.Q<ListView>("product_list");
            listViewRoot.makeItem = () => _productCellView.Instantiate();
            listViewRoot.bindItem = (cell, index) => {
                cell.Q<Label>("product_name").text = $"Product {index}";
                cell.Q<Button>("product_button").clickable.clicked += () => {
                    Debug.Log($"open product detail {index}");
                };
            };
            listViewRoot.itemsSource = new string[10];
            
            root.Add(mainView);
        }

        // private void OnValidate() {
        //     // _storeController
        // }

        // private void Start() {
        //     Debug.Assert(_document, "_document is null", this);
        //     Debug.Assert(_styleSheet, "_styleSheet is null", this);
        //
        //     StartCoroutine(Generate());
        // }

        // private void OnValidate() {
        //     if (Application.isPlaying) return;
        //     StartCoroutine(Generate());
        // }
        //
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