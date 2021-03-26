using System;
using System.Text.Json;

namespace TestSysplan.Core
{
    public static class Serializer
    {
        private static JsonSerializerOptions GetJsonSerializerOptionsDefault()
        {
            return new JsonSerializerOptions
            {
                IgnoreReadOnlyFields = true,
                IgnoreReadOnlyProperties = true,
                IgnoreNullValues = true
            };
        }

        /// <summary>
        /// Serialize current object to json.
        /// </summary>
        /// <param name="target">target</param>
        /// <param name="options">serializer options</param>
        /// <returns></returns>
        public static string ToJson(this object target, JsonSerializerOptions options = null)
        {
            return JsonSerializer.Serialize(target,
                options ?? GetJsonSerializerOptionsDefault());
        }

        /// <summary>
        /// Serialize current object to json.
        /// </summary>
        /// <param name="target">target</param>
        /// <param name="options">serializer options</param>
        /// <returns></returns>
        public static byte[] ToJsonBytes(this object target, JsonSerializerOptions options = null)
        {
            return JsonSerializer.SerializeToUtf8Bytes(target,
                options ?? GetJsonSerializerOptionsDefault());
        }

        /// <summary>
        /// Deserialize json to object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json">json target</param>
        /// <param name="options">options</param>
        /// <returns></returns>
        public static T FromJson<T>(string json, JsonSerializerOptions options = null)
        {
            return JsonSerializer.Deserialize<T>(json,
                options ?? GetJsonSerializerOptionsDefault());
        }

        /// <summary>
        /// Deserialize json to object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json">json target</param>
        /// <param name="options">options</param>
        /// <returns></returns>
        public static T FromJson<T>(ReadOnlySpan<byte> json, JsonSerializerOptions options = null)
        {
            return JsonSerializer.Deserialize<T>(json,
                options ?? GetJsonSerializerOptionsDefault());
        }

        /// <summary>
        /// Deserialize json to object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json">json target</param>
        /// <param name="options">options</param>
        /// <returns></returns>
        public static T FromJson<T>(ReadOnlyMemory<byte> json, JsonSerializerOptions options = null)
        {
            return JsonSerializer.Deserialize<T>(json.Span,
                options ?? GetJsonSerializerOptionsDefault());
        }

    }
}
