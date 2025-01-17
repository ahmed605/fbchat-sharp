﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Reflection;

namespace fbchat_sharp
{
    internal static class Extensions
    {
        public static JToken get(this JToken token, string key = null)
        {
            if (key != null)
            {
                return token.Type != JTokenType.Null && token[key] != null && token[key].Type != JTokenType.Null ?
                    token[key] : null;
            }
            return token.Type != JTokenType.Null ? token : null;
        }

        public static Dictionary<string, List<Cookie>> GetAllCookies(this CookieContainer container)
        {
            var allCookies = new Dictionary<string, List<Cookie>>();
            var domainTableField = container.GetType().GetRuntimeFields().FirstOrDefault(x => x.Name == "m_domainTable");
            var domains = (IDictionary)domainTableField.GetValue(container);

            foreach (var val in domains.Values)
            {
                var type = val.GetType().GetRuntimeFields().First(x => x.Name == "m_list");
                var values = (IDictionary)type.GetValue(val);

                foreach (CookieCollection cookies in values.Values)
                {
                    var domain = cookies.Cast<Cookie>().FirstOrDefault()?.Domain;
                    if (domain == null) continue;
                    if (!allCookies.ContainsKey(domain))
                        allCookies[domain] = new List<Cookie>();
                    allCookies[domain].AddRange(cookies.Cast<Cookie>());
                }
            }
            return allCookies;
        }

        public static TValue GetValueOrDefault<TKey, TValue>
            (this IDictionary<TKey, TValue> dictionary,
            TKey key,
            TValue defaultValue = default(TValue))
        {
            TValue value;
            return dictionary.TryGetValue(key, out value) ? value : defaultValue;
        }

        public static string GetEnumDescriptionAttribute<T>(this T value) where T : struct
        {
            // The type of the enum, it will be reused.
            Type type = typeof(T);

            // If T is not an enum, get out.
            if (!type.GetTypeInfo().IsEnum)
                throw new InvalidOperationException(
                    "The type parameter T must be an enum type.");

            // If the value isn't defined throw an exception.
            if (!Enum.IsDefined(type, value))
                throw new ArgumentException($"Invalid value {value}");

            // Get the static field for the value.
            FieldInfo fi = type.GetRuntimeField(value.ToString());

            // Get the description attribute, if there is one.
            return fi.GetCustomAttributes(typeof(DescriptionAttribute), true).
                Cast<DescriptionAttribute>().SingleOrDefault().Description;
        }
    }
}
