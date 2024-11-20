using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace ARB.Extensions
{
    /// <summary>
    /// Extension methods for the Texture and Texture2D classes.
    /// </summary>
    internal static class TextureExtensions
    {
        public static async Task<bool> ResizePreservingAspectRatio(this Texture2D texture, int maxSize)
        {
            return await ResizePreservingAspectRatio(texture, maxSize, maxSize);
        }

        public static async Task<bool> ResizePreservingAspectRatio(this Texture2D texture, int maxWidth, int maxHeight)
        {
            if (texture == null || texture.width <= 0 || texture.height <= 0) return false;

            int width = texture.width;
            int height = texture.height;

            float ratioX = (float)maxWidth / (float)width;
            float ratioY = (float)maxHeight / (float)height;
            float ratio = Mathf.Min(ratioX, ratioY);

            int newWidth = (int)(width * ratio);
            int newHeight = (int)(height * ratio);

            RenderTexture renderTexture = RenderTexture.GetTemporary(newWidth, newHeight, 0, RenderTextureFormat.ARGB32);
            Graphics.Blit(texture, renderTexture);

            if (renderTexture.SupportsAsyncGPUReadback())
            {
                AsyncGPUReadbackRequest request = AsyncGPUReadback.Request(renderTexture, 0, texture.format);
                while (!request.done) await Task.Yield();
                if (request.hasError || texture == null) return false;
                texture.Reinitialize(newWidth, newHeight, texture.graphicsFormat, texture.mipmapCount > 1);
                texture.SetPixelData(request.GetData<byte>(), 0);
            }
            else
            {
                texture.Reinitialize(newWidth, newHeight, texture.graphicsFormat, texture.mipmapCount > 1);
                texture.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
            }

            RenderTexture.ReleaseTemporary(renderTexture);

            return true;
        }

        public static bool SupportsAsyncGPUReadback(this Texture texture)
        {
#if UNITY_6000_0_OR_NEWER
            return SystemInfo.supportsAsyncGPUReadback && SystemInfo.IsFormatSupported(texture.graphicsFormat, GraphicsFormatUsage.ReadPixels);
#else
            return SystemInfo.supportsAsyncGPUReadback && SystemInfo.IsFormatSupported(texture.graphicsFormat, FormatUsage.ReadPixels);
#endif
        }

        public static bool SupportsCompression(this Texture2D texture)
        {
            return texture.width % 4 == 0 && texture.height % 4 == 0;
        }
    }
}