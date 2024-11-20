// Created by Andre Rene Biasi on 2024/08/01.
// Copyright (c) 2024 Andre Rene Biasi. All rights reserved.

using ARB.TextureLoader.Extensions;
using UnityEngine;

namespace ARB.TextureLoader
{
    public class RendererTextureTarget : TextureTarget
    {
        protected readonly Renderer target;
        protected Material fader;

        public RendererTextureTarget(Renderer target, float fadeTime = 0f)
            : base(fadeTime)
        {
            this.target = target;
        }

        public RendererTextureTarget(Renderer target, Texture2D defaultPlaceholder, Texture2D loadingPlaceholder, Texture2D errorPlaceholder, float fadeTime = 0f)
            : base(fadeTime)
        {
            this.target = target;

            if (defaultPlaceholder != null)
            {
                this.defaultPlaceholder = new RendererPlaceholder(target, defaultPlaceholder, "DefaultPlaceholder");
            }

            if (loadingPlaceholder != null)
            {
                this.loadingPlaceholder = new RendererPlaceholder(target, loadingPlaceholder, "LoadingPlaceholder");
            }

            if (errorPlaceholder != null)
            {
                this.errorPlaceholder = new RendererPlaceholder(target, errorPlaceholder, "ErrorPlaceholder");
            }
        }

        protected override Texture CurrentTexture
        {
            get => target != null ? target.material.mainTexture : null;
            set
            {
                if (target != null) target.material.mainTexture = value;
            }
        }

        public override void SetAlpha(float alpha)
        {
            if (target != null && target.material != null) target.material.color = target.material.color.WithAlpha(alpha);
        }

        public override void Dispose()
        {
            if (target == null) return;

            if (fader != null)
            {
                target.RemoveMaterial(fader);
                GameObject.Destroy(fader);
                fader = null;
            }

            if (target.material != null && target.material.mainTexture != null)
            {
                target.material.mainTexture = null;
            }
        }

        protected override void SetFaderAlpha(float alpha)
        {
            if (fader != null) fader.color = fader.color.WithAlpha(alpha);
        }

        protected override void SetFaderTexture(Texture texture)
        {
            if (texture == null)
            {
                if (fader == null) return;
                target.RemoveMaterial(fader);
                GameObject.Destroy(fader);
                fader = null;
                return;
            }

            target.AddMaterial(new(target.sharedMaterial) { name = "Fader", mainTexture = texture });
            fader = target.materials[^1];
        }
    }
}
