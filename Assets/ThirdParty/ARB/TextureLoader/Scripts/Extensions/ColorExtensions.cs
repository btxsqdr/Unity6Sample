using UnityEngine;

namespace ARB.TextureLoader.Extensions
{
    /// <summary>
    /// Extension methods for the Color class.
    /// </summary>
    public static class ColorExtensions
    {
        public static Color WithAlpha(this Color color, float alpha)
        {
            color.a = Mathf.Clamp01(alpha);
            return color;
        }

        public static string ToHex(this Color color)
        {
            Color32 c = color;
            return string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", c.r, c.g, c.b, c.a);
        }
    }
}
