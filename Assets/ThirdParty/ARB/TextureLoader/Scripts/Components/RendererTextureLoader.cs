// Created by Andre Rene Biasi on 2024/09/01.
// Copyright (c) 2024 Andre Rene Biasi. All rights reserved.

using UnityEngine;

namespace ARB.TextureLoader
{
    /// <summary>
    /// Attach this to a GameObject with a Renderer component to load a texture into it.
    /// </summary>
    [AddComponentMenu("TextureLoader/Renderer Texture Loader")]
    [RequireComponent(typeof(Renderer))]
    public class RendererTextureLoader : TargetTextureLoader
    {
        private Renderer textureRenderer;

        private TextureTarget target;
        protected override TextureTarget Target => target;

        protected override void Awake()
        {
            textureRenderer = GetComponent<Renderer>();
            target = new RendererTextureTarget(textureRenderer, defaultTexture, loadingTexture, errorTexture, fadeTime);
            base.Awake();
        }
    }
}
