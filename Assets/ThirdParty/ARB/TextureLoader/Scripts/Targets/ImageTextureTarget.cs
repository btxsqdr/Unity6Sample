// Created by Andre Rene Biasi on 2024/08/01.
// Copyright (c) 2024 Andre Rene Biasi. All rights reserved.

using UnityEngine;
using UnityEngine.UI;

namespace ARB.TextureLoader
{
    public class ImageTextureTarget : UGUITextureTarget
    {
        private readonly Image image;

        protected override Texture CurrentTexture
        {
            get => image != null && image.sprite != null && image.sprite.texture != null ? image.sprite.texture : null;
            set
            {
                if (image == null) return;
                image.sprite = value == null ? null : Sprite.Create(value as Texture2D, new Rect(0f, 0f, value.width, value.height), Vector2.one * 0.5f);
                if (value != null) ResizeToFit();
            }
        }

        public ImageTextureTarget(Image target, ScaleMode scaleMode = ScaleMode.None, float fadeTime = 0f)
            : base(target, scaleMode, fadeTime)
        {
            image = target;
        }

        public ImageTextureTarget(Image target, Texture2D defaultPlaceholder, Texture2D loadingTexture, Texture2D errorTexture, ScaleMode scaleMode = ScaleMode.None, float fadeTime = 0f)
            : base(target, defaultPlaceholder, loadingTexture, errorTexture, scaleMode, fadeTime)
        {
            image = target;
        }

        public ImageTextureTarget(Image target, CanvasGroup defaultPlaceholder, CanvasGroup loadingPlaceholder, CanvasGroup errorPlaceholder, ScaleMode scaleMode = ScaleMode.None, float fadeTime = 0f)
            : base(target, defaultPlaceholder, loadingPlaceholder, errorPlaceholder, scaleMode, fadeTime)
        {
            image = target;
        }

        public override void Dispose()
        {
            base.Dispose();
            image.sprite = null;
        }
    }
}
