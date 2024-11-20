// Created by Andre Rene Biasi on 2024/08/01.
// Copyright (c) 2024 Andre Rene Biasi. All rights reserved.

using UnityEngine;
using UnityEngine.UIElements;

namespace ARB.TextureLoader
{
    /// <summary>
    /// Defines a UIToolkit texture target.
    /// </summary>
    public class UIToolkitTextureTarget : TextureTarget
    {
        protected readonly VisualElement target;
        protected Image image;
        protected Image fader;
        protected bool shouldDisposePlaceholders;
        private readonly Length length = Length.Percent(100);

        public UIToolkitTextureTarget(VisualElement target, ScaleMode scaleMode, float fadeTime = 0)
            : base(fadeTime)
        {
            this.target = target;
            Initialize(target, scaleMode, fadeTime);
            shouldDisposePlaceholders = false;
        }

        public UIToolkitTextureTarget(VisualElement target, Texture2D defaultPlaceholder, Texture2D loadingPlaceholder, Texture2D errorPlaceholder, ScaleMode scaleMode, float fadeTime = 0)
            : base(fadeTime)
        {
            this.target = target;
            Initialize(target, scaleMode, fadeTime);
            shouldDisposePlaceholders = true;

            if (defaultPlaceholder != null)
            {
                this.defaultPlaceholder = new UIToolkitPlaceholder(CreateImage("DefaultPlaceholder", 0, scaleMode, defaultPlaceholder));
            }

            if (loadingPlaceholder != null)
            {
                this.loadingPlaceholder = new UIToolkitPlaceholder(CreateImage("LoadingPlaceholder", 0, scaleMode, loadingPlaceholder));
            }

            if (errorPlaceholder != null)
            {
                this.errorPlaceholder = new UIToolkitPlaceholder(CreateImage("ErrorPlaceholder", 0, scaleMode, errorPlaceholder));
            }
        }

        public UIToolkitTextureTarget(VisualElement target, VisualElement defaultPlaceholder, VisualElement loadingPlaceholder, VisualElement errorPlaceholder, ScaleMode scaleMode, float fadeTime = 0)
            : base(fadeTime)
        {
            this.target = target;
            Initialize(target, scaleMode, fadeTime);
            shouldDisposePlaceholders = false;

            if (defaultPlaceholder != null)
            {
                this.defaultPlaceholder = new UIToolkitPlaceholder(defaultPlaceholder);
            }

            if (loadingPlaceholder != null)
            {
                this.loadingPlaceholder = new UIToolkitPlaceholder(loadingPlaceholder);
            }

            if (errorPlaceholder != null)
            {
                this.errorPlaceholder = new UIToolkitPlaceholder(errorPlaceholder);
            }
        }

        protected void Initialize(VisualElement target, ScaleMode scaleMode, float fadeTime)
        {
            if (fadeTime > 0)
            {
                fader = CreateImage("Fader", 0, scaleMode);
            }

            if (target is Image)
            {
                image = (Image)target;
                image.scaleMode = scaleMode;
            }
            else
            {
                image = CreateImage("Image", 0, scaleMode);
            }
        }

        protected override Texture CurrentTexture
        {
            get => image != null ? image.image : null;
            set
            {
                if (image != null) image.image = value;
            }
        }

        public override void SetAlpha(float alpha)
        {
            if (image != null) image.style.opacity = alpha;
        }

        public override void Dispose()
        {
            if (fader != null) target.Remove(fader);

            if (shouldDisposePlaceholders)
            {
                defaultPlaceholder?.Dispose();
                loadingPlaceholder?.Dispose();
                errorPlaceholder?.Dispose();
            }
        }

        private Image CreateImage(string name, float opacity, ScaleMode scaleMode, Texture2D texture = null)
        {
            Image image = new()
            {
                name = name,
                scaleMode = scaleMode,
                image = texture,
                style =
                {
                    opacity = opacity,
                    position = Position.Absolute,
                    width = length,
                    height = length
                },
            };

            target.Insert(0, image);

            return image;
        }

        protected override void SetFaderTexture(Texture texture)
        {
            if (fader != null) fader.image = texture;
        }

        protected override void SetFaderAlpha(float alpha)
        {
            if (fader != null) fader.style.opacity = alpha;
        }
    }
}
