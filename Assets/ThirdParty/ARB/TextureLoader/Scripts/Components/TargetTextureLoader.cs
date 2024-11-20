// Created by Andre Rene Biasi on 2024/09/01.
// Copyright (c) 2024 Andre Rene Biasi. All rights reserved.

using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace ARB.TextureLoader
{
    /// <summary>
    /// Base class to create components that can be attached to GameObjects with a renderer to load a texture into it.
    /// </summary>
    public abstract class TargetTextureLoader : MonoBehaviour
    {
        [SerializeField]
        protected bool loadOnAwake = true;

        [SerializeField]
        protected string url = string.Empty;

        [Space()]

        [SerializeField]
        protected Texture2D defaultTexture;

        [SerializeField]
        protected Texture2D loadingTexture;

        [SerializeField]
        protected Texture2D errorTexture;

        [SerializeField]
        protected float fadeTime = 0;

        [SerializeField]
        protected bool queue = true;

        [Space()]

        [SerializeField]
        protected TextureSettings textureSettings;

        [Space()]

        [SerializeField]
        protected CacheSettings cacheSettings;

        [Space()]

        public OnStartUnityEvent OnStart;
        public OnProgressUnityEvent OnProgress;
        public OnCompleteUnityEvent OnComplete;
        public OnErrorUnityEvent OnError;

        protected TextureLoader loader;
        protected abstract TextureTarget Target { get; }

        protected virtual void OnValidate()
        {
            if (Application.isPlaying && loader != null && Target != null) YieldAndLoad();
        }

        protected virtual void Awake()
        {
            if (loadOnAwake) Load();
        }

        protected virtual void OnDestroy()
        {
            Dispose();
        }

        public void Load()
        {
            loader?.Dispose();
            loader = TextureLoader.Load(url, textureSettings)
                .Into(Target)
                .UseCache(cacheSettings.Enabled, cacheSettings.Format, cacheSettings.Quality, cacheSettings.Duration)
                .OnStart(StartEventHandler)
                .OnProgress(ProgressEventHandler)
                .OnComplete(CompleteEventHandler)
                .OnError(ErrorEventHandler);
            loader.Start(queue);
        }

        public void Cancel() => loader?.Cancel();

        public void Dispose() => loader?.Dispose();

        private void StartEventHandler() => OnStart?.Invoke();

        private void ProgressEventHandler(float progress) => OnProgress?.Invoke(progress);

        private void CompleteEventHandler(Texture2D texture) => OnComplete?.Invoke(texture);

        private void ErrorEventHandler(string error) => OnError?.Invoke(error);

        private async void YieldAndLoad()
        {
            await Task.Yield();
            Load();
        }
    }

    [Serializable]
    public class OnStartUnityEvent : UnityEvent { }

    [Serializable]
    public class OnProgressUnityEvent : UnityEvent<float> { }

    [Serializable]
    public class OnCompleteUnityEvent : UnityEvent<Texture2D> { }

    [Serializable]
    public class OnErrorUnityEvent : UnityEvent<string> { }
}
