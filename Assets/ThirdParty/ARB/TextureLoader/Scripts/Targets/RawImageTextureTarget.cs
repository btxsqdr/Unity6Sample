// Created by Andre Rene Biasi on 2024/08/01.
// Copyright (c) 2024 Andre Rene Biasi. All rights reserved.

using UnityEngine;
using UnityEngine.UI;

namespace ARB.TextureLoader
{
    public class RawImageTextureTarget : UGUITextureTarget
    {
        protected readonly RawImage rawImage;

        protected override Texture CurrentTexture
        {
            get => rawImage != null ? rawImage.texture : null;
            set
            {
                if (rawImage == null) return;
                rawImage.texture = value;
                if (value != null) ResizeToFit();
            }
        }

        public RawImageTextureTarget(RawImage target, ScaleMode scaleMode = ScaleMode.None, float fadeTime = 0f)
            : base(target, scaleMode, fadeTime)
        {
            rawImage = target;
        }

        public RawImageTextureTarget(RawImage target, Texture2D defaultPlaceholder, Texture2D loadingPlaceholder, Texture2D errorPlaceholder, ScaleMode scaleMode = ScaleMode.None, float fadeTime = 0f)
            : base(target, defaultPlaceholder, loadingPlaceholder, errorPlaceholder, scaleMode, fadeTime)
        {
            rawImage = target;
        }

        public RawImageTextureTarget(RawImage target, CanvasGroup defaultPlaceholder, CanvasGroup loadingPlaceholder, CanvasGroup errorPlaceholder, ScaleMode scaleMode = ScaleMode.None, float fadeTime = 0f)
            : base(target, defaultPlaceholder, loadingPlaceholder, errorPlaceholder, scaleMode, fadeTime)
        {
            rawImage = target;
        }

        public override void Dispose()
        {
            base.Dispose();
            rawImage.texture = null;
        }
    }
}
