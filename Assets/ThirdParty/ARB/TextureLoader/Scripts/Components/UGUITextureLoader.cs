// Created by Andre Rene Biasi on 2024/09/01.
// Copyright (c) 2024 Andre Rene Biasi. All rights reserved.

using UnityEngine;

namespace ARB.TextureLoader
{
    /// <summary>
    /// Base class to create components that can be attached to GameObjects with a UGUI renderers to load a texture into it.
    /// </summary>
    public abstract class UGUITextureLoader : TargetTextureLoader
    {
        [Space()]

        [SerializeField]
        protected UGUITextureTarget.ScaleMode scaleMode = UGUITextureTarget.ScaleMode.None;

        [SerializeField]
        protected bool resizeToFitOnParentDimensionsChange;

        protected virtual void OnRectTransformDimensionsChange()
        {
            if (!resizeToFitOnParentDimensionsChange || loader == null) return;

            foreach (UGUITextureTarget target in loader.GetTargets<UGUITextureTarget>())
            {
                target.ResizeToFit();
            }
        }
    }
}
