// Created by Andre Rene Biasi on 2024/09/01.
// Copyright (c) 2024 Andre Rene Biasi. All rights reserved.

using UnityEngine;

namespace ARB.TextureLoader
{
    /// <summary>
    /// Attach this to a GameObject with a SpriteRenderer component to load a texture into it.
    /// </summary>
    [AddComponentMenu("TextureLoader/Sprite Renderer Texture Loader")]
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteRendererTextureLoader : TargetTextureLoader
    {
        private SpriteRenderer textureRenderer;
        private TextureTarget target;
        protected override TextureTarget Target => target;

        protected override void Awake()
        {
            textureRenderer = GetComponent<SpriteRenderer>();
            target = new SpriteRendererTextureTarget(textureRenderer, defaultTexture, loadingTexture, errorTexture, fadeTime);
            base.Awake();
        }
    }
}
