// Created by Andre Rene Biasi on 2024/08/01.
// Copyright (c) 2024 Andre Rene Biasi. All rights reserved.

using UnityEngine.UIElements;

namespace ARB.TextureLoader
{
    /// <summary>
    /// Defines a UIToolkit placeholder.
    /// </summary>
    public class UIToolkitPlaceholder : Placeholder
    {
        protected readonly VisualElement visualElement;

        public UIToolkitPlaceholder(VisualElement visualElement)
        {
            this.visualElement = visualElement;
            SetActive(false);
        }

        public override bool IsActive => visualElement != null && visualElement.visible;

        public override void SetActive(bool value)
        {
            if (visualElement != null) visualElement.visible = value;
        }

        public override void SetAlpha(float value)
        {
            if (visualElement != null) visualElement.style.opacity = value;
        }

        public override void Dispose()
        {
            if (visualElement == null) return;
            VisualElement parent = visualElement.parent;
            parent.Remove(visualElement);
        }
    }
}
