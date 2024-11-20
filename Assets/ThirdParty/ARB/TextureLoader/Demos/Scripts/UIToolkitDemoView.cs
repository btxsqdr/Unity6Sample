using UnityEngine;
using UnityEngine.UIElements;

namespace ARB.TextureLoader.Demos
{
    /// <summary>
    /// View that demonstrates the usage of <see cref="TextureLoader"/> with Unity's UIToolkit system.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class UIToolkitDemoView : MonoBehaviour
    {
        [SerializeField]
        private VisualTreeAsset imageTemplate;

        private void Start()
        {
            VisualElement root = GetComponent<UIDocument>().rootVisualElement;
            VisualElement imageGrid = root.Q<VisualElement>("grid");

            ScrollView scrollView = root.Q<ScrollView>("ScrollView");
            scrollView.AddManipulator(new ScrollViewPointerManipulator());

            int imageCount = 50;
            int size = 512;

            for (int i = 0; i < imageCount; i++)
            {
                TemplateContainer imageElement = imageTemplate.CloneTree();
                VisualElement imageContainer = imageElement.Q("container");
                VisualElement defaultPlaceholder = imageElement.Q("default-placeholder");
                VisualElement loadingPlaceholder = imageElement.Q("loading-placeholder");
                VisualElement errorPlaceholder = imageElement.Q("error-placeholder");

                TextureLoader.Load($"https://picsum.photos/id/{i}/{size}/{size}")
                    .Into(imageContainer, defaultPlaceholder, loadingPlaceholder, errorPlaceholder, ScaleMode.ScaleAndCrop, 0.3f);

                imageGrid.Add(imageElement);
            }

            TextureLoader.StartAll(true);
        }

        private void OnDestroy()
        {
            TextureLoader.DisposeAll();
        }
    }
}