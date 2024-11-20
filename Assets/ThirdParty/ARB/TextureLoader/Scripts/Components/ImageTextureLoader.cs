// Created by Andre Rene Biasi on 2024/09/01.
// Copyright (c) 2024 Andre Rene Biasi. All rights reserved.

using UnityEngine;
using UnityEngine.UI;

namespace ARB.TextureLoader
{
    /// <summary>
    /// Attach this to a GameObject with a Image component to load a texture into it.
    /// </summary>
    [AddComponentMenu("TextureLoader/Image Texture Loader")]
    [RequireComponent(typeof(Image))]
    public class ImageTextureLoader : UGUITextureLoader
    {
        private Image textureRenderer;
        private TextureTarget target;
        protected override TextureTarget Target => target;

        protected override void Awake()
        {
            textureRenderer = GetComponent<Image>();
            target = new ImageTextureTarget(textureRenderer, defaultTexture, loadingTexture, errorTexture, scaleMode, fadeTime);
            base.Awake();
        }
    }
}
