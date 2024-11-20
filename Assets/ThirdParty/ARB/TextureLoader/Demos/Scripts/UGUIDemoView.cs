using System.Collections;
using System.Collections.Generic;
using ARB.TextureLoader.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ARB.TextureLoader.Demos
{
    /// <summary>
    /// View that demonstrates the usage of <see cref="TextureLoader"/> with Unity's UGUI system.
    /// </summary>
    public class UGUIDemoView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI infoText;

        [SerializeField]
        private Button settingsButton;

        [SerializeField]
        private GameObject settingsPanel;

        [SerializeField]
        private Button loadButton;

        [SerializeField]
        private Image loadButtonIcon;

        [SerializeField]
        private Sprite loadButtonIconSprite;

        [SerializeField]
        private Sprite loadButtonPauseIconSprite;

        [SerializeField]
        private ScrollRect scrollRect;

        [SerializeField]
        private GalleryImageView[] thumbnails;

        [SerializeField]
        private GalleryImageView image;

        private bool isLoading;
        private int loadedCount = 0;
        private int errorCount = 0;

        private void OnValidate()
        {
            thumbnails = GetComponentsInChildren<GalleryImageView>();
        }

        private void OnEnable()
        {
            foreach (GalleryImageView thumbnail in thumbnails)
            {
                thumbnail.OnSelected += OnGalleryThumbnailSelected;
            }
        }

        private IEnumerator Start()
        {
            loadButton.onClick.AddListener(OnLoadButtonClick);
            scrollRect.onValueChanged.AddListener(OnScrollRectValueChanged);
            yield return null;
            LoadThumbnails();
        }

        private void OnDisable()
        {
            foreach (GalleryImageView thumbnail in thumbnails)
            {
                thumbnail.OnSelected -= OnGalleryThumbnailSelected;
            }
        }

        private void OnDestroy()
        {
            UnloadThumbnails();
            UnloadImage();
        }

        private void OnRectTransformDimensionsChange()
        {
            LoadPartiallyVisibleThumbnails();
            ResizeLoadedThumbnails();
        }

        private void SetInfoText(string value)
        {
            infoText.text = value;
        }

        private void LoadImage(string url)
        {
            image.Loader?.Dispose();
            image.Loader = TextureLoader.Load(url)
                .Into(image.Image, image.DefaultPlaceholder, image.LoadingPlaceholder, image.ErrorPlaceholder, UGUITextureTarget.ScaleMode.ScaleToCover, SettingsView.FadeTime);
            image.Loader.Start();
            image.gameObject.SetActive(true);
        }

        private void UnloadImage()
        {
            image.Loader?.Dispose();
            image.gameObject.SetActive(false);
        }

        private void LoadThumbnails()
        {
            UnloadImage();
            UnloadThumbnails();
            SetLoadButtonIcon(loadButtonPauseIconSprite);

            isLoading = true;
            loadedCount = 0;

            int index = 10;
            int size = 1024;

            foreach (GalleryImageView thumbnail in thumbnails)
            {
                index++;

                thumbnail.Loader = TextureLoader.Load($"https://picsum.photos/id/{index}/{size}/{size}")
                    .Into(thumbnail.Image, thumbnail.DefaultPlaceholder, thumbnail.LoadingPlaceholder, thumbnail.ErrorPlaceholder, UGUITextureTarget.ScaleMode.ScaleToCover, SettingsView.FadeTime)
                    .OnComplete(OnThumbnailLoadComplete)
                    .OnError(OnThumbnailLoadError);

                if (thumbnail.Image.rectTransform.IsPartiallyInside((RectTransform)scrollRect.transform))
                {
                    thumbnail.Loader.Start(SettingsView.Queue);
                }
            }
        }

        private void LoadPartiallyVisibleThumbnails()
        {
            foreach (GalleryImageView thumbnail in thumbnails)
            {
                if (thumbnail.Loader != null && thumbnail.Image.rectTransform.IsPartiallyInside((RectTransform)scrollRect.transform))
                {
                    thumbnail.Loader.Start(SettingsView.Queue);
                }
            }
        }

        private void ResizeLoadedThumbnails()
        {
            foreach (GalleryImageView image in thumbnails)
            {
                List<UGUITextureTarget> targets = image.Loader?.GetTargets<UGUITextureTarget>();

                if (targets == null) continue;

                foreach (UGUITextureTarget target in targets)
                {
                    target?.ResizeToFit();
                }
            }
        }

        private void UnloadThumbnails()
        {
            isLoading = false;
            SetLoadButtonIcon(loadButtonIconSprite);

            foreach (GalleryImageView thumbnail in thumbnails)
            {
                thumbnail.Loader?.Dispose();
            }
        }

        private void CancelLoadingThumbnails()
        {
            isLoading = false;
            SetLoadButtonIcon(loadButtonIconSprite);

            foreach (GalleryImageView thumbnail in thumbnails)
            {
                thumbnail.Loader?.Cancel();
            }
        }

        private void SetLoadButtonIcon(Sprite sprite)
        {
            if (loadButtonIcon != null) loadButtonIcon.sprite = sprite;
        }

        private void UpdateLoadButtonAndInfoText()
        {
            if (loadedCount + errorCount == thumbnails.Length)
            {
                isLoading = false;
                SetLoadButtonIcon(loadButtonIconSprite);
            }

            SetInfoText($"Images loaded: {loadedCount}/{thumbnails.Length}");
        }

        private void OnLoadButtonClick()
        {
            if (isLoading)
            {
                CancelLoadingThumbnails();
                return;
            }

            LoadThumbnails();
        }

        private void OnScrollRectValueChanged(Vector2 value)
        {
            LoadPartiallyVisibleThumbnails();
        }

        private void OnThumbnailLoadComplete(Texture2D _)
        {
            loadedCount++;
            UpdateLoadButtonAndInfoText();
        }

        private void OnThumbnailLoadError(string _)
        {
            errorCount++;
            UpdateLoadButtonAndInfoText();
        }

        private void OnGalleryThumbnailSelected(string url)
        {
            LoadImage(url);
        }
    }
}
