// Created by Andre Rene Biasi on 2024/08/01.
// Copyright (c) 2024 Andre Rene Biasi. All rights reserved.

using System;
using UnityEngine;

namespace ARB.TextureLoader
{
    [Serializable]
    public class CacheSettings
    {
        /// <summary>
        /// Whether the loaded texture should be cached on disk.
        /// </summary>
        [Tooltip("Whether the loaded texture should be cached on disk.")]
        public bool Enabled = TextureLoader.DefaultUseCache;

        /// <summary>
        /// The file format to be used when caching the loaded texture.
        /// </summary>
        [Tooltip("The file format to be used when caching the loaded texture.")]
        public FileFormat Format = TextureLoader.DefaultCacheFormat;

        /// <summary>
        /// The quality setting (0-100) for caching the loaded texture when using lossy formats like JPG.
        /// </summary>
        [Tooltip("The quality setting (0-100) for caching the loaded texture when using lossy formats like JPG.")]
        [Range(1, 100)]
        public int Quality = TextureLoader.DefaultCacheQuality;

        /// <summary>
        /// The days until the cached texture needs to be deleted and retrieved from the source again. Use a negative number for an infinite duration.
        /// </summary>
        [Tooltip("Days until the cached texture needs to be deleted and retrieved from the source again. Use a negative number for an infinite duration.")]
        public int Days = TextureLoader.DefaultCacheDuration.Days;

        /// <summary>
        /// Gets the time span until the cached texture needs to be deleted and retrieved from the source again.
        /// </summary>
        public TimeSpan Duration => Days < 0 ? TimeSpan.MaxValue : TimeSpan.FromDays(Days);

        public CacheSettings() { }
    }

    [Serializable]
    public class FullCacheSettings : CacheSettings
    {
        /// <summary>
        /// The directory where the loaded texture will be stored.
        /// </summary>
        [Tooltip("The directory where the loaded texture will be stored.")]
        public DirectoryType Directory = TextureLoader.DefaultCacheDirectory;

        /// <summary>
        /// The subdirectory where the loaded texture will be stored.
        /// </summary>
        [Tooltip("The subdirectory where the loaded texture will be stored.")]
        public string Subdirectory = TextureLoader.DefaultCacheSubdirectory;

        public FullCacheSettings() { }

    }
}
