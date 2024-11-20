namespace ARB.TextureLoader
{
    /// <summary>
    /// Compression setting for textures.
    /// </summary>
    public enum TextureCompression
    {
        /// <summary>
        /// Texture is not compressed.
        /// </summary>
        None,

        /// <summary>
        /// Texture is compressed to DXT/BCn or ETC formats, depending on the platform and if supported by the graphics card.
        /// </summary>
        NormalQuality,

        /// <summary>
        /// Texture is compressed to DXT/BCn or ETC formats, depending on the platform and if supported by the graphics card.
        /// Source texture is dithered during compression, which helps to reduce compression artifacts but is slightly slower.
        /// Ignored for ETC compression.
        /// </summary>
        HighQuality
    }
}
