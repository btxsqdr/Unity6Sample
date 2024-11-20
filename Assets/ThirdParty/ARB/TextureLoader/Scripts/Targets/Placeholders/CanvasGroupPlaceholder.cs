// Created by Andre Rene Biasi on 2024/08/01.
// Copyright (c) 2024 Andre Rene Biasi. All rights reserved.

using UnityEngine;

namespace ARB.TextureLoader
{
    /// <summary>
    /// Defines a CanvasGroup placeholder.
    /// </summary>
    public class CanvasGroupPlaceholder : UGUIPlaceholder
    {
        protected readonly CanvasGroup canvasGroup;

        public CanvasGroupPlaceholder(CanvasGroup canvasGroup)
        {
            this.canvasGroup = canvasGroup;
            SetActive(false);
        }

        public override Vector2 SizeDelta
        {
            get => canvasGroup != null ? ((RectTransform)canvasGroup.transform).sizeDelta : Vector2.zero;
            set
            {
                if (canvasGroup != null) ((RectTransform)canvasGroup.transform).sizeDelta = value;
            }
        }

        public override bool IsActive => canvasGroup != null && canvasGroup.gameObject.activeSelf;

        public override void SetActive(bool value)
        {
            if (canvasGroup != null && canvasGroup.gameObject.activeSelf != value) canvasGroup.gameObject.SetActive(value);
        }

        public override void SetAlpha(float alpha)
        {
            if (canvasGroup != null) canvasGroup.alpha = alpha;
        }

        public override void Dispose()
        {
            if (canvasGroup != null) GameObject.Destroy(canvasGroup);
        }
    }
}
