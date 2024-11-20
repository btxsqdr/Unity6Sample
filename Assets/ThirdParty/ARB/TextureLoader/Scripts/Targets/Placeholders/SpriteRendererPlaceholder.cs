// Created by Andre Rene Biasi on 2024/08/01.
// Copyright (c) 2024 Andre Rene Biasi. All rights reserved.

using ARB.TextureLoader.Extensions;
using UnityEngine;

namespace ARB.TextureLoader
{
    /// <summary>
    /// Defines a Sprite Renderer placeholder.
    /// </summary>
    public class SpriteRendererPlaceholder : Placeholder
    {
        protected readonly SpriteRenderer spriteRenderer;

        public SpriteRendererPlaceholder(SpriteRenderer spriteRenderer)
        {
            this.spriteRenderer = spriteRenderer;
            SetActive(false);
        }

        public override bool IsActive => spriteRenderer != null && spriteRenderer.gameObject.activeSelf;

        public override void SetActive(bool value)
        {
            if (spriteRenderer != null) spriteRenderer.gameObject.SetActive(value);
        }

        public override void SetAlpha(float value)
        {
            if (spriteRenderer != null) spriteRenderer.color = spriteRenderer.color.WithAlpha(value);
        }

        public override void Dispose()
        {
            if (spriteRenderer != null) GameObject.Destroy(spriteRenderer.gameObject);
        }
    }
}
