// Created by Andre Rene Biasi on 2024/09/01.
// Copyright (c) 2024 Andre Rene Biasi. All rights reserved.

using System.IO;
using UnityEditor;
using UnityEngine;

namespace ARB.TextureLoader
{
    /// <summary>
    /// Generates an Editor menu containing helper actions for the <see cref="TextureLoader"/> class.
    /// </summary>
    internal class TextureLoaderEditor
    {
        private const string MenuPath = "Window/TextureLoader/";
        private const string ResourcesPath = "Assets/Resources";
        private const string SettingsPath = "Assets/Resources/TextureLoaderSettings.asset";

        [MenuItem(MenuPath + "Settings", priority = 10000)]
        private static void RevealSettings()
        {
            Directory.CreateDirectory(ResourcesPath);

            TextureLoaderSettings asset = AssetDatabase.LoadAssetAtPath<TextureLoaderSettings>(SettingsPath);

            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<TextureLoaderSettings>();
                AssetDatabase.CreateAsset(asset, SettingsPath);
                AssetDatabase.SaveAssets();
            }

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }

        [MenuItem(MenuPath + "Reveal Cache Folder", priority = 10001)]
        private static void RevealCacheFolder()
        {
            TextureLoaderSettings settings = Resources.Load<TextureLoaderSettings>(nameof(TextureLoaderSettings));

            if (settings != null)
            {
                string path = settings.CacheDefaults.Directory == DirectoryType.Persistent ? Application.persistentDataPath : Application.temporaryCachePath;
                path += "/";

                if (!string.IsNullOrEmpty(settings.CacheDefaults.Subdirectory))
                {
                    path += settings.CacheDefaults.Subdirectory.Trim();
                }

                path = path.EndsWith("/") ? path : path + "/";

                Directory.CreateDirectory(path);
                Application.OpenURL("file://" + path);
                settings = null;
            }
        }

        [MenuItem(MenuPath + "Clear Cache Folder", priority = 10002)]
        private static void ClearCacheFolder()
        {
            TextureLoader.ClearCache();
        }
    }
}