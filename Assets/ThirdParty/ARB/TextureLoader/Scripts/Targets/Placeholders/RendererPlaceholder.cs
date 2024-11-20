// Created by Andre Rene Biasi on 2024/08/01.
// Copyright (c) 2024 Andre Rene Biasi. All rights reserved.

using ARB.TextureLoader.Extensions;
using UnityEngine;

namespace ARB.TextureLoader
{
    /// <summary>
    /// Defines a Renderer placeholder.
    /// </summary>
    public class RendererPlaceholder : Placeholder
    {
        protected readonly Renderer renderer;
        protected readonly Texture texture;
        protected readonly string name;
        protected Material material;

        public RendererPlaceholder(Renderer renderer, Texture texture, string name)
        {
            this.renderer = renderer;
            this.texture = texture;
            this.name = name;
            SetActive(false);
        }

        public override bool IsActive => material != null;

        public override void SetActive(bool value)
        {
            if (renderer == null || texture == null) return;

            if (value)
            {
                if (material == null)
                {
                    renderer.AddMaterial(new(renderer.sharedMaterial) { name = name, mainTexture = texture });
                    material = renderer.materials[^1];
                }

                return;
            }

            if (material != null)
            {
                renderer.RemoveMaterial(material);
                GameObject.Destroy(material);
                material = null;
            }
        }

        public override void SetAlpha(float value)
        {
            if (material != null) material.color = material.color.WithAlpha(value);
        }

        public override void Dispose()
        {
            if (material == null) return;
            renderer.RemoveMaterial(material);
            GameObject.Destroy(material);
            material = null;
        }
    }
}
