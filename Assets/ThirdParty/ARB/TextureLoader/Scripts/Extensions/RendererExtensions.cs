using UnityEngine;

namespace ARB.TextureLoader.Extensions
{
    /// <summary>
    /// Extension methods for the Renderer class.
    /// </summary>
    internal static class RendererExtensions
    {
        public static bool HasMaterial(this Renderer renderer, Material material)
        {
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                if (renderer.materials[i].name.Equals(material.name)) return true;
            }

            return false;
        }

        public static void AddMaterial(this Renderer renderer, Material material)
        {
            if (renderer.HasMaterial(material)) return;
            renderer.materials = renderer.materials.Add(material);
        }

        public static void RemoveMaterial(this Renderer renderer, Material material)
        {
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                if (renderer.materials[i].name.Equals(material.name))
                {
                    renderer.materials = renderer.materials.RemoveAt(i);
                    return;
                }
            }
        }
    }
}
