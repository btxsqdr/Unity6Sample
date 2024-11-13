using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity6Sample {
    public class MainScreenView : BaseView, IScreenView {
        [SerializeField] private UIDocument _document;
        [SerializeField] private StyleSheet _styleSheet;

        private void Start() {
            Debug.Assert(_document, "_document is null", this);
            Debug.Assert(_styleSheet, "_styleSheet is null", this);

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

            // logo, title and on the right settings button
            var navBar = Create("nav-bar");
            root.Add(navBar);
            
            // dynamic content area
            var contentBox = Create("content-box");

            // scrollview
            var scrollBox = Create("scroll-box");
            
            // a product cell
            var productCell = Create("product-cell");
            scrollBox.Add(productCell);
            
            contentBox.Add(scrollBox);

            root.Add(contentBox);
        }
    }
}