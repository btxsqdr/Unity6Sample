namespace ARB.TextureLoader.Extensions
{
    /// <summary>
    /// Extension methods for arrays.
    /// </summary>
    internal static class ArrayExtensions
    {
        public static T[] Add<T>(this T[] array, T item)
        {
            T[] newArray = new T[array.Length + 1];

            for (int i = 0; i < array.Length; i++)
            {
                newArray[i] = array[i];
            }

            newArray[^1] = item;
            return newArray;
        }

        public static T[] RemoveAt<T>(this T[] array, int index)
        {
            if (index < 0 || index >= array.Length) return array;

            T[] newArray = new T[array.Length - 1];

            for (int i = 0, j = 0; i < array.Length; i++)
            {
                if (i == index) continue;
                newArray[j++] = array[i];
            }

            return newArray;
        }
    }
}
