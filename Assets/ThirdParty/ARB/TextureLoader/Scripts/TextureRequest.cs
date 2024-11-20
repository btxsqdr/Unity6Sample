// Created by Andre Rene Biasi on 2024/08/01.
// Copyright (c) 2024 Andre Rene Biasi. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ARB.Extensions;
using ARB.TextureLoader.Extensions;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Networking;

namespace ARB.TextureLoader
{
    internal class TextureRequest : IDisposable
    {
        private const string ClassName = nameof(TextureRequest);
        private const int DefaultTimeout = 30; // In seconds.

        private readonly TextureLoader loader;
        private readonly string url;
        private readonly string alternateUrl;
        private readonly string cacheFilePath;
        private readonly int timeout;

        private float progress;
        private bool isCanceled;
        private bool isDisposed;

        private readonly string logContext;

        public TextureRequest(TextureLoader loader, string url, string alternateUrl = "", string cacheFilePath = "", int timeout = DefaultTimeout)
        {
            this.loader = loader;
            this.url = url;
            this.alternateUrl = alternateUrl;
            this.cacheFilePath = cacheFilePath;
            this.timeout = timeout;

            if (TextureLoader.Logger.CanLogDebug)
            {
                logContext = $"Context: {GetLogContext()}";
                TextureLoader.Logger.Debug($"Request created. {logContext}");
            }
        }

        public void Start() => StartHandler(url);

        public void Cancel()
        {
            if (isCanceled) return;

            if (TextureLoader.Logger.CanLogDebug)
            {
                TextureLoader.Logger.Debug($"Canceling request... {logContext}");
            }

            isCanceled = true;
        }

        public void Dispose()
        {
            DoDispose();
            GC.SuppressFinalize(this);
        }

        private async void StartHandler(string url)
        {
#if UNITY_6000_0_OR_NEWER
            DownloadedTextureParams parameters = DownloadedTextureParams.Default;
            parameters.readable = true;
            parameters.mipmapChain = loader.MipmapChain;
            parameters.linearColorSpace = loader.LinearColorSpace;

            using UnityWebRequest request = UnityWebRequestTexture.GetTexture(url, parameters);
#else
            using UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
#endif
            request.timeout = timeout;

            UnityWebRequestAsyncOperation operation = request.SendWebRequest();

            progress = 0;
            isCanceled = false;

            bool isLocalFile = url.StartsWith("file://");

            if (TextureLoader.Logger.CanLogDebug)
            {
                TextureLoader.Logger.Debug($"Request started. {logContext}");
            }

            while (!operation.isDone)
            {
                if (isCanceled)
                {
                    request.Abort();
                    request.Dispose();
                    CancelHandler();
                    return;
                }

                if (!isLocalFile && request.downloadProgress > progress)
                {
                    progress = request.downloadProgress;
                    OnRequestProgress?.Invoke(loader.Id, progress);
                }

                await Task.Yield();
            }

            // Make sure progress is set to 1 (100%).
            if (progress < 1)
            {
                progress = 1;
                OnRequestProgress?.Invoke(loader.Id, progress);
            }

            await CompleteHandler(request);
            request.Dispose();
            Dispose();
        }

        private void CancelHandler(Texture2D texture = null)
        {
            if (!Application.isPlaying)
            {
                if (texture != null) GameObject.DestroyImmediate(texture);
                return;
            }

            if (texture != null) GameObject.Destroy(texture);

            if (TextureLoader.Logger.CanLogDebug)
            {
                TextureLoader.Logger.Debug($"Request canceled. {logContext}");
            }

            Dispose();
        }

