using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace REPOssessed
{
    public class Loader : MonoBehaviour
    {
        private static GameObject Load;
        public static bool harmonyLoaded = false;

        public static void Init()
        {
            if (Load != null)
            {
                Debug.LogError("REPOssessed is already injected");
                return;
            }
            LoadHarmony();
            Loader.Load = new GameObject();
            Load.AddComponent<REPOssessed>();
            Object.DontDestroyOnLoad(Loader.Load);
        }

        public static void LoadHarmony()
        {
            String name = "REPOssessed.Resources.0Harmony.dll";
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream stream = assembly.GetManifestResourceStream(name);
            byte[] rawAssembly = new byte[stream.Length];
            stream.Read(rawAssembly, 0, (int)stream.Length);

            AppDomain.CurrentDomain.Load(rawAssembly);
            harmonyLoaded = true;
        }
         
        public static void Unload() => Object.Destroy(Load);
    }
}
