// Created by Andre Rene Biasi on 2024/08/01.
// Copyright (c) 2024 Andre Rene Biasi. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ARB.TextureLoader.Extensions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace ARB.TextureLoader
{
    /// <summary>
    /// Manages the loading, caching, and application of textures.
    /// Provides an interface for specifying size constraints, mipmaps, readable state, wrap mode, filter mode, anisotropic level, compression, and cache settings.
    /// It supports applying loaded textures to multiple <see cref="TextureTarget"/> instances and allows tracking the rogress of the loading operation.
    /// </summary>
    public class TextureLoader : IDisposable
    {
        private string id;
        /// <summary>
        /// Gets the unique identifier for this texture loader instance.
        /// </summary>
        public string Id => id;

        private readonly string url;
        /// <summary>
        /// Gets the URL from which the texture will be loaded.
        /// </summary>
        public string Url => url;

        private readonly int minSize = DefaultMinSize;
        /// <summary>
        /// Gets the minimum size (in pixels) allowed for the loaded texture.
        /// An error is returned if it doesn't meet this value.
        /// </summary>
        public int MinSize => minSize;

        private readonly int maxSize = DefaultMaxSize;
        /// <summary>
        /// Gets the maximum size (in pixels) allowed for the loaded texture.
        /// Resized
        /// </summary>
        public int MaxSize => maxSize;

        private readonly bool mipmapChain = DefaultMipmapChain;
        /// <summary>
        /// Gets a value indicating whether a mipmap chain should be generated for the loaded texture.
        /// </summary>
        public bool MipmapChain => mipmapChain;

        private readonly bool readable = DefaultReadable;
        /// <summary>
        /// Gets a value indicating whether the loaded texture should be readable by the CPU.
        /// </summary>
        public bool Readable => readable;

        private readonly TextureWrapMode wrapMode = DefaultWrapMode;
        /// <summary>
        /// Gets the wrap mode to be used for the loaded texture.
        /// </summary>
        public TextureWrapMode WrapMode => wrapMode;

        private readonly FilterMode filterMode = DefaultFilterMode;
        /// <summary>
        /// Gets the filter mode to be used for the loaded texture.
        /// </summary>
        public FilterMode FilterMode => filterMode;

        private readonly int anisoLevel;
        /// <summary>
        /// Gets the anisotropic filtering level to be used for the loaded texture.
        /// </summary>
        public int AnisoLevel => DefaultAnisoLevel;

        private readonly TextureCompression compression = DefaultCompression;
        /// <summary>
        /// Gets the compression setting to be used for the loaded texture.
        /// </summary>
        public TextureCompression Compression => compression;

#if UNITY_6000_0_OR_NEWER
        private readonly bool linearColorSpace = DefaultLinearColorSpace;
        /// <summary>
        /// Gets the linear color space setting to be used for the loaded texture.
        /// </summary>
        public bool LinearColorSpace => linearColorSpace;
#endif

        private bool isCacheEnabled = DefaultUseCache;
        /// <summary>
        /// Gets a value indicating whether caching is enabled for the loaded texture.
        /// </summary>
        public bool IsCacheEnabled => isCacheEnabled;

        private readonly DirectoryType cacheDirectory = DefaultCacheDirectory;
        private readonly string cacheSubdirectory = DefaultCacheSubdirectory;
        /// <summary>
        /// Gets the full path to the cache directory where the loaded texture will be stored.
        /// </summary>
        public string CachePath => GetCachePath(cacheDirectory, cacheSubdirectory);

        private FileFormat cacheFormat = DefaultCacheFormat;
        /// <summary>
        /// Gets the file format to be used when caching the loaded texture.
        /// </summary>
        public FileFormat CacheFormat => cacheFormat;

        private int cacheQuality = DefaultCacheQuality;
        /// <summary>
        /// Gets the quality setting (0-100) for caching the loaded texture when using lossy formats like JPG.
        /// </summary>
        public int CacheQuality => cacheQuality;

        private TimeSpan cacheDuration = DefaultCacheDuration;
        /// <summary>
        /// Gets the duration for which the loaded texture should be cached.
        /// </summary>
        public TimeSpan CacheDuration => cacheDuration;

        /// <summary>
        /// Gets the file extension associated with the cache format (e.g., ".jpg" or ".png").
        /// </summary>
        public string CacheExtension => cacheFormat == FileFormat.JPG ? ".jpg" : ".png";

        private readonly HashSet<TextureTarget> targets = new();
        /// <summary>
        /// Gets the number of <see cref="TextureTarget"/> instances associated with this texture loader.
        /// </summary>
        public int TargetCount => targets.Count;

        private bool isQueued;
        /// <summary>
        /// Gets a value indicating whether the texture loading operation is currently queued.
        /// </summary>
        public bool IsQueued => isQueued;

        private bool inProgress;
        /// <summary>
        /// Gets a value indicating whether the texture loading operation is currently in progress.
        /// </summary>
        public bool InProgress => inProgress;

        private bool isDone;
        /// <summary>
        /// Gets a value indicating whether the texture loading operation is complete.
        /// </summary>
        public bool IsDone => isDone;

        private bool isCanceled;
        /// <summary>
        /// Gets a value indicating whether the texture loading operation has been canceled.
        /// </summary>
        public bool IsCanceled => isCanceled;

        private bool isDisposed;
        /// <summary>
        /// Gets a value indicating whether the texture loader has been disposed.
        /// </summary>
        public bool IsDisposed => isDisposed;

        private float progress;
        /// <summary>
        /// Gets the progress of the texture loading operation, represented as a float between 0 and 1.
        /// </summary>
        public float Progress => progress;

        private Texture2D texture;
        /// <summary>
        /// Gets the loaded <see cref="Texture2D"/> instance.
        /// </summary>
        public Texture2D Texture => texture;

        private string hash;
        private string logContext;
        private float startTime;

        private Action onStart;
        private Action<float> onProgress;
        private Action onCancel;
        private Action<Texture2D> onComplete;
        private Action<string> onError;

        private TextureLoader(string url, TextureSettings settings)
        {
            this.url = url;
            minSize = settings.MinSize;
            maxSize = settings.MaxSize;
            mipmapChain = settings.MipmapChain;
            readable = settings.Readable;
            wrapMode = settings.WrapMode;
            filterMode = settings.FilterMode;
            anisoLevel = settings.AnisoLevel;
            compression = settings.Compression;

#if UNITY_6000_0_OR_NEWER
            linearColorSpace = settings.LinearColorSpace;
#endif

            if (!instances.Contains(this)) instances.Add(this);
        }

        private TextureLoader(string url, int maxSize, TextureCompression compression, bool readable)
        {
            this.url = url;
            this.maxSize = maxSize;
            this.compression = compression;
            this.readable = readable;
            if (!instances.Contains(this)) instances.Add(this);
        }

        /// <summary>
        /// Initiates the texture loading process. If <paramref name="enqueue"/> is set to true,
        /// the texture loading operation will be added to a queue instead of starting immediately.
        /// </summary>
        /// <param name="enqueue">
        /// Optional. If true, the loading operation will be enqueued and executed later;
        /// if false, it will start immediately. Default is false.
        /// </param>
        public async void Start(bool enqueue = false)
        {
            if ((enqueue && isQueued) || inProgress || isDone || isDisposed) return;

            // Check for null or empty URL.
            if (string.IsNullOrEmpty(url))
            {
                if (Logger.CanLogErrors) Logger.Error("Missing URL.");
                return;
            }

            foreach (TextureTarget target in targets) target?.DisplayLoadingPlaceholder();

            hash = GetHash(url);
            id = GetId(hash, this);

            inProgress = false;
            isDone = false;
            isCanceled = false;
            isDisposed = false;

            progress = 0;
            startTime = 0;

            if (Logger.Level != LogLevel.None)
            {
                logContext = $"Context: {GetLogContext()}";
            }

            // Check if it should be queued.
            if (enqueue)
            {
                isQueued = true;
                Enqueue(this);
                return;
            }

            isQueued = false;
            inProgress = true;
            startTime = Time.realtimeSinceStartup;

            onStart?.Invoke();
            OnLoadStart?.Invoke(this);

            onProgress?.Invoke(progress);
            OnLoadProgress?.Invoke(this, progress);

            // Load from memory.
            if (textures.ContainsKey(id))
            {
                if (Logger.CanLogDebug) Logger.Debug($"Returning texture in memory. {logContext}");
                CompleteHandler(id, textures[id].Texture);
                return;
            }

            if (Logger.CanLogInfo) Logger.Info($"Loading texture. {logContext}");

            BindRequestHandlers();

            // Load from cache.
            if (isCacheEnabled)
            {
                string cacheFileName = $"{hash}-{maxSize}";
                string cacheFilePath = $"{CachePath}{cacheFileName}{CacheExtension}";

                if (File.Exists(cacheFilePath))
                {
                    if (HasCacheFileExpired(cacheFilePath, cacheDuration))
                    {
                        if (Logger.CanLogDebug) Logger.Debug($"Cached file expired. FilePath: {cacheFilePath} {logContext}");
                        await Task.Run(() => DeleteFile(cacheFilePath));
                    }
                    else
                    {
                        Request(this, $"file://{cacheFilePath}", url, cacheFilePath);
                        return;
                    }
                }

                // Load from URL.
                Request(this, url, null, cacheFilePath);
                return;
            }

            // Load from URL.
            Request(this, url);
        }

        /// <summary>
        /// Cancels the ongoing texture loading operation.
        /// </summary>
        public void Cancel()
        {
            if (isCanceled || isDone || isDisposed) return;

            if (Logger.CanLogInfo) Logger.Info($"Canceled texture load. {logContext}");

            UnbindRequestHandlers();

            foreach (TextureTarget target in targets) target?.DisplayDefaultPlaceholder();

            isCanceled = true;
            inProgress = false;
            startTime = 0;

            onCancel?.Invoke();
            OnLoadCancel?.Invoke(this);

            if (queue.Contains(this))
            {
                isQueued = false;
                queue.Remove(this);
                return;
            }

            if (lastDequeued?.Id == id)
            {
                lastDequeued = null;
                isWaitingForNextInQueue = false;
                StartNextInQueue();
            }
        }

        /// <summary>
        /// Releases all resources used by this texture loader instance.
        /// Must be called when no longer needed to avoid memory leaks.
        /// </summary>
        public void Dispose()
        {
            DoDispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Loads the texture into the specified <see cref="TextureTarget"/>.
        /// </summary>
        /// <param name="target">The <see cref="TextureTarget"/> where the texture should be loaded.</param>
        /// <returns>Returns the current instance of <see cref="TextureLoader"/> with the specified target.</returns>
        public TextureLoader Into(TextureTarget target)
        {
            target.DisplayDefaultPlaceholder();
            targets.Add(target);
            return this;
        }

        /// <summary>
        /// Loads the texture into the specified <see cref="Renderer"/>
        /// with optional <see cref="Texture2D"/> placeholders for default, loading, and error states, as well as fade time.
        /// </summary>
        /// <param name="target">The <see cref="Renderer"/> to which the texture should be applied.</param>
        /// <param name="fadeTime">The time in seconds for the texture to fade in. Default is 0 (no fade).</param>
        /// <returns>Returns the current instance of <see cref="TextureLoader"/> with the specified settings.</returns>
        public TextureLoader Into(Renderer target, float fadeTime = 0)
            => Into(new RendererTextureTarget(target, fadeTime));

        /// <summary>
        /// Loads the texture into the specified <see cref="Renderer"/>
        /// with optional <see cref="Texture2D"/> placeholders for default, loading, and error states, as well as fade time.
        /// </summary>
        /// <param name="target">The <see cref="Renderer"/> to which the texture should be applied.</param>
        /// <param name="defaultPlaceholder">Optional. The <see cref="Texture2D"/> to display before starting to load the texture. Default is null.</param>
        /// <param name="loadingPlaceholder">Optional. The <see cref="Texture2D"/> to display while the texture is loading. Default is null.</param>
        /// <param name="errorPlaceholder">Optional. The <see cref="Texture2D"/> to display if an error occurs during loading. Default is null.</param>
        /// <param name="fadeTime">The time in seconds for the texture to fade in. Default is 0 (no fade).</param>
        /// <returns>Returns the current instance of <see cref="TextureLoader"/> with the specified settings.</returns>
        public TextureLoader Into(Renderer target, Texture2D defaultPlaceholder, Texture2D loadingPlaceholder, Texture2D errorPlaceholder, float fadeTime = 0)
            => Into(new RendererTextureTarget(target, defaultPlaceholder, loadingPlaceholder, errorPlaceholder, fadeTime));

        /// <summary>
        /// Loads the texture into the specified <see cref="SpriteRenderer"/>
        /// with optional <see cref="Texture2D"/> placeholders for default, loading, and error states, as well as fade time.
        /// </summary>
        /// <param name="target">The <see cref="SpriteRenderer"/> to which the texture should be applied.</param>
        /// <param name="fadeTime">The time in seconds for the texture to fade in. Default is 0 (no fade).</param>
        /// <returns>Returns the current instance of <see cref="TextureLoader"/> with the specified settings.</returns>
        public TextureLoader Into(SpriteRenderer target, float fadeTime = 0)
            => Into(new SpriteRendererTextureTarget(target, fadeTime));

        /// <summary>
        /// Loads the texture into the specified <see cref="SpriteRenderer"/>
        /// with optional <see cref="Texture2D"/> placeholders for default, loading, and error states, as well as fade time.
        /// </summary>
        /// <param name="target">The <see cref="SpriteRenderer"/> to which the texture should be applied.</param>
        /// <param name="defaultPlaceholder">Optional. The <see cref="Texture2D"/> to display before starting to load the texture. Default is null.</param>
        /// <param name="loadingPlaceholder">Optional. The <see cref="Texture2D"/> to display while the texture is loading. Default is null.</param>
        /// <param name="errorPlaceholder">Optional. The <see cref="Texture2D"/> to display if an error occurs during loading. Default is null.</param>
        /// <param name="fadeTime">The time in seconds for the texture to fade in. Default is 0 (no fade).</param>
        /// <returns>Returns the current instance of <see cref="TextureLoader"/> with the specified settings.</returns>
        public TextureLoader Into(SpriteRenderer target, Texture2D defaultPlaceholder, Texture2D loadingPlaceholder, Texture2D errorPlaceholder, float fadeTime = 0)
            => Into(new SpriteRendererTextureTarget(target, defaultPlaceholder, loadingPlaceholder, errorPlaceholder, fadeTime));

        /// <summary>
        /// Loads the texture into the specified <see cref="SpriteRenderer"/>
        /// with optional <see cref="Texture2D"/> placeholders for default, loading, and error states, as well as fade time.
        /// </summary>
        /// <param name="target">The <see cref="SpriteRenderer"/> to which the texture should be applied.</param>
        /// <param name="defaultPlaceholder">Optional. The <see cref="SpriteRenderer"/> to display before starting to load the texture. Default is null.</param>
        /// <param name="loadingPlaceholder">Optional. The <see cref="SpriteRenderer"/> to display while the texture is loading. Default is null.</param>
        /// <param name="errorPlaceholder">Optional. The <see cref="SpriteRenderer"/> to display if an error occurs during loading. Default is null.</param>
        /// <param name="fadeTime">The time in seconds for the texture to fade in. Default is 0 (no fade).</param>
        /// <returns>Returns the current instance of <see cref="TextureLoader"/> with the specified settings.</returns>
        public TextureLoader Into(SpriteRenderer target, SpriteRenderer defaultPlaceholder, SpriteRenderer loadingPlaceholder, SpriteRenderer errorPlaceholder, float fadeTime = 0)
            => Into(new SpriteRendererTextureTarget(target, defaultPlaceholder, loadingPlaceholder, errorPlaceholder, fadeTime));

        /// <summary>
        /// Loads the texture into the specified <see cref="Image"/>
        /// with optional <see cref="Texture2D"/> placeholders for default, loading, and error states, as well as scale mode and fade time.
        /// </summary>
        /// <param name="target">The <see cref="Image"/> to which the texture should be loaded.</param>
        /// <param name="scaleMode">The mode to use for scaling the texture. Default is <see cref="UGUITextureTarget.ScaleMode.None"/>.</param>
        /// <param name="fadeTime">The time in seconds for the texture to fade in. Default is 0 (no fade).</param>
        /// <returns>Returns the current instance of <see cref="TextureLoader"/> with the specified settings.</returns>
        public TextureLoader Into(Image target, UGUITextureTarget.ScaleMode scaleMode = UGUITextureTarget.ScaleMode.None, float fadeTime = 0)
            => Into(new ImageTextureTarget(target, scaleMode, fadeTime));

        /// <summary>
        /// Loads the texture into the specified <see cref="Image"/>
        /// with optional <see cref="Texture2D"/> placeholders for default, loading, and error states, as well as scale mode and fade time.
        /// </summary>
        /// <param name="target">The <see cref="Image"/> to which the texture should be loaded.</param>
        /// <param name="defaultPlaceholder">Optional. The <see cref="Texture2D"/> to display before starting to load the texture. Default is null.</param>
        /// <param name="loadingPlaceholder">Optional. The <see cref="Texture2D"/> to display while the texture is loading. Default is null.</param>
        /// <param name="errorPlaceholder">Optional. The <see cref="Texture2D"/> to display if an error occurs during loading. Default is null.</param>
        /// <param name="scaleMode">The mode to use for scaling the texture. Default is <see cref="UGUITextureTarget.ScaleMode.None"/>.</param>
        /// <param name="fadeTime">The time in seconds for the texture to fade in. Default is 0 (no fade).</param>
        /// <returns>Returns the current instance of <see cref="TextureLoader"/> with the specified settings.</returns>
        public TextureLoader Into(Image target, Texture2D defaultPlaceholder, Texture2D loadingPlaceholder, Texture2D errorPlaceholder, UGUITextureTarget.ScaleMode scaleMode = UGUITextureTarget.ScaleMode.None, float fadeTime = 0)
            => Into(new ImageTextureTarget(target, defaultPlaceholder, loadingPlaceholder, errorPlaceholder, scaleMode, fadeTime));

        /// <summary>
        /// Loads the texture into the specified <see cref="Image"/>
        /// with optional <see cref="CanvasGroup"/> placeholders for default, loading, and error states, as well as scale mode and fade time.
        /// </summary>
        /// <param name="target">The <see cref="Image"/> to which the texture should be loaded.</param>
        /// <param name="defaultPlaceholder"> Optional. The <see cref="CanvasGroup"/> to display before starting to load the texture. Default is null.</param>
        /// <param name="loadingPlaceholder">Optional. The <see cref="CanvasGroup"/> to display while the texture is loading. Default is null.</param>
        /// <param name="errorPlaceholder">Optional. The <see cref="CanvasGroup"/> to display if an error occurs during loading. Default is null.</param>
        /// <param name="scaleMode">The mode to use for scaling the texture. Default is <see cref="UGUITextureTarget.ScaleMode.None"/>.</param>
        /// <param name="fadeTime">The time in seconds for the texture to fade in. Default is 0 (no fade).</param>
        /// <returns>Returns the current instance of <see cref="TextureLoader"/> with the specified settings.</returns>
        public TextureLoader Into(Image target, CanvasGroup defaultPlaceholder, CanvasGroup loadingPlaceholder, CanvasGroup errorPlaceholder, UGUITextureTarget.ScaleMode scaleMode = UGUITextureTarget.ScaleMode.None, float fadeTime = 0)
            => Into(new ImageTextureTarget(target, defaultPlaceholder, loadingPlaceholder, errorPlaceholder, scaleMode, fadeTime));

        /// <summary>
        /// Loads the texture into the specified <see cref="RawImage"/>
        /// with optional <see cref="Texture2D"/> placeholders for default, loading, and error states, as well as scale mode and fade time.
        /// </summary>
        /// <param name="target">The <see cref="RawImage"/> to which the texture should be loaded.</param>
        /// <param name="scaleMode">The mode to use for scaling the texture. Default is <see cref="UGUITextureTarget.ScaleMode.None"/>.</param>
        /// <param name="fadeTime">The time in seconds for the texture to fade in. Default is 0 (no fade).</param>
        /// <returns>Returns the current instance of <see cref="TextureLoader"/> with the specified settings.</returns>
        public TextureLoader Into(RawImage target, UGUITextureTarget.ScaleMode scaleMode = UGUITextureTarget.ScaleMode.None, float fadeTime = 0)
            => Into(new RawImageTextureTarget(target, scaleMode, fadeTime));

        /// <summary>
        /// Loads the texture into the specified <see cref="RawImage"/>
        /// with optional <see cref="Texture2D"/> placeholders for default, loading, and error states, as well as scale mode and fade time.
        /// </summary>
        /// <param name="target">The <see cref="RawImage"/> to which the texture should be loaded.</param>
        /// <param name="defaultPlaceholder">Optional. The <see cref="Texture2D"/> to display before starting to load the texture. Default is null.</param>
        /// <param name="loadingPlaceholder">Optional. The <see cref="Texture2D"/> to display while the texture is loading. Default is null.</param>
        /// <param name="errorPlaceholder">Optional. The <see cref="Texture2D"/> to display if an error occurs during loading. Default is null.</param>
        /// <param name="scaleMode">The mode to use for scaling the texture. Default is <see cref="UGUITextureTarget.ScaleMode.None"/>.</param>
        /// <param name="fadeTime">The time in seconds for the texture to fade in. Default is 0 (no fade).</param>
        /// <returns>Returns the current instance of <see cref="TextureLoader"/> with the specified settings.</returns>
        public TextureLoader Into(RawImage target, Texture2D defaultPlaceholder, Texture2D loadingPlaceholder, Texture2D errorPlaceholder, UGUITextureTarget.ScaleMode scaleMode = UGUITextureTarget.ScaleMode.None, float fadeTime = 0)
            => Into(new RawImageTextureTarget(target, defaultPlaceholder, loadingPlaceholder, errorPlaceholder, scaleMode, fadeTime));

        /// <summary>
        /// Loads the texture into the specified <see cref="RawImage"/>
        /// with optional <see cref="CanvasGroup"/> placeholders for default, loading, and error states, as well as scale mode and fade time.
        /// </summary>
        /// <param name="target">The <see cref="RawImage"/> to which the texture should be loaded.</param>
        /// <param name="defaultPlaceholder"> Optional. The <see cref="CanvasGroup"/> to display before starting to load the texture. Default is null.</param>
        /// <param name="loadingPlaceholder">Optional. The <see cref="CanvasGroup"/> to display while the texture is loading. Default is null.</param>
        /// <param name="errorPlaceholder">Optional. The <see cref="CanvasGroup"/> to display if an error occurs during loading. Default is null.</param>
        /// <param name="scaleMode">The mode to use for scaling the texture. Default is <see cref="UGUITextureTarget.ScaleMode.None"/>.</param>
        /// <param name="fadeTime">The time in seconds for the texture to fade in. Default is 0 (no fade).</param>
        /// <returns>Returns the current instance of <see cref="TextureLoader"/> with the specified settings.</returns>
        public TextureLoader Into(RawImage target, CanvasGroup defaultPlaceholder, CanvasGroup loadingPlaceholder, CanvasGroup errorPlaceholder, UGUITextureTarget.ScaleMode scaleMode = UGUITextureTarget.ScaleMode.None, float fadeTime = 0)
            => Into(new RawImageTextureTarget(target, defaultPlaceholder, loadingPlaceholder, errorPlaceholder, scaleMode, fadeTime));

        /// <summary>
        /// Loads the texture into the specified <see cref="UnityEngine.UIElements.VisualElement"/>
        /// with optional <see cref="Texture2D"/> placeholders for default, loading, and error states, as well as scale mode and fade time.
        /// </summary>
        /// <param name="target">The <see cref="UnityEngine.UIElements.VisualElement"/> to which the texture should be loaded.</param>
        /// <param name="scaleMode">The mode to use for scaling the texture. Default is <see cref="ScaleMode.ScaleAndCrop"/>.</param>
        /// <param name="fadeTime">The time in seconds for the texture to fade in. Default is 0 (no fade).</param>
        /// <returns>Returns the current instance of <see cref="TextureLoader"/> with the specified settings.</returns>
        public TextureLoader Into(UnityEngine.UIElements.VisualElement target, ScaleMode scaleMode = ScaleMode.ScaleAndCrop, float fadeTime = 0)
            => Into(new UIToolkitTextureTarget(target, scaleMode, fadeTime));

        /// <summary>
        /// Loads the texture into the specified <see cref="UnityEngine.UIElements.VisualElement"/>
        /// with optional <see cref="Texture2D"/> placeholders for default, loading, and error states, as well as scale mode and fade time.
        /// </summary>
        /// <param name="target">The <see cref="UnityEngine.UIElements.VisualElement"/> to which the texture should be loaded.</param>
        /// <param name="defaultPlaceholder">Optional. The <see cref="Texture2D"/> to display before starting to load the texture. Default is null.</param>
        /// <param name="loadingPlaceholder">Optional. The <see cref="Texture2D"/> to display while the texture is loading. Default is null.</param>
        /// <param name="errorPlaceholder">Optional. The <see cref="Texture2D"/> to display if an error occurs during loading. Default is null.</param>
        /// <param name="scaleMode">The mode to use for scaling the texture. Default is <see cref="ScaleMode.ScaleAndCrop"/>.</param>
        /// <param name="fadeTime">The time in seconds for the texture to fade in. Default is 0 (no fade).</param>
        /// <returns>Returns the current instance of <see cref="TextureLoader"/> with the specified settings.</returns>
        public TextureLoader Into(UnityEngine.UIElements.VisualElement target, Texture2D defaultPlaceholder, Texture2D loadingPlaceholder, Texture2D errorPlaceholder, ScaleMode scaleMode = ScaleMode.ScaleAndCrop, float fadeTime = 0)
            => Into(new UIToolkitTextureTarget(target, defaultPlaceholder, loadingPlaceholder, errorPlaceholder, scaleMode, fadeTime));

        /// <summary>
        /// Loads the texture into the specified <see cref="UnityEngine.UIElements.VisualElement"/>
        /// with optional placeholders for default, loading, and error states, as well as scale mode and fade time.
        /// </summary>
        /// <param name="target">The <see cref="UnityEngine.UIElements.VisualElement"/> to which the texture should be loaded.</param>
        /// <param name="defaultPlaceholder">Optional. The <see cref="UnityEngine.UIElements.VisualElement"/> to display before starting to load the texture. Default is null.</param>
        /// <param name="loadingPlaceholder">Optional. The <see cref="UnityEngine.UIElements.VisualElement"/> to display while the texture is loading. Default is null.</param>
        /// <param name="errorPlaceholder">Optional. The <see cref="UnityEngine.UIElements.VisualElement"/> to display if an error occurs during loading. Default is null.</param>
        /// <param name="scaleMode">The mode to use for scaling the texture. Default is <see cref="ScaleMode.ScaleAndCrop"/>.</param>
        /// <param name="fadeTime">The time in seconds for the texture to fade in. Default is 0 (no fade).</param>
        /// <returns>Returns the current instance of <see cref="TextureLoader"/> with the specified settings.</returns>
        public TextureLoader Into(UnityEngine.UIElements.VisualElement target, UnityEngine.UIElements.VisualElement defaultPlaceholder, UnityEngine.UIElements.VisualElement loadingPlaceholder, UnityEngine.UIElements.VisualElement errorPlaceholder, ScaleMode scaleMode = ScaleMode.ScaleAndCrop, float fadeTime = 0)
            => Into(new UIToolkitTextureTarget(target, defaultPlaceholder, loadingPlaceholder, errorPlaceholder, scaleMode, fadeTime));

        /// <summary>
        /// Retrieves a list of targets of the specified type that inherit from <see cref="TextureTarget"/>.
        /// </summary>
        /// <typeparam name="T">The type of target to retrieve, which must inherit from <see cref="TextureTarget"/>.</typeparam>
        /// <returns>
        /// A list of targets of type <typeparamref name="T"/>. If there are no targets of the specified type,
        /// an empty list is returned.
        /// </returns>
        public List<T> GetTargets<T>() where T : TextureTarget
            => targets.Any() ? targets.OfType<T>().ToList() : new();

        /// <summary>
        /// Sets whether to use caching for texture loading and optionally specifies the file format,
        /// quality, and duration for which the texture should be cached.
        /// </summary>
        /// <param name="value">Indicates whether caching should be enabled (true) or disabled (false).</param>
        /// <param name="fileFormat">Optional. Specifies the file format to be used when caching the texture.</param>
        /// <param name="quality">Optional. Specifies the quality level for the cached texture. Only applies to JPG.</param>
        /// <param name="duration">Optional. Specifies the duration for which the texture should be cached.</param>
        /// <returns>Returns the current instance of <see cref="TextureLoader"/> with the updated cache settings.</returns>
        public TextureLoader UseCache(bool value, FileFormat? fileFormat = null, int? quality = null, TimeSpan? duration = null)
        {
            isCacheEnabled = value;
            cacheFormat = fileFormat ?? cacheFormat;
            cacheQuality = quality ?? cacheQuality;
            cacheDuration = duration ?? cacheDuration;
            return this;
        }

        /// <summary>
        /// Sets a callback that will be invoked once the texture loading starts.
        /// </summary>
        /// <returns>Returns the current instance of <see cref="TextureLoader"/> with the added callback.</returns>
        public TextureLoader OnStart(Action action)
        {
            onStart = action;
            return this;
        }

        /// <summary>
        /// Sets a callback that will be invoked whenever the texture loading progresses.
        /// </summary>
        /// <returns>Returns the current instance of <see cref="TextureLoader"/> with the added callback.</returns>
        public TextureLoader OnProgress(Action<float> action)
        {
            onProgress = action;
            return this;
        }

        /// <summary>
        /// Sets a callback that will be invoked if the texture loading is canceled.
        /// </summary>
        /// <param name="action">The callback to invoke when the texture loading is canceled.</param>
        /// <returns>Returns the current instance of <see cref="TextureLoader"/> with the added callback.</returns>
        public TextureLoader OnCancel(Action action)
        {
            onCancel = action;
            return this;
        }

        /// <summary>
        /// Sets a callback that will be invoked once the texture loading completes.
        /// </summary>
        /// <param name="action">The callback to invoke once the texture loading completes.</param>
        /// <returns>Returns the current instance of <see cref="TextureLoader"/> with the added callback.</returns>
        public TextureLoader OnComplete(Action<Texture2D> action)
        {
            onComplete = action;
            return this;
        }

        /// <summary>
        /// Sets a callback that will be invoked if there's an error loading the texture.
        /// </summary>
        /// <param name="action">The callback to invoke if there's an error loading the texture.</param>
        /// <returns>Returns the current instance of <see cref="TextureLoader"/> with the added callback.</returns>
        public TextureLoader OnError(Action<string> action)
        {
            onError = action;
            return this;
        }

        private void ProgressHandler(string id, float value)
        {
            if (string.IsNullOrEmpty(id) || id != this.id) return;
            progress = Mathf.Clamp01(value);
            onProgress?.Invoke(value);
            OnLoadProgress?.Invoke(this, value);
        }

        private void CompleteHandler(string id, Texture2D texture)
        {
            if (string.IsNullOrEmpty(id) || id != this.id) return;

            if (Logger.CanLogInfo)
            {
                Logger.Info($"Successfully loaded texture in {Time.realtimeSinceStartup - startTime} second(s). {logContext}");
            }

            UnbindRequestHandlers();

            this.texture = texture;

            // Update memory reference.
            if (textures.ContainsKey(id))
            {
                textures[id].Increase();
                if (Logger.CanLogDebug) Logger.Debug($"Texture references increased to {textures[id].Count}. {logContext}");
            }
            else
            {
                textures.Add(id, new(texture));
                if (Logger.CanLogDebug) Logger.Debug($"Created texture reference. {logContext}");
            }

            foreach (TextureTarget target in targets) target?.SetTexture(texture);

            if (Logger.CanLogDebug) Logger.Debug($"Texture set on {targets.Count} target(s). {logContext}");

            inProgress = false;
            isDone = true;

            if (progress < 1)
            {
                progress = 1;
                onProgress?.Invoke(progress);
                OnLoadProgress?.Invoke(this, progress);
            }

            onComplete?.Invoke(texture);
            OnLoadComplete?.Invoke(this, texture);

            YieldAndStartNextInQueue();
        }

        private void ErrorHandler(string id, string error)
        {
            if (string.IsNullOrEmpty(id) || id != this.id) return;

            if (string.IsNullOrEmpty(error))
            {
                error = "N/A.";
            }
            else if (!error.EndsWith("."))
            {
                error += ".";
            }

            if (Logger.CanLogErrors)
            {
                Logger.Error($"Failed to load the texture. Error: {error} {logContext}");
            }

            UnbindRequestHandlers();

            foreach (TextureTarget target in targets) target?.DisplayErrorPlaceholder();

            inProgress = false;
            isDone = true;

            onError?.Invoke(error);
            OnLoadError?.Invoke(this, error);

            YieldAndStartNextInQueue();
        }

        private async void YieldAndStartNextInQueue()
        {
            await Task.Yield();
            isWaitingForNextInQueue = false;
            StartNextInQueue();
        }

        private void BindRequestHandlers()
        {
            TextureRequest.OnRequestProgress += ProgressHandler;
            TextureRequest.OnRequestComplete += CompleteHandler;
            TextureRequest.OnRequestError += ErrorHandler;
        }

        private void UnbindRequestHandlers()
        {
            TextureRequest.OnRequestProgress -= ProgressHandler;
            TextureRequest.OnRequestComplete -= CompleteHandler;
            TextureRequest.OnRequestError -= ErrorHandler;
        }

        private void DoDispose()
        {
            if (isDisposed) return;

            Cancel();

            foreach (TextureTarget target in targets) target?.Dispose();

            if (Logger.CanLogDebug) Logger.Debug($"Disposed {targets.Count} target(s). {logContext}");
            targets.Clear();

            texture = null;

            if (!string.IsNullOrEmpty(id) && textures.ContainsKey(id))
            {
                TextureReference reference = textures[id];
                reference.Decrease();

                if (reference.Count < 1)
                {
                    GameObject.Destroy(reference.Texture);
                    textures.Remove(id);
                    if (Logger.CanLogDebug) Logger.Debug($"No more references, texture destroyed. {logContext}");
                }
                else
                {
                    if (Logger.CanLogDebug) Logger.Debug($"Texture references decreased to {reference.Count}. {logContext}");
                }
            }

            onStart = null;
            onProgress = null;
            onComplete = null;
            isDisposed = true;
            instances.Remove(this);

            if (Logger.CanLogInfo) Logger.Info($"Successfully disposed the texture loader. {logContext}");
        }

        private string GetLogContext()
        {
            Dictionary<string, object> context = new()
            {
                { nameof(id), id },
                { nameof(url), url },
                { nameof(minSize), minSize },
                { nameof(maxSize), maxSize },
                { nameof(mipmapChain), mipmapChain },
                { nameof(readable), readable },
                { nameof(wrapMode), wrapMode },
                { nameof(compression), compression },
                #if UNITY_6000_0_OR_NEWER
                { nameof(linearColorSpace), linearColorSpace },
                #endif
                { nameof(isCacheEnabled), isCacheEnabled },
                { nameof(cacheDirectory), cacheDirectory },
                { nameof(cacheSubdirectory), cacheSubdirectory },
                { nameof(cacheFormat), cacheFormat },
                { nameof(cacheQuality), cacheQuality },
                { nameof(cacheDuration), cacheDuration },
                { nameof(targets), targets.Count }
            };

            return context.ToJson();
        }

        #region Static

        private static readonly MD5 md5 = MD5.Create();

        private static readonly List<TextureLoader> instances = new();
        private static readonly Dictionary<string, TextureRequest> requests = new();
        private static readonly Dictionary<string, TextureReference> textures = new();
        private static readonly List<TextureLoader> queue = new();

        private static bool isWaitingForNextInQueue;
        private static TextureLoader lastDequeued;

        /// <summary>
        /// Event triggered when a texture loading process starts.
        /// The <see cref="TextureLoader"/> instance responsible for the loading is passed as a parameter.
        /// </summary>
        public static event Action<TextureLoader> OnLoadStart;

        /// <summary>
        /// Event triggered to report the progress of a texture loading operation.
        /// The <see cref="TextureLoader"/> instance and the current progress as a float (0 to 1) are passed as parameters.
        /// </summary>
        public static event Action<TextureLoader, float> OnLoadProgress;

        /// <summary>
        /// Event triggered when a texture loading operation is canceled.
        /// The <see cref="TextureLoader"/> instance responsible for the operation is passed as a parameter.
        /// </summary>
        public static event Action<TextureLoader> OnLoadCancel;

        /// <summary>
        /// Event triggered when a texture loading operation completes successfully.
        /// The <see cref="TextureLoader"/> instance and the loaded <see cref="Texture2D"/> are passed as parameters.
        /// </summary>
        public static event Action<TextureLoader, Texture2D> OnLoadComplete;

        /// <summary>
        /// Event triggered when an error occurs during a texture loading operation.
        /// The <see cref="TextureLoader"/> instance and the error message as a string are passed as parameters.
        /// </summary>
        public static event Action<TextureLoader, string> OnLoadError;

        /// <summary>
        /// The current version of the <see cref="TextureLoader"/> class.
        /// </summary>
        public static readonly string Version = "1.1.0";

        /// <summary>
        /// The default logging level used.
        /// </summary>
        public static LogLevel DefaultLogLevel { get; private set; } = LogLevel.Info | LogLevel.Error;

        /// <summary>
        /// The default minimum size (in pixels) allowed for textures loaded.
        /// An error will be returned for textures that don't meet this value.
        /// </summary>
        public static int DefaultMinSize = 0;

        /// <summary>
        /// The default maximum size (in pixels) allowed for textures loaded.
        /// Textures will be resized (preserving aspect ratio) if they exceed this value.
        /// </summary>
        public static int DefaultMaxSize = 2048;

        /// <summary>
        /// The default setting indicating whether mipmap chains should be generated for textures loaded.
        /// </summary>
        public static bool DefaultMipmapChain = true;

        /// <summary>
        /// The default setting indicating whether textures loaded should be readable by the CPU.
        /// <br/>https://docs.unity3d.com/ScriptReference/Texture-isReadable.html
        /// </summary>
        public static bool DefaultReadable = false;

        /// <summary>
        /// The default wrap mode to be used for textures loaded.
        /// <br/>https://docs.unity3d.com/ScriptReference/TextureWrapMode.html
        /// </summary>
        public static TextureWrapMode DefaultWrapMode = TextureWrapMode.Clamp;

        /// <summary>
        /// The default wrap mode to be used for textures loaded.
        /// <br/>https://docs.unity3d.com/ScriptReference/Texture-filterMode.html
        /// </summary>
        public static FilterMode DefaultFilterMode = FilterMode.Bilinear;

        /// <summary>
        /// The default anisotropic filtering level to be used for textures loaded.
        /// <br/>https://docs.unity3d.com/ScriptReference/Texture-anisoLevel.html
        /// </summary>
        public static int DefaultAnisoLevel = 0;

        /// <summary>
        /// The default compression setting to be used for textures loaded.
        /// <br/>https://docs.unity3d.com/ScriptReference/Texture2D.Compress.html
        /// </summary>
        public static TextureCompression DefaultCompression = TextureCompression.NormalQuality;

#if UNITY_6000_0_OR_NEWER
        /// <summary>
        /// The default linear color space to be used for textures loaded.
        /// <br/>https://docs.unity3d.com/6000.0/Documentation/Manual/linear-color-space.html
        /// </summary>
        public static bool DefaultLinearColorSpace = DownloadedTextureParams.Default.linearColorSpace;
#endif

        /// <summary>
        /// The default setting indicating whether texture caching is enabled for textures loaded.
        /// </summary>
        public static bool DefaultUseCache = true;

        /// <summary>
        /// The default directory where cached textures should be stored.
        /// </summary>
        public static DirectoryType DefaultCacheDirectory = DirectoryType.Temporary;

        /// <summary>
        /// The default subdirectory within the cache directory where cached textures should be stored.
        /// </summary>
        public static string DefaultCacheSubdirectory = nameof(TextureLoader);

        /// <summary>
        /// The default file format to be used when caching textures.
        /// </summary>
        public static FileFormat DefaultCacheFormat = FileFormat.PNG;

        /// <summary>
        /// The default quality setting (0-100) for cached textures when using lossy formats like JPG.
        /// </summary>
        public static int DefaultCacheQuality = 70;

        /// <summary>
        /// The default duration for which cached textures are retained before they are deleted and retrieved from the source.
        /// </summary>
        public static TimeSpan DefaultCacheDuration = TimeSpan.FromDays(7);

        /// <summary>
        /// The default cache path where textures are cached, determined by the default cache directory and subdirectory settings.
        /// </summary>
        public static string DefaultCachePath => GetCachePath(DefaultCacheDirectory, DefaultCacheSubdirectory);

        internal static Logger Logger { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            TextureLoaderSettings settings = Resources.Load<TextureLoaderSettings>(nameof(TextureLoaderSettings));

            if (settings != null)
            {
                DefaultLogLevel = settings.LogLevel;
                DefaultMinSize = settings.TextureDefaults.MinSize;
                DefaultMaxSize = settings.TextureDefaults.MaxSize;
                DefaultMipmapChain = settings.TextureDefaults.MipmapChain;
                DefaultReadable = settings.TextureDefaults.Readable;
                DefaultWrapMode = settings.TextureDefaults.WrapMode;
                DefaultFilterMode = settings.TextureDefaults.FilterMode;
                DefaultAnisoLevel = settings.TextureDefaults.AnisoLevel;
                DefaultCompression = settings.TextureDefaults.Compression;
                DefaultUseCache = settings.CacheDefaults.Enabled;
                DefaultCacheDirectory = settings.CacheDefaults.Directory;
                DefaultCacheSubdirectory = settings.CacheDefaults.Subdirectory;
                DefaultCacheFormat = settings.CacheDefaults.Format;
                DefaultCacheQuality = settings.CacheDefaults.Quality;
                DefaultCacheDuration = settings.CacheDefaults.Duration;

#if UNITY_6000_0_OR_NEWER
                DefaultLinearColorSpace = settings.TextureDefaults.LinearColorSpace;
#endif
            }

            TextureRequest.OnRequestDisposed += OnRequestDisposed;
            Logger = new(nameof(TextureLoader), DefaultLogLevel);
            if (Logger.CanLogInfo) Logger.Info($"Version: {Version}");
        }

        /// <summary>
        /// Loads a texture from a given URL.
        /// </summary>
        /// <param name="url">The URL of the texture to load. Must start with "file://" if the path is local.</param>
        /// <param name="settings">The settings for the texture to load.</param>
        public static TextureLoader Load(string url, TextureSettings settings) => new(url, settings);

        /// <summary>
        /// Loads a texture from a given URL.
        /// </summary>
        /// <param name="url">The URL of the texture to load. Must start with "file://" if the path is local.</param>
        public static TextureLoader Load(string url, int? maxSize = null, TextureCompression? compression = null, bool? readable = null)
            => new(url, maxSize ?? DefaultMaxSize, compression ?? DefaultCompression, readable ?? DefaultReadable);

        /// <summary>
        /// Starts a texture loader instance.
        /// </summary>
        /// <param name="instance">The texture loader instance to start.</param>
        /// <param name="enqueue">
        /// Optional. If true, the loading operation will be enqueued and executed later;
        /// if false, it will start immediately. Default is false.
        /// </param>
        public static void Start(TextureLoader instance, bool enqueue = false) => instance.Start(enqueue);

        /// <summary>
        /// Starts all texture loader instances.
        /// </summary>
        /// <param name="enqueue">
        /// Optional. If true, the loading operation will be enqueued and executed later;
        /// if false, it will start immediately. Default is false.
        /// </param>
        public static void StartAll(bool enqueue = false)
        {
            if (Logger.CanLogInfo)
            {
                string message = enqueue
                    ? $"Starting all texture loader instances in sequence..."
                    : $"Starting all texture loader instances...";

                Logger.Info(message);
            }

            for (int i = 0; i < instances.Count; i++) instances[i].Start(enqueue);
        }

        /// <summary>
        /// Cancels a texture loader instance.
        /// </summary>
        /// <param name="instance">The texture loader instance to cancel.</param>
        public static void Cancel(TextureLoader instance) => instance.Cancel();

        /// <summary>
        /// Cancels all texture loaders instances.
        /// </summary>
        public static void CancelAll()
        {
            if (Logger.CanLogInfo) Logger.Info($"Canceling all texture loader instances...");
            for (int i = 0; i < instances.Count; i++) instances[i].Cancel();
        }

        /// <summary>
        /// Disposes a texture loader instance.
        /// </summary>
        /// <param name="instance">The texture loader instance to dispose.</param>
        public static void Dispose(TextureLoader instance) => instance.Dispose();

        /// <summary>
        /// Disposes all texture loader instances.
        /// </summary>
        public static void DisposeAll()
        {
            if (Logger.CanLogInfo) Logger.Info($"Disposing all texture loader instances...");
            for (int i = instances.Count - 1; i >= 0; i--) instances[i].Dispose();
            instances.Clear();
        }

        /// <summary>
        /// Removes all texture files from disk cache.
        /// </summary>
        /// <param name="olderThan">Optional. The age of files to remove, based on when the file was created.</param>
        public static async void ClearCache(TimeSpan? olderThan = null)
        {
            if (olderThan == null)
            {
                olderThan = new();
                if (Logger.CanLogInfo) Logger.Info($"Clearing all disk cache...");
            }
            else
            {
                if (Logger.CanLogInfo) Logger.Info($"Clearing disk cache older than {olderThan}...");
            }

            DirectoryInfo directory = new(DefaultCachePath);

            foreach (FileInfo file in directory.GetFiles().Where(x => DateTime.Now - x.CreationTime > olderThan))
            {
                await DeleteFile(file.FullName);
            }
        }

        /// <summary>
        /// Removes the associated texture files from disk cache for a given URL and size.
        /// </summary>
        /// <param name="url">The URL of the associated texture file to remove.</param>
        /// <param name="size">The size of the texture to remove for the given URL.</param>
        public static async void ClearCache(string url, int? size = null)
        {
            if (Logger.CanLogInfo)
            {
                Logger.Info($"Clearing cache for {url}" + size == null ? "" : $" Size: {size}");
            }

            string hash = GetHash(url);
            string fileName = size == null ? "" : $"{hash}-{size}";

            DirectoryInfo directory = new(DefaultCachePath);

            foreach (FileInfo file in directory.GetFiles().Where(x => x.Name.StartsWith(fileName)))
            {
                await DeleteFile(file.FullName);
            }
        }

        private static async Task DeleteFile(string filePath)
        {
            string result = string.Empty;
            bool error = false;

            await Task.Run(() =>
            {
                try
                {
                    File.Delete(filePath);
                    if (Logger.CanLogInfo) result = $"File deleted: {filePath}";
                }
                catch (Exception ex)
                {
                    if (Logger.CanLogErrors) result = $"Failed to delete file: {filePath} Error: {ex}";
                    error = true;
                }
            });

            if (error)
            {
                if (Logger.CanLogErrors) Logger.Error(result);
                return;
            }

            if (Logger.CanLogInfo) Logger.Info(result);
        }

        private static void Request(TextureLoader loader, string url, string alternateUrl = "", string cacheFilePath = "")
        {
            string id = loader.Id;
            if (requests.ContainsKey(id)) return;
            TextureRequest request = new(loader, url, alternateUrl, cacheFilePath);
            request.Start();
            requests.Add(id, request);
        }

        private static void Enqueue(TextureLoader loader)
        {
            if (queue.Contains(loader)) return;

            queue.Add(loader);

            if (!isWaitingForNextInQueue)
            {
                StartNextInQueue();
                return;
            }

            if (Logger.CanLogInfo)
            {
                Logger.Info($"Enqueued texture loader instance: {loader.Url}");
            }
        }

        private static void StartNextInQueue()
        {
            if (!queue.Any()) return;

            TextureLoader loader = queue[0];

            if (loader != null)
            {
                if (Logger.CanLogInfo)
                {
                    Logger.Info($"Starting the next texture loader instance in queue...");
                }

                isWaitingForNextInQueue = true;
                loader.Start(false);
            }

            lastDequeued = loader;
            queue.RemoveAt(0);
        }

        private static void OnRequestDisposed(string id) => requests.Remove(id);

        private static string GetHash(string input)
        {
            byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sb = new();
            foreach (byte b in data) sb.Append(b.ToString("X2"));
            return sb.ToString();
        }

        private static string GetId(string hash, TextureLoader loader)
        {
            StringBuilder sb = new();
            sb.Append(hash);
            sb.Append("-");
            sb.Append(loader.MaxSize);
            sb.Append("-");
            sb.Append(loader.MipmapChain ? 1 : 0);
            sb.Append("-");
            sb.Append(loader.Readable ? 1 : 0);
            sb.Append("-");
            sb.Append((int)loader.WrapMode);
            sb.Append("-");
            sb.Append((int)loader.FilterMode);
            sb.Append("-");
            sb.Append((int)loader.Compression);
            return sb.ToString();
        }

        private static string GetCachePath(DirectoryType directory, string subdirectory)
        {
            string path = directory == DirectoryType.Persistent ? Application.persistentDataPath : Application.temporaryCachePath;
            path += "/";
            if (string.IsNullOrEmpty(subdirectory)) return path;
            path += subdirectory.Trim();
            return path.EndsWith("/") ? path : path + "/";
        }

        private static bool HasCacheFileExpired(string filePath, TimeSpan duration)
        {
            if (duration.Ticks < 0) return false;
            DateTime created = File.GetCreationTime(filePath);
            DateTime expires = created.Add(duration);
            return DateTime.Now > expires;
        }

        #endregion
    }
}