        private async Task CompleteHandler(UnityWebRequest request)
        {
            // Canceled.
            if (isCanceled)
            {
                CancelHandler(request.result == UnityWebRequest.Result.Success ? DownloadHandlerTexture.GetContent(request) : null);
                return;
            }

            // Error.
            if (request.result != UnityWebRequest.Result.Success)
            {
                // Retry with the alternate URL if possible.
                if (!string.IsNullOrEmpty(alternateUrl) && url != alternateUrl)
                {
                    if (TextureLoader.Logger.CanLogDebug)
                    {
                        TextureLoader.Logger.Debug($"Request failed. Trying alternate url... Error: {request.error} {logContext}");
                    }

                    StartHandler(alternateUrl);
                    return;
                }

                ErrorHandler($"{request.error} @ {request.url}");
                return;
            }

            Texture2D texture = DownloadHandlerTexture.GetContent(request);

            // No subscribers.
            if (OnRequestComplete == null)
            {
                if (TextureLoader.Logger.CanLogDebug)
                {
                    TextureLoader.Logger.Debug($"No subscribers. {logContext}");
                }

                CancelHandler(texture);
                return;
            }

            // No texture.
            if (texture == null)
            {
                ErrorHandler("Invalid texture data.");
                return;
            }

            Dictionary<string, object> textureInfo = null;

            if (TextureLoader.Logger.CanLogDebug)
            {
                textureInfo = GetTextureInfo(texture, (long)request.downloadedBytes);
                TextureLoader.Logger.Debug($"Texture retrieved. Info: {textureInfo.ToJson()} {logContext}");
            }

            // Texture is too small. Destroy texture and return null.
            if (texture.width < loader.MinSize && texture.height < loader.MinSize)
            {
                GameObject.Destroy(texture);
                ErrorHandler($"Texture is smaller than minimum size specified.");
                return;
            }
            // Texture is too large. Resize.
            else if (texture.width > loader.MaxSize || texture.height > loader.MaxSize)
            {
                if (TextureLoader.Logger.CanLogDebug)
                {
                    TextureLoader.Logger.Debug($"Texture is larger than maximum size specified. Resizing... {logContext}");
                }

                bool resized = await texture.ResizePreservingAspectRatio(loader.MaxSize);

                if (!resized && TextureLoader.Logger.CanLogWarnings)
                {
                    TextureLoader.Logger.Warning($"Failed to resize texture. {logContext}");
                }

                if (isCanceled)
                {
                    CancelHandler(texture);
                    return;
                }

                if (TextureLoader.Logger.CanLogDebug)
                {
                    TextureLoader.Logger.Debug($"Texture resized from ({textureInfo["width"]}x{textureInfo["height"]}) to ({texture.width}x{texture.height}). {logContext}");
                    textureInfo["width"] = texture.width;
                    textureInfo["height"] = texture.height;
                }
            }

#if !UNITY_6000_0_OR_NEWER
            // Remove texture mipmaps if requested (only if they were generated).
            // Not required in Unity 6.0.0 and newer, UnityWebRequestTexture.GetTexture allows passing a parameter to control mipmaps.
            if (!loader.MipmapChain && texture.mipmapCount > 1)
            {
                texture.Reinitialize(texture.width, texture.height, texture.format, false);
                texture.LoadImage(request.downloadHandler.data);

                if (TextureLoader.Logger.CanLogDebug)
                {
                    TextureLoader.Logger.Debug($"Texture mipmaps removed. {logContext}");
                }
            }
#endif

            // Set texture name.
            texture.name = loader.Url;

            // Cache texture on disk if requested (only if it's not already cached).
            if (loader.IsCacheEnabled)
            {
                if (!File.Exists(cacheFilePath))
                {
                    await SaveToCache(url, texture, cacheFilePath, loader.CacheFormat, loader.CacheQuality);

                    if (isCanceled)
                    {
                        CancelHandler(texture);
                        return;
                    }
                }
                else if (TextureLoader.Logger.CanLogDebug)
                {
                    TextureLoader.Logger.Debug($"Texture already cached on disk. {logContext}");
                }
            }

            // Set wrap mode.
            if (texture.wrapMode != loader.WrapMode)
            {
                texture.wrapMode = loader.WrapMode;
            }

            // Set filter mode.
            if (texture.filterMode != loader.FilterMode)
            {
                texture.filterMode = loader.FilterMode;
            }

            // Set anisotropic level.
            if (texture.anisoLevel != loader.AnisoLevel)
            {
                texture.anisoLevel = loader.AnisoLevel;
            }

            if (TextureLoader.Logger.CanLogDebug)
            {
                TextureLoader.Logger.Debug($"Texture wrap mode set to {texture.wrapMode}. {logContext}");
                TextureLoader.Logger.Debug($"Texture filter mode set to {texture.filterMode}. {logContext}");
                TextureLoader.Logger.Debug($"Texture anisotropic level set to {texture.anisoLevel}. {logContext}");
            }

            // Compress the texture if requested (only if possible).
            if (loader.Compression != TextureCompression.None && texture.SupportsCompression())
            {
                bool highQuality = loader.Compression == TextureCompression.HighQuality;
                texture.Compress(highQuality);

                if (TextureLoader.Logger.CanLogDebug)
                {
                    TextureLoader.Logger.Debug($"Texture compressed in {(highQuality ? "high" : "normal")} quality. {logContext}");
                }
            }

            // Generate mipmaps and/or make non-readable if requested.
            texture.Apply(loader.MipmapChain && texture.mipmapCount <= 1, !loader.Readable);

            if (TextureLoader.Logger.CanLogDebug)
            {
                if (texture.mipmapCount > 1)
                {
                    TextureLoader.Logger.Debug($"Generated {texture.mipmapCount} mipmaps. {logContext}");
                }

                if (!texture.isReadable)
                {
                    TextureLoader.Logger.Debug($"Texture changed to non-readable. {logContext}");
                }

                TextureLoader.Logger.Debug($"Request completed. {logContext}");
            }

            OnRequestComplete?.Invoke(loader.Id, texture);
        }

