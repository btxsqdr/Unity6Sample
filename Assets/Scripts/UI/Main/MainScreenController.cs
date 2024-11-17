using System;
using UnityEngine;

namespace Unity6Sample {

    public interface IViewController {
        void ConnectModel();
        void ConnectView();
    }

    public class MainScreenController : MonoBehaviour, IViewController {
        [SerializeField] MainScreenView view;
        private readonly MainScreenModel model;

        private void Start() {
            ConnectModel();
            ConnectView();
        }

        private void Update() {
            
        }

        public void ConnectModel() {
            
        }

        public void ConnectView() {
            
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