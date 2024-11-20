// Created by Andre Rene Biasi on 2024/08/01.
// Copyright (c) 2024 Andre Rene Biasi. All rights reserved.

namespace ARB.TextureLoader
{
    /// <summary>
    /// Base class for defining <see cref="TextureTarget"/> placeholders.
    /// </summary>
    public abstract class Placeholder
    {
        /// <summary>
        /// Gets a value indicating whether the placeholder is currently active.
        /// </summary>
        public abstract bool IsActive { get; }

        /// <summary>
        /// Sets the active state of the placeholder.
        /// </summary>
        /// <param name="value">If true, the placeholder is activated; otherwise, it is deactivated.</param>
        public abstract void SetActive(bool value);

        /// <summary>
        /// Sets the transparency level of the placeholder.
        /// </summary>
        /// <param name="value">The alpha value to set, where 0 is fully transparent and 1 is fully opaque.</param>
        public abstract void SetAlpha(float value);

        /// <summary>
        /// Releases all resources used by the placeholder.
        /// This method should be called when the placeholder is no longer needed.
        /// </summary>
        public abstract void Dispose();
    }
}