        private void ErrorHandler(string error)
        {
            if (string.IsNullOrEmpty(error))
            {
                error = "N/A.";
            }
            else if (!error.EndsWith("."))
            {
                error += ".";
            }

            if (TextureLoader.Logger.CanLogErrors)
            {
                TextureLoader.Logger.Error($"Request failed. Error: {error} {logContext}");
            }

            OnRequestError?.Invoke(loader.Id, error);
            Dispose();
        }

        private void DoDispose()
        {
            if (isDisposed) return;
            isDisposed = true;

            if (TextureLoader.Logger.CanLogDebug)
            {
                TextureLoader.Logger.Debug($"Request disposed. {logContext}");
            }

            OnRequestDisposed?.Invoke(loader.Id);
        }

        private string GetLogContext()
        {
            Dictionary<string, object> context = new()
            {
                { nameof(loader.Id), loader.Id },
                { nameof(url), url },
                { nameof(alternateUrl), alternateUrl },
                { nameof(cacheFilePath), cacheFilePath },
                { nameof(timeout), timeout },
            };

            return context.ToJson();
        }

        #region Static

        public static Action<string, float> OnRequestProgress;
        public static Action<string, Texture2D> OnRequestComplete;
        public static Action<string, string> OnRequestError;
        public static Action<string> OnRequestDisposed;

        private static async Task SaveToCache(string url, Texture2D texture, string filePath, FileFormat format, int quality)
        {
            NativeArray<byte> bytes = texture.GetRawTextureData<byte>();
            GraphicsFormat graphicsFormat = texture.graphicsFormat;

            uint width = (uint)texture.width;
            uint height = (uint)texture.height;

            byte[] encodedBytes = await Task.Run(() =>
            {
                return format == FileFormat.JPG
                    ? ImageConversion.EncodeNativeArrayToJPG(bytes, graphicsFormat, width, height, 0, quality).ToArray()
                    : ImageConversion.EncodeNativeArrayToPNG(bytes, graphicsFormat, width, height, 0).ToArray();
            });

            string result = string.Empty;
            bool error = false;

            await Task.Run(async () =>
            {
                try
                {
                    new FileInfo(filePath).Directory.Create();
                    await Task.Yield();
                    File.WriteAllBytes(filePath, encodedBytes);
                    File.SetCreationTime(filePath, DateTime.Now);

                    if (TextureLoader.Logger.CanLogDebug)
                    {
                        result = $"Texture from {url} saved to {filePath}";
                    }
                }
                catch (Exception ex)
                {
                    if (TextureLoader.Logger.CanLogErrors)
                    {
                        result = $"Failed to save texture from {url} to {filePath} {ex}";
                    }
                }
            });

            if (string.IsNullOrEmpty(result)) return;

            if (error)
            {
                TextureLoader.Logger.Error(result);
                return;
            }

            TextureLoader.Logger.Debug(result);
        }

        private Dictionary<string, object> GetTextureInfo(Texture2D texture, long bytes)
        {
            return new()
            {
                { "width", texture.width },
                { "height", texture.height },
                { "format", texture.format },
                { "graphicsFormat", texture.graphicsFormat },
                { "mipmapCount", texture.mipmapCount},
                { "data", BytesToString(bytes) },
            };
        }

        private static string BytesToString(long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
            if (byteCount == 0) return "0" + suf[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString() + suf[place];
        }

        #endregion
    }
}
