using System;
using System.Collections.Generic;
using System.Text;

namespace ARB.TextureLoader.Extensions
{
    /// <summary>
    /// Extension methods for the Dictionary class.
    /// </summary>
    internal static class DictionaryExtensions
    {
        public static string ToJson(this Dictionary<string, object> dictionary)
        {
            StringBuilder json = new();
            json.Append("{");

            foreach (var kvp in dictionary)
            {
                json.Append($"\"{EscapeString(kvp.Key)}\":");
                json.Append(ConvertValueToJson(kvp.Value));
                json.Append(",");
            }

            if (json.Length > 1)
            {
                json.Length--; // Remove the trailing comma
            }

            json.Append("}");
            return json.ToString();
        }

        private static string ConvertValueToJson(object value)
        {
            if (value == null)
            {
                return "null";
            }

            if (value is string str)
            {
                return $"\"{EscapeString(str)}\"";
            }

            if (value is bool boolean)
            {
                return boolean.ToString().ToLower();
            }

            if (value is Enum || value is DateTime || value is TimeSpan)
            {
                return $"\"{value}\"";
            }

            if (value is IDictionary<string, object> dict)
            {
                return ToJson(new Dictionary<string, object>(dict));
            }

            if (value is IEnumerable<object> list)
            {
                StringBuilder json = new();
                json.Append("[");

                foreach (var item in list)
                {
                    json.Append(ConvertValueToJson(item));
                    json.Append(",");
                }

                if (json.Length > 1)
                {
                    json.Length--; // Remove the trailing comma
                }

                json.Append("]");
                return json.ToString();
            }

            return value.ToString();
        }

        private static string EscapeString(string str)
        {
            return str.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\b", "\\b")
                      .Replace("\f", "\\f").Replace("\n", "\\n").Replace("\r", "\\r")
                      .Replace("\t", "\\t");
        }
    }
}
