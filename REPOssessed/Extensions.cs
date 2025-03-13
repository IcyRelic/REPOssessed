using REPOssessed.Util;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace REPOssessed
{
    public static class Extensions
    {
        private static readonly Dictionary<Type, Delegate> TryParseMethods = new Dictionary<Type, Delegate>()
        {
            { typeof(int), new TryParseDelegate<int>(int.TryParse) },
            { typeof(float), new TryParseDelegate<float>(float.TryParse) },
            { typeof(double), new TryParseDelegate<double>(double.TryParse) },
            { typeof(decimal), new TryParseDelegate<decimal>(decimal.TryParse) },
            { typeof(bool), new TryParseDelegate<bool>(bool.TryParse) },
            { typeof(DateTime), new TryParseDelegate<DateTime>(DateTime.TryParse) },
            { typeof(TimeSpan), new TryParseDelegate<TimeSpan>(TimeSpan.TryParse) },
            { typeof(ulong), new TryParseDelegate<ulong>(ulong.TryParse) },
            { typeof(long), new TryParseDelegate<long>(long.TryParse) },
        };

        public static T GetComponentHierarchy<T>(this GameObject obj) where T : Component
        {
            T component = obj.GetComponent<T>();
            if (component != null) return component;
            component = obj.GetComponentInChildren<T>(true);
            if (component != null) return component;
            component = obj.GetComponentInParent<T>();
            return component;
        }

        public static RaycastHit[] SphereCastForward(this Transform transform, float sphereRadius = 1.0f)
        {
            try
            {
                return Physics.SphereCastAll(transform.position + (transform.forward * (sphereRadius + 1.75f)), sphereRadius, transform.forward, float.MaxValue);
            }
            catch (NullReferenceException) { return null; }
        }

        public static bool Parse<T>(this string s, out T result) where T : struct, IConvertible, IComparable<T>
        {
            result = default(T);
            bool success = false;

            if (TryParseMethods.TryGetValue(typeof(T), out var method))
                success = ((TryParseDelegate<T>)method)(s, out result);

            return success;
        }

        private delegate bool TryParseDelegate<T>(string input, out T result);

        public static bool StateIs(this ExtractionPoint extraction, ExtractionPoint.State state) => extraction.Reflect().GetValue<ExtractionPoint.State>("currentState") == state;
    }
}