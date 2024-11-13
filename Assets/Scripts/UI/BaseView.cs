using UnityEngine;
using UnityEngine.UIElements;

namespace Unity6Sample {
    public class BaseView : MonoBehaviour {
        protected VisualElement Create(params string[] classNames) {
            return Create<VisualElement>(classNames);
        }

        protected T Create<T>(params string[] classNames) where T : VisualElement, new() {
            var ele = new T();
            foreach (var className in classNames) {
                ele.AddToClassList(className);
            }

            return ele;
        }
    }
}