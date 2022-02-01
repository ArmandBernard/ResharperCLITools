using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

#nullable enable

namespace ResharperToolsLib
{
    internal static class Extensions
    {
        /// <summary>
        /// IEnumerable version of ForEach
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="action">The action to perform</param>
        public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            foreach (var item in list)
            {
                action(item);
            }
        }

        public static JToken? GetInsensitive(this JToken jToken, string childName)
        {
            return ((JObject)jToken).GetValue(childName, StringComparison.OrdinalIgnoreCase);
        }

        public static T? GetObjectInsensitive<T>(this JToken jToken, string childName, T? defaultValue = default)
        {
            var child = ((JObject)jToken).GetInsensitive(childName);

            return child != null ? child.ToObject<T>() : defaultValue;
        }
    }    
}
