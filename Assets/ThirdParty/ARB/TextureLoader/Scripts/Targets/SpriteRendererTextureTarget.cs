// Created by Andre Rene Biasi on 2024/08/01.
// Copyright (c) 2024 Andre Rene Biasi. All rights reserved.

using ARB.TextureLoader.Extensions;
using UnityEngine;

namespace ARB.TextureLoader
{
    public class SpriteRendererTextureTarget : TextureTarget
    {
        protected readonly SpriteRenderer target;
        protected SpriteRenderer fader;

        protected bool shouldDisposePlaceholders;

        public SpriteRendererTextureTarget(SpriteRenderer target, float fadeTime = 0f)
            : base(fadeTime)
        {
            this.target = target;
            shouldDisposePlaceholders = false;
        }

        public SpriteRendererTextureTarget(SpriteRenderer target, Texture2D defaultPlaceholder, Texture2D loadingPlaceholder, Texture2D errorPlaceholder, float fadeTime = 0f)
            : base(fadeTime)
        {
            this.target = target;
            shouldDisposePlaceholders = true;

            if (fadeTime > 0)
            {
                fader = CreateSpriteRenderer("Fader");
            }

            if (defaultPlaceholder != null)
            {
                this.defaultPlaceholder = new SpriteRendererPlaceholder(CreateSpriteRenderer("DefaultPlaceholder", defaultPlaceholder));
            }

            if (loadingPlaceholder != null)
            {
                this.loadingPlaceholder = new SpriteRendererPlaceholder(CreateSpriteRenderer("LoadingPlaceholder", loadingPlaceholder));
            }

            if (errorPlaceholder != null)
            {
                this.errorPlaceholder = new SpriteRendererPlaceholder(CreateSpriteRenderer("ErrorPlaceholder", errorPlaceholder));
            }
        }

        public SpriteRendererTextureTarget(SpriteRenderer target, SpriteRenderer defaultPlaceholder, SpriteRenderer loadingPlaceholder, SpriteRenderer errorPlaceholder, float fadeTime = 0f)
            : base(fadeTime)
        {
            this.target = target;
            shouldDisposePlaceholders = false;

            if (fadeTime > 0)
            {
                fader = CreateSpriteRenderer("Fader");
            }

            if (defaultPlaceholder != null)
            {
                this.defaultPlaceholder = new SpriteRendererPlaceholder(defaultPlaceholder);
            }

            if (loadingPlaceholder != null)
            {
                this.loadingPlaceholder = new SpriteRendererPlaceholder(loadingPlaceholder);
            }

            if (errorPlaceholder != null)
            {
                this.errorPlaceholder = new SpriteRendererPlaceholder(errorPlaceholder);
            }
        }

        protected override Texture CurrentTexture
        {
            get => target != null && target.sprite != null ? target.sprite.texture : null;
            set
            {
                if (target != null) target.sprite = value == null ? null : Sprite.Create(value as Texture2D, new Rect(0f, 0f, value.width, value.height), Vector2.one * 0.5f);
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

            target.sprite = null;
        }

        private SpriteRenderer CreateSpriteRenderer(string name, Texture texture = null)
        {
            SpriteRenderer spriteRenderer = new GameObject(name, typeof(SpriteRenderer)).GetComponent<SpriteRenderer>();
            spriteRenderer.transform.SetParent(target.transform, false);
            spriteRenderer.sprite = texture == null ? null : Sprite.Create(texture as Texture2D, new Rect(0f, 0f, texture.width, texture.height), Vector2.one * 0.5f);
            spriteRenderer.maskInteraction = target.maskInteraction;
            spriteRenderer.color = spriteRenderer.color.WithAlpha(0);
            spriteRenderer.gameObject.SetActive(false);
            return spriteRenderer;
        }

        protected override void SetFaderAlpha(float alpha)
        {
            if (fader != null) fader.color = fader.color.WithAlpha(alpha);
        }

        protected override void SetFaderTexture(Texture texture)
        {
            if (fader == null) return;
            fader.sprite = texture == null ? null : Sprite.Create(texture as Texture2D, new Rect(0f, 0f, texture.width, texture.height), Vector2.one * 0.5f);
            fader.gameObject.SetActive(texture != null);
        }
    }
}
