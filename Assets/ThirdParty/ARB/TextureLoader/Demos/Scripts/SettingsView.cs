using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ARB.TextureLoader.Demos
{
    /// <summary>
    /// View that displays a panel to control the different <see cref="TextureLoader"/> settings.
    /// </summary>
    public class SettingsView : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField minSizeInputField;

        [SerializeField]
        private TMP_InputField maxSizeInputField;

        [SerializeField]
        private Toggle mipmapChainToggle;

        [SerializeField]
        private Toggle readableToggle;

        [SerializeField]
        private TMP_Dropdown wrapModeDropdown;

        [SerializeField]
        private TMP_Dropdown compressionDropdown;

        [SerializeField]
        private TMP_InputField fadeTimeInputField;

        [SerializeField]
        private Toggle queueToggle;

        [SerializeField]
        private Toggle cacheToggle;

        [SerializeField]
        private TMP_Dropdown directoryDropdown;

        [SerializeField]
        private TMP_InputField subdirectoryInputField;

        [SerializeField]
        private TMP_Dropdown fileFormatDropdown;

        [SerializeField]
        private TMP_InputField qualityInputField;

        [SerializeField]
        private Button clearCacheButton;

        public static bool Queue { get; private set; } = true;

        public static float FadeTime { get; private set; } = 0.15f;

        private void Awake()
        {
            wrapModeDropdown.ClearOptions();
            wrapModeDropdown.AddOptions(GetDropdownOptions<TextureWrapMode>());

            compressionDropdown.ClearOptions();
            compressionDropdown.AddOptions(GetDropdownOptions<TextureCompression>());

            directoryDropdown.ClearOptions();
            directoryDropdown.AddOptions(GetDropdownOptions<DirectoryType>());

            fileFormatDropdown.ClearOptions();
            fileFormatDropdown.AddOptions(GetDropdownOptions<FileFormat>());
        }

        private void OnEnable()
        {
            clearCacheButton.onClick.AddListener(OnClearCacheButtonClick);

            minSizeInputField.text = TextureLoader.DefaultMinSize.ToString();
            minSizeInputField.onEndEdit.AddListener(OnMinSizeInputFieldEndEdit);

            maxSizeInputField.text = TextureLoader.DefaultMaxSize.ToString();
            maxSizeInputField.onEndEdit.AddListener(OnMaxSizeInputFieldEndEdit);

            mipmapChainToggle.isOn = TextureLoader.DefaultMipmapChain;

            readableToggle.isOn = TextureLoader.DefaultReadable;

            wrapModeDropdown.value = (int)TextureLoader.DefaultWrapMode;

            compressionDropdown.value = (int)TextureLoader.DefaultCompression;

            fadeTimeInputField.text = "" + FadeTime;
            fadeTimeInputField.onEndEdit.AddListener(OnFadeTimeInputFieldEndEdit);

            queueToggle.isOn = Queue;
            queueToggle.onValueChanged.AddListener(OnQueueToggleValueChanged);

            cacheToggle.isOn = TextureLoader.DefaultUseCache;

            directoryDropdown.value = (int)TextureLoader.DefaultCacheDirectory;

            subdirectoryInputField.text = TextureLoader.DefaultCacheSubdirectory;

            fileFormatDropdown.value = (int)TextureLoader.DefaultCacheFormat;
            fileFormatDropdown.onValueChanged.AddListener(OnFileFormatDropdownValueChanged);

            qualityInputField.text = "" + TextureLoader.DefaultCacheQuality;
            qualityInputField.interactable = fileFormatDropdown.value == (int)FileFormat.JPG;
        }

        private void OnDisable()
        {
            if (clearCacheButton != null)
            {
                clearCacheButton.onClick.RemoveListener(OnClearCacheButtonClick);
            }

            if (minSizeInputField != null)
            {
                minSizeInputField.onEndEdit.RemoveListener(OnMinSizeInputFieldEndEdit);
            }

            if (maxSizeInputField != null)
            {
                maxSizeInputField.onEndEdit.RemoveListener(OnMaxSizeInputFieldEndEdit);
            }

            if (queueToggle != null)
            {
                queueToggle.onValueChanged.RemoveListener(OnQueueToggleValueChanged);
            }

            if (fileFormatDropdown != null)
            {
                fileFormatDropdown.onValueChanged.RemoveListener(OnFileFormatDropdownValueChanged);
            }

            ApplyChanges();
        }

        private void OnClearCacheButtonClick()
        {
            TextureLoader.ClearCache();
        }

        private void OnFileFormatDropdownValueChanged(int value)
        {
            qualityInputField.interactable = fileFormatDropdown.value == (int)FileFormat.JPG;
        }

        private void OnMinSizeInputFieldEndEdit(string value)
        {
            int min = Int32.Parse(minSizeInputField.text);
            minSizeInputField.text = "" + Mathf.Clamp(min, 0, 8192);
        }

        private void OnMaxSizeInputFieldEndEdit(string value)
        {
            int min = Int32.Parse(minSizeInputField.text);
            int max = Int32.Parse(value);
            maxSizeInputField.text = "" + Mathf.Clamp(max, min, 8192);
        }

        private void OnFadeTimeInputFieldEndEdit(string value)
        {
            FadeTime = Mathf.Clamp(float.Parse(value), 0, 60);
            fadeTimeInputField.text = "" + FadeTime;
        }

        private void OnQueueToggleValueChanged(bool value)
        {
            Queue = value;
        }

        private void ApplyChanges()
        {
            TextureLoader.DefaultMinSize = int.Parse(minSizeInputField.text);
            TextureLoader.DefaultMaxSize = int.Parse(maxSizeInputField.text);
            TextureLoader.DefaultMipmapChain = mipmapChainToggle.isOn;
            TextureLoader.DefaultReadable = readableToggle.isOn;
            TextureLoader.DefaultWrapMode = (TextureWrapMode)wrapModeDropdown.value;
            TextureLoader.DefaultCompression = (TextureCompression)compressionDropdown.value;
            TextureLoader.DefaultUseCache = cacheToggle.isOn;
            TextureLoader.DefaultCacheDirectory = (DirectoryType)directoryDropdown.value;
            TextureLoader.DefaultCacheSubdirectory = subdirectoryInputField.text;
            TextureLoader.DefaultCacheFormat = (FileFormat)fileFormatDropdown.value;
            TextureLoader.DefaultCacheQuality = int.Parse(qualityInputField.text);
        }

        private static List<TMP_Dropdown.OptionData> GetDropdownOptions<T>() where T : Enum
        {
            List<TMP_Dropdown.OptionData> options = new();

            foreach (T value in Enum.GetValues(typeof(T)))
            {
                options.Add(new(value.ToString()));
            }

            return options;
        }
    }
}
