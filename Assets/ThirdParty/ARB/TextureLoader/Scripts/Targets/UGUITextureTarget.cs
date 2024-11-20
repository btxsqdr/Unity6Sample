// Created by Andre Rene Biasi on 2024/08/01.
// Copyright (c) 2024 Andre Rene Biasi. All rights reserved.

using ARB.TextureLoader.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace ARB.TextureLoader
{
    /// <summary>
    /// Base class for creating texture targets from UGUI objects.
    /// </summary>
    public abstract class UGUITextureTarget : TextureTarget
    {
        protected MaskableGraphic target;
        protected ScaleMode scaleMode;
        protected RectTransform parent;
        protected RawImage fader;

        protected readonly new UGUIPlaceholder defaultPlaceholder;
        protected readonly new UGUIPlaceholder loadingPlaceholder;
        protected readonly new UGUIPlaceholder errorPlaceholder;

        protected bool shouldDisposePlaceholders;

        public UGUITextureTarget(MaskableGraphic target, ScaleMode scaleMode = ScaleMode.None, float fadeTime = 0f)
            : base(fadeTime)
        {
            this.target = target;
            this.target.rectTransform.SetAnchor(RectTransformExtensions.Anchor.MiddleCenter);
            this.scaleMode = scaleMode;
            parent = target.transform.parent as RectTransform;
            shouldDisposePlaceholders = false;

            if (fadeTime > 0)
            {
                fader = CreateRawImage("Fader");
            }
        }

        public UGUITextureTarget(MaskableGraphic target, Texture2D defaultPlaceholder, Texture2D loadingPlaceholder, Texture2D errorPlaceholder, ScaleMode scaleMode = ScaleMode.None, float fadeTime = 0f)
            : base(fadeTime)
        {
            this.target = target;
            this.target.rectTransform.SetAnchor(RectTransformExtensions.Anchor.MiddleCenter);
            this.scaleMode = scaleMode;
            parent = target.transform.parent as RectTransform;
            shouldDisposePlaceholders = true;

            if (fadeTime > 0)
            {
                fader = CreateRawImage("Fader");
            }

            if (defaultPlaceholder != null)
            {
                this.defaultPlaceholder = new MaskableGraphicPlaceholder(CreateRawImage("DefaultPlaceholder", defaultPlaceholder));
                base.defaultPlaceholder = this.defaultPlaceholder;
            }

            if (loadingPlaceholder != null)
            {
                this.loadingPlaceholder = new MaskableGraphicPlaceholder(CreateRawImage("LoadingPlaceholder", loadingPlaceholder));
                base.loadingPlaceholder = this.loadingPlaceholder;
            }

            if (errorPlaceholder != null)
            {
                this.errorPlaceholder = new MaskableGraphicPlaceholder(CreateRawImage("ErrorPlaceholder", errorPlaceholder));
                base.errorPlaceholder = this.errorPlaceholder;
            }
        }

        public UGUITextureTarget(MaskableGraphic target, CanvasGroup defaultPlaceholder, CanvasGroup loadingPlaceholder, CanvasGroup errorPlaceholder, ScaleMode scaleMode = ScaleMode.None, float fadeTime = 0f)
            : base(fadeTime)
        {
            this.target = target;
            this.target.rectTransform.SetAnchor(RectTransformExtensions.Anchor.MiddleCenter);
            this.scaleMode = scaleMode;
            parent = target.transform.parent as RectTransform;
            shouldDisposePlaceholders = false;

            if (fadeTime > 0)
            {
                fader = CreateRawImage("Fader");
            }

            if (defaultPlaceholder != null)
            {
                this.defaultPlaceholder = new CanvasGroupPlaceholder(defaultPlaceholder);
                base.defaultPlaceholder = this.defaultPlaceholder;
            }

            if (loadingPlaceholder != null)
            {
                this.loadingPlaceholder = new CanvasGroupPlaceholder(loadingPlaceholder);
                base.loadingPlaceholder = this.loadingPlaceholder;
            }

            if (errorPlaceholder != null)
            {
                this.errorPlaceholder = new CanvasGroupPlaceholder(errorPlaceholder);
                base.errorPlaceholder = this.errorPlaceholder;
            }
        }

        public override void SetAlpha(float alpha)
        {
            if (target != null) target.color = target.color.WithAlpha(alpha);
        }

        public override void Dispose()
        {
            if (fader != null) GameObject.Destroy(fader.gameObject);

            if (shouldDisposePlaceholders)
            {
                defaultPlaceholder?.Dispose();
                loadingPlaceholder?.Dispose();
                errorPlaceholder?.Dispose();
            }
        }

        /// <summary>
        /// Adjusts the size to fit within the dimensions of its parent according to the current scaling mode.
        /// </summary>
        public virtual void ResizeToFit()
        {
            if (parent == null) return;

            if (target != null)
            {
                target.rectTransform.sizeDelta = target.mainTexture == null
                   ? parent.sizeDelta
                   : GetSizeDeltaByScaleMode(target.mainTexture, parent, scaleMode);
            }

            if (fader != null)
            {
                fader.rectTransform.sizeDelta = fader.mainTexture == null
                   ? parent.sizeDelta
                   : GetSizeDeltaByScaleMode(fader.mainTexture, parent, scaleMode);
            }

            if (defaultPlaceholder != null && defaultPlaceholder.Texture != null)
            {
                defaultPlaceholder.SizeDelta = GetSizeDeltaByScaleMode(defaultPlaceholder.Texture, parent, scaleMode);
            }

            if (loadingPlaceholder != null && loadingPlaceholder.Texture != null)
            {
                loadingPlaceholder.SizeDelta = GetSizeDeltaByScaleMode(loadingPlaceholder.Texture, parent, scaleMode);
            }

            if (errorPlaceholder != null && errorPlaceholder.Texture != null)
            {
                errorPlaceholder.SizeDelta = GetSizeDeltaByScaleMode(errorPlaceholder.Texture, parent, scaleMode);
            }
        }

        /// <summary>
        /// Adjusts the size to fit within the dimensions of its parent according to the given scaling mode.
        /// </summary>
        public virtual void ResizeToFit(ScaleMode scaleMode)
        {
            this.scaleMode = scaleMode;
            ResizeToFit();
        }

        private RawImage CreateRawImage(string name, Texture texture = null)
        {
            RawImage rawImage = new GameObject(name, typeof(RawImage)).GetComponent<RawImage>();
            rawImage.transform.SetParent(target.transform, false);
            rawImage.color = rawImage.color.WithAlpha(0f);
            rawImage.raycastTarget = false;

            if (texture != null)
            {
                rawImage.texture = texture;
                rawImage.rectTransform.sizeDelta = GetSizeDeltaByScaleMode(texture, parent, scaleMode);
            }

            rawImage.gameObject.SetActive(false);
            return rawImage;
        }

        protected override void SetFaderAlpha(float alpha)
        {
            if (fader != null) fader.color = fader.color.WithAlpha(alpha);
        }

        protected override void SetFaderTexture(Texture texture)
        {
            if (fader == null) return;
            fader.texture = texture;
            fader.gameObject.SetActive(texture != null);
        }

        protected Vector2 GetSizeDeltaByScaleMode(Texture texture, RectTransform parent, ScaleMode scaleMode)
        {
            if (texture == null || scaleMode == ScaleMode.StretchToFill) return parent.rect.size;

            if (scaleMode == ScaleMode.None) return new Vector2(texture.width, texture.height);

            float width = texture.width;
            float height = texture.height;
            float widthRatio = parent.rect.width / width;
            float heightRatio = parent.rect.height / height;
            float factor = 1f;

            if (scaleMode == ScaleMode.ScaleToFit || scaleMode == ScaleMode.ScaleDown)
            {
                factor = Mathf.Min(widthRatio, heightRatio);

                if (scaleMode == ScaleMode.ScaleDown)
                {
                    float noneSize = width * height;
                    float containWidth = factor * width;
                    float containHeight = factor * height;
                    float containSize = containWidth * containHeight;
                    return noneSize <= containSize ? new Vector2(width, height) : new Vector2(containWidth, containHeight);
                }
            }
            else if (scaleMode == ScaleMode.ScaleToCover)
            {
                factor = Mathf.Max(widthRatio, heightRatio);
            }

            return new Vector2(factor * width, factor * height);
        }

        public enum ScaleMode
        {
            /// <summary>
            /// The texture is not resized.
            /// </summary>
            None,

            /// <summary>
            /// Stretches the texture to fill the parent completely.
            /// </summary>
            StretchToFill,

            /// <summary>
            /// Scales the texture, maintaining aspect ratio, so it completely fits withing the parent.
            /// </summary>
            ScaleToFit,

            /// <summary>
            /// Scales the texture, maintaining aspect ratio, so it completely covers the parent.
            /// The texture may extend further out than the parent.
            /// </summary>
            ScaleToCover,

            /// <summary>
            /// The texture is scaled down to the smallest version of None or ScaleToFit.
            /// </summary>
            ScaleDown
        }
    }
}