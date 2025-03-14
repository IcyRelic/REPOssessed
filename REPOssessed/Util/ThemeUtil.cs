using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace REPOssessed.Util
{
    public class ThemeUtil
    {
        public static string name { get; set; }
        public static GUISkin Skin { get; set; }
        public static AssetBundle AssetBundle { get; set; }
        private static string Resource =>  $"{Assembly.GetExecutingAssembly().GetName().Name}.Resources.Theme.";

        public static void Initialize() => SetTheme("Default");

        private static bool ThemeExists(string theme) => Assembly.GetExecutingAssembly().GetManifestResourceStream($"{Resource}{theme}.skin") != null;

        private static AssetBundle LoadAssetBundle(string theme) => AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream(theme));

        public static string[] GetThemes() => Assembly.GetExecutingAssembly().GetManifestResourceNames().Where(r => r.StartsWith(Resource) && r.EndsWith(".skin")).Select(r => r.Replace(Resource, "").Replace(".skin", "")).OrderBy(n => n).ToArray();

        public static void SetTheme(string theme)
        {
            if (name == theme)
            {
                Debug.LogError($"[ERROR] Theme {theme} already loaded");
                return;
            }
            name = ThemeExists(theme) ? theme : "Default";
            AssetBundle?.Unload(true);
            AssetBundle = LoadAssetBundle($"{Resource}{theme}.skin");
            if (AssetBundle == null)
            {
                Debug.LogError($"[ERROR] Failed to load Theme {theme}");
                return;
            }
            Skin = AssetBundle.LoadAllAssets<GUISkin>().FirstOrDefault();
            if (Skin == null)
            {
                Debug.LogError($"[ERROR] Failed to load Theme {theme}");
                return;
            }
            Debug.Log($"Loaded Theme {theme}");
        }
    }
}
