// Created by Andre Rene Biasi on 2024/08/01.
// Copyright (c) 2024 Andre Rene Biasi. All rights reserved.

using UnityEngine;

namespace ARB.TextureLoader
{
    /// <summary>
    /// Defines a base class for UGUI placeholders.
    /// </summary>
    public abstract class UGUIPlaceholder : Placeholder
    {
        /// <summary>
        /// The texture associated with this UGUI placeholder.
        /// </summary>
        protected Texture texture;

        /// <summary>
        /// Gets the texture currently assigned to the placeholder.
        /// </summary>
        public Texture Texture => texture;

        /// <summary>
        /// Gets or sets the size delta of the UGUI placeholder,
        /// which controls the size of the placeholder element relative to its parent.
        /// </summary>
        public abstract Vector2 SizeDelta { get; set; }
    }
}
