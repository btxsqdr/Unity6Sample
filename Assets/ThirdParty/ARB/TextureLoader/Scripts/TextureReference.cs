using UnityEngine;

namespace ARB.TextureLoader
{
    internal class TextureReference
    {
        public Texture2D Texture { get; private set; }
        public int Count { get; private set; }

        internal TextureReference(Texture2D texture)
        {
            Texture = texture;
            Count = 1;
        }

        internal void Increase() => Count++;

        internal void Decrease() => Count--;
    }
}
