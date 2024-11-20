// Created by Andre Rene Biasi on 2024/08/01.
// Copyright (c) 2024 Andre Rene Biasi. All rights reserved.

using ARB.TextureLoader.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace ARB.TextureLoader
{
    /// <summary>
    /// Defines a MaskableGraphic placeholder.
    /// </summary>
    public class MaskableGraphicPlaceholder : UGUIPlaceholder
    {
        protected readonly MaskableGraphic maskableGraphic;

        public MaskableGraphicPlaceholder(MaskableGraphic maskableGraphic)
        {
            texture = maskableGraphic.mainTexture;
            this.maskableGraphic = maskableGraphic;
            SetActive(false);
        }

        public override Vector2 SizeDelta
        {
            get => maskableGraphic != null ? maskableGraphic.rectTransform.sizeDelta : Vector2.zero;
            set
            {
                if (maskableGraphic != null) maskableGraphic.rectTransform.sizeDelta = value;
            }
        }

        public override bool IsActive => maskableGraphic != null && maskableGraphic.gameObject.activeSelf;

        public override void SetActive(bool value)
        {
            if (maskableGraphic != null && maskableGraphic.gameObject.activeSelf != value) maskableGraphic.gameObject.SetActive(value);
        }

        public override void SetAlpha(float alpha)
        {
            if (maskableGraphic != null) maskableGraphic.color = maskableGraphic.color.WithAlpha(alpha);
        }

        public override void Dispose()
        {
            if (maskableGraphic != null) GameObject.Destroy(maskableGraphic.gameObject);
        }
    }
}
