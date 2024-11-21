using System;
using UnityEngine;

namespace Unity6Sample {
    public class UIManager : MonoBehaviour {
        [SerializeField] MainScreenView mainScreenView;
        
        MainScreenController mainScreenController;

        private void Awake() {
            mainScreenController = new MainScreenController.Builder().Build(mainScreenView);
        }

        private void Update() => mainScreenController.Update(Time.deltaTime);
    }
}