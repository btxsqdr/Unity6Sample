using ARB.TextureLoader;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity6Sample {
    public static class ToolkitExtensions {
        public static void AsyncLoadBackgroundImage(this IStyle style, string url) {
            TextureLoader
                .Load(url)
                .OnComplete((texture) => {
                    style.backgroundImage = texture;
                })
                .OnError(error => Debug.LogError($"{error} @ {url}"))
                .Start(enqueue: true);
        }
    }
}
