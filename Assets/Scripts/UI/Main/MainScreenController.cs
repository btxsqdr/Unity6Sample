using System;
using UnityEngine;

namespace Unity6Sample {

    public interface IViewController {
        void ConnectModelAndView();
    }

    public class MainScreenController : MonoBehaviour, IViewController {
        [SerializeField] MainScreenView view;
        private readonly MainScreenModel model = new MainScreenModel();

        private void Start() {
            Preconditions.CheckState(view);
            
            ConnectModelAndView();
        }

        private void Update() {
            
        }

        public void ConnectModelAndView() {
            
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