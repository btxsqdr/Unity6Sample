// Created by Andre Rene Biasi on 2024/08/01.
// Copyright (c) 2024 Andre Rene Biasi. All rights reserved.

using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace ARB.TextureLoader
{
    /// <summary>
    /// Base class for defining targets to which textures can be applied.
    /// </summary>
    public abstract class TextureTarget
    {
        protected readonly float fadeTime;
        protected Placeholder defaultPlaceholder;
        protected Placeholder loadingPlaceholder;
        protected Placeholder errorPlaceholder;
        protected Placeholder currentPlaceholder;
        protected CancellationTokenSource animationCTS;

        public TextureTarget(float fadeTime = 0f)
        {
            this.fadeTime = fadeTime;
        }

        protected abstract Texture CurrentTexture { get; set; }

        public abstract void SetAlpha(float alpha);

        public virtual void SetTexture(Texture2D texture, bool transition = true)
        {
            CancelAnimation();

            if (texture == null)
            {
                CurrentTexture = null;
                DisplayPlaceholder(null);
                return;
            }

            Texture currentTexture = CurrentTexture;
            CurrentTexture = texture;

            if (transition && fadeTime > 0)
            {
                if (currentTexture != null || currentPlaceholder != null)
                {
                    if (currentPlaceholder != null) currentTexture = null;
                    SetAlpha(1);
                    SetFaderTexture(currentTexture);
                    SetFaderAlpha(1);
                    Crossfade();
                    return;
                }

                SetAlpha(0);
                FadeIn();
                return;
            }

            SetAlpha(1);
            DisplayPlaceholder(null);
        }

        public virtual void DisplayDefaultPlaceholder()
        {
            if (defaultPlaceholder != null && !defaultPlaceholder.IsActive)
            {
                DisplayPlaceholder(defaultPlaceholder);
            }
        }

        public virtual void DisplayLoadingPlaceholder()
        {
            if (loadingPlaceholder != null && !loadingPlaceholder.IsActive)
            {
                DisplayPlaceholder(loadingPlaceholder);
            }
        }

        public virtual void DisplayErrorPlaceholder()
        {
            if (errorPlaceholder != null && !errorPlaceholder.IsActive)
            {
                DisplayPlaceholder(errorPlaceholder);
            }
        }

        public abstract void Dispose();

        protected abstract void SetFaderAlpha(float value);

        protected abstract void SetFaderTexture(Texture texture);

        protected void DisplayPlaceholder(Placeholder placeholder)
        {
            currentPlaceholder?.SetActive(false);
            currentPlaceholder = placeholder;
            currentPlaceholder?.SetActive(true);
            currentPlaceholder?.SetAlpha(1);
        }

        protected async void Crossfade()
        {
            void OnUpdate(float value)
            {
                value = 1 - value;

                if (currentPlaceholder != null)
                {
                    currentPlaceholder.SetAlpha(value);
                    return;
                }

                SetFaderAlpha(value);
            }

            void OnComplete()
            {
                SetFaderTexture(null);
                DisplayPlaceholder(null);
                animationCTS?.Dispose();
                animationCTS = null;
            }

            await Task.Yield();
            Lerp01(fadeTime, OnUpdate, OnComplete, animationCTS.Token);
        }

        protected async void FadeIn()
        {
            void OnUpdate(float value) => SetAlpha(value);

            void OnComplete()
            {
                animationCTS?.Dispose();
                animationCTS = null;
            }

            await Task.Yield();
            Lerp01(fadeTime, OnUpdate, OnComplete, animationCTS.Token);
        }

        protected void CancelAnimation()
        {
            animationCTS?.Cancel();
            animationCTS = new();
        }

        protected async void Lerp01(float duration, Action<float> onUpdate, Action onComplete, CancellationToken cancellationToken)
        {
            float a = 0;
            float b = 1;
            float value = a;
            float time = Time.time;

            while (value < b)
            {
                value = Mathf.Lerp(a, b, (Time.time - time) / duration);
                onUpdate?.Invoke(value);
                await Task.Yield();
                if (cancellationToken.IsCancellationRequested || !Application.isPlaying) return;
            }

            onComplete?.Invoke();
        }
    }
}
