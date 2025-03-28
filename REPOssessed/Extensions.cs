﻿using Photon.Pun;
using REPOssessed.Util;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Object = UnityEngine.Object;

namespace REPOssessed.Extensions
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

        public static T GetComponentHierarchy<T>(this Component component) 
        {
            T _component = component.GetComponent<T>();
            if (_component != null) return _component;
            _component = component.GetComponentInChildren<T>();
            if (_component != null) return _component;
            _component = component.GetComponentInParent<T>();
            return _component;
        }

        public static List<T> GetComponentsHierarchy<T>(this Component component)
        {
            List<T> components = new List<T>();
            components.Add(component.GetComponent<T>());
            components.Add(component.GetComponentInChildren<T>());
            components.Add(component.GetComponentInParent<T>());
            return components;
        }

        public static T GetComponentHierarchy<T>(this GameObject gameObject)
        {
            T component = gameObject.GetComponent<T>();
            if (component != null) return component;
            component = gameObject.GetComponentInChildren<T>();
            if (component != null) return component;
            component = gameObject.GetComponentInParent<T>();
            return component;
        }

        public static List<T> GetComponentsHierarchy<T>(this GameObject gameObject)
        {
            List<T> components = new List<T>();
            components.Add(gameObject.GetComponent<T>());
            components.Add(gameObject.GetComponentInChildren<T>());
            components.Add(gameObject.GetComponentInParent<T>());
            return components;
        }

        public static void CompleteExtraction(this ExtractionPoint extraction)
        {
            if (SemiFunc.IsMultiplayer()) extraction.Reflect().GetValue<PhotonView>("photonView").RPC("StateSetRPC", RpcTarget.All, ExtractionPoint.State.Complete);
             else extraction.StateSetRPC(ExtractionPoint.State.Complete);
        }

        public static GameObject Spawn(this string path, Vector3 position = default, Quaternion rotation = default)
        {
            return PhotonNetwork.InstantiateRoomObject(path, position, rotation);
        }

        public static GameObject Spawn(this GameObject gameObject, Vector3 position = default, Quaternion rotation = default)
        {
            return Object.Instantiate(gameObject, position, rotation);
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