using System;
using UnityEngine;
using UnityEngine.UI;

namespace ARB.TextureLoader.Demos
{
    public class GalleryImageView : MonoBehaviour
    {
        [SerializeField]
        private Button button;

        [SerializeField]
        private RawImage image;
        public RawImage Image => image;

        [SerializeField]
        private CanvasGroup defaultPlaceholder;
        public CanvasGroup DefaultPlaceholder => defaultPlaceholder;

        [SerializeField]
        private CanvasGroup loadingPlaceholder;
        public CanvasGroup LoadingPlaceholder => loadingPlaceholder;

        [SerializeField]
        private CanvasGroup errorPlaceholder;
        public CanvasGroup ErrorPlaceholder => errorPlaceholder;

        public TextureLoader Loader { get; set; }

        public Action<string> OnSelected;

        private void OnEnable()
        {
            button.onClick.AddListener(OnButtonClick);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            if (Loader != null && !string.IsNullOrEmpty(Loader.Url) && Loader.IsDone)
            {
                OnSelected?.Invoke(Loader.Url);
            }
        }
    }
}
