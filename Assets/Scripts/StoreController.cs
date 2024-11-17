using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity6Sample {
    public class StoreController {
        private readonly VisualElement _root;
        private readonly VisualTreeAsset _uxmlTreeAsset;
        private readonly StyleSheet _styleSheet;

        public StoreController(VisualElement root, VisualTreeAsset uxmlTreeAsset, StyleSheet styleSheet) {
            if (root is null) throw new ArgumentNullException(nameof(root));
            if (uxmlTreeAsset is null) throw new ArgumentNullException(nameof(uxmlTreeAsset));
            
            _root = root;
            _uxmlTreeAsset = uxmlTreeAsset;
            _styleSheet = styleSheet;
        }
        
        public void OnValidate() {
            _root.Clear();
            _uxmlTreeAsset.CloneTree(_root);
            _root.styleSheets.Add(_styleSheet);
        }
    }
}