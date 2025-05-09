using System.Text.Json;

namespace ReportHub.Web.Helper
{
    public static class JsonHelper
    {
        public static string ToJson<T>(this T obj, JsonSerializerOptions options = null)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            return JsonSerializer.Serialize(obj, options);
        }

        public static T FromJson<T>(this string json, JsonSerializerOptions options = null)
        {
            if (string.IsNullOrWhiteSpace(json))
                throw new ArgumentException("JSON string cannot be null or empty.", nameof(json));

            return JsonSerializer.Deserialize<T>(json, options);
        }

        public static T FromJson<T>(this object json, JsonSerializerOptions options = null)
        {
            if (json is null)
                throw new ArgumentException("JSON string cannot be null or empty.", nameof(json));

            return JsonSerializer.Deserialize<T>(json.ToString(), options);
        }
    }
}
