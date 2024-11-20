// Created by Andre Rene Biasi on 2024/08/01.
// Copyright (c) 2024 Andre Rene Biasi. All rights reserved.

using System;
using UnityEngine;

namespace ARB.TextureLoader
{
    [Serializable]
    public class TextureSettings
    {
        /// <summary>
        /// Minimum texture size (in pixels) allowed. An error will be returned if the loaded texture doesn't meet this value.
        /// </summary>
        [Tooltip("Minimum texture size (in pixels) allowed. An error will be returned if the loaded texture doesn't meet this value.")]
        [Range(0, 8192)]
        public int MinSize = TextureLoader.DefaultMinSize;

        /// <summary>
        /// Maximum texture size (in pixels) allowed. The loaded texture will be resized (preserving aspect ratio) if it exceeds this value.
        /// </summary>
        [Tooltip("Maximum texture size (in pixels) allowed. The loaded texture will be resized (preserving aspect ratio) if it exceeds this value.")]
        [Range(0, 8192)]
        public int MaxSize = TextureLoader.DefaultMaxSize;

        /// <summary>
        /// Whether a mipmap chain should be generated for the loaded texture.
        /// </summary>
        [Tooltip("Whether a mipmap chain should be generated for the loaded texture.")]
        public bool MipmapChain = TextureLoader.DefaultMipmapChain;

        /// <summary>
        /// Whether the loaded texture should be readable by the CPU.
        /// <br/>https://docs.unity3d.com/ScriptReference/Texture-isReadable.html
        /// </summary>
        [Tooltip("Whether the loaded texture should be readable by the CPU.")]
        public bool Readable = TextureLoader.DefaultReadable;

        /// <summary>
        /// Wrap mode to be used for the loaded texture.
        /// <br/>https://docs.unity3d.com/ScriptReference/TextureWrapMode.html
        /// </summary>
        [Tooltip("Wrap mode to be used for the loaded texture.")]
        public TextureWrapMode WrapMode = TextureLoader.DefaultWrapMode;

        /// <summary>
        /// Filter mode to be used for the loaded texture.
        /// <br/>https://docs.unity3d.com/ScriptReference/Texture-filterMode.html
        /// </summary>
        [Tooltip("Filter mode to be used for the loaded texture.")]
        public FilterMode FilterMode = TextureLoader.DefaultFilterMode;

        /// <summary>
        /// Anisotropic filtering level to be used for the loaded texture.
        /// <br/>https://docs.unity3d.com/ScriptReference/Texture-anisoLevel.html
        /// </summary>
        [Tooltip("Anisotropic filtering level to be used for the loaded texture.")]
        [Range(0, 16)]
        public int AnisoLevel = TextureLoader.DefaultAnisoLevel;

        /// <summary>
        /// Compression setting to be used for the loaded texture.
        /// <br/>https://docs.unity3d.com/ScriptReference/Texture2D.Compress.html
        /// </summary>
        [Tooltip("Compression setting to be used for the loaded texture.")]
        public TextureCompression Compression = TextureLoader.DefaultCompression;

#if UNITY_6000_0_OR_NEWER
        /// <summary>
        /// Linear color space setting to be used for the loaded texture.
        /// <br/>https://docs.unity3d.com/6000.0/Documentation/Manual/linear-color-space.html
        /// </summary>
        [Tooltip("Linear color space setting to be used for the loaded texture.")]
        public bool LinearColorSpace = TextureLoader.DefaultLinearColorSpace;
#endif

        public TextureSettings() { }
    }
}
