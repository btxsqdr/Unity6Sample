// Created by Andre Rene Biasi on 2024/08/01.
// Copyright (c) 2024 Andre Rene Biasi. All rights reserved.

using UnityEngine;

namespace ARB.TextureLoader
{
    [CreateAssetMenu(fileName = nameof(TextureLoaderSettings), menuName = "TextureLoader/Settings", order = 1)]
    public class TextureLoaderSettings : ScriptableObject
    {
        public LogLevel LogLevel = TextureLoader.DefaultLogLevel;

        [Space()]

        public TextureSettings TextureDefaults;

        [Space()]

        public FullCacheSettings CacheDefaults;

        private void OnValidate()
        {
            TextureDefaults.MaxSize = Mathf.Clamp(TextureDefaults.MaxSize, TextureDefaults.MinSize, 8192);
        }
    }
}
