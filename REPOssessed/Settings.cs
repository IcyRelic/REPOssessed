using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using REPOssessed.Cheats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using REPOssessed.Cheats.Core;
using REPOssessed.Util;
using Audial.Utils;
using REPOssessed.Menu.Core;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using REPOssessed.Language;

namespace REPOssessed
{
    internal class Settings
    {

        /* *    
         * Menu Settings
         * */
        public static string s_Version = "v1.0.0";
        public static bool b_IsFirstLaunch = true;

        public static bool b_isMenuOpen = false;
        public static int i_menuFontSize = 14;
        public static int i_menuWidth = 810;
        public static int i_menuHeight = 410;
        public static int i_sliderWidth = 100;
        public static int i_textboxWidth = 85;
        public static float f_menuAlpha = 1f;

        /* *
         * Hack Settings
         * */
        public static bool b_DebugMode = false;
        public static string s_DebugMessage = "";
        public static float f_inputMovementSpeed = 10f;

        /* *    
         * Color Settings
         * */
        public static RGBAColor c_primary = new RGBAColor(165, 55, 253, 1f);
        public static RGBAColor c_success = new RGBAColor(0, 255, 0, 1f);
        public static RGBAColor c_error = new RGBAColor(255, 0, 0, 1f);
        public static RGBAColor c_menuText = new RGBAColor(255, 255, 255, 1f);
        public static RGBAColor c_espPlayers = new RGBAColor(0, 255, 0, 1f);
        public static RGBAColor c_espItems = new RGBAColor(255, 255, 255, 1f);
        public static RGBAColor c_espEnemies = new RGBAColor(255, 0, 0, 1f);
        public static RGBAColor c_espCart = new RGBAColor(0, 0, 255, 1f);
        public static RGBAColor c_espExtractions = new RGBAColor(255, 165, 0, 1f);
        public static RGBAColor c_espDeathHeads = new RGBAColor(128, 0, 128, 1f);
        public static RGBAColor c_espTruck = new RGBAColor(0, 255, 255, 0.5f);

        public static bool b_PlayerESP = false;
        public static bool b_EnemyESP = false;
        public static bool b_ItemESP = false;
        public static bool b_CartESP = false;
        public static bool b_ExtractionESP = false;
        public static bool b_DeathHeadESP = false;
        public static bool b_TruckESP = false;

        public static bool b_DisplayMapObjects = false;
        public static bool b_DisplayDeathHeads = false;
        public static bool b_DisplaySpectators = false;

        internal class Config
        {
            private static string config = "REPOssessed.config.json";
            private static string defaultConf = "REPOssessed.default.config.json";
            public static void CreateConfigIfNotExists()
            {
                if (HasConfig()) return;
                SaveConfig();
            }

            public static void SaveDefaultConfig()
            {
                SaveConfig(defaultConf);
            }
            public static bool HasConfig()
            {
                return config != null && File.Exists(config);
            }

            public static void SaveConfig()
            {
                SaveConfig(config);
            }

            public static void SaveConfig(string conf)
            {
                Dictionary<string, string> keybinds = new Dictionary<string, string>();
                Dictionary<string, string> toggles = new Dictionary<string, string>();
                Dictionary<string, string> cheatValues = new Dictionary<string, string>();


                Cheat.instances.FindAll(c => !c.Hidden).ForEach(c =>
                {
                    if (c.HasKeybind) keybinds.Add(c.GetType().Name, c.keybind.ToString());
                    if (c is ToggleCheat) toggles.Add(c.GetType().Name, ((ToggleCheat)c).Enabled.ToString());

                    if (c.GetType().GetInterface(typeof(IVariableCheat<>).FullName) != null)
                    {
                        FieldInfo valueField = c.GetType().GetField("Value", BindingFlags.Static | BindingFlags.Public);
                        cheatValues.Add(c.GetType().Name, valueField.GetValue(c).ToString());
                    }
                });


                JObject json = new JObject();
                JObject settings = new JObject();
                JObject colors = new JObject();
                JObject cheatSettings = new JObject();


                colors["MenuText"] = JsonConvert.SerializeObject(c_menuText);
                colors["ESPPlayers"] = JsonConvert.SerializeObject(c_espPlayers);
                colors["ESPItems"] = JsonConvert.SerializeObject(c_espItems);
                colors["ESPEnemies"] = JsonConvert.SerializeObject(c_espEnemies);
                colors["ESPTraps"] = JsonConvert.SerializeObject(c_espCart);

                settings["Theme"] = ThemeUtil.name;
                settings["Language"] = LanguageUtil.Language.Name;
                settings["MenuFontSize"] = i_menuFontSize.ToString();
                settings["MenuWidth"] = i_menuWidth.ToString();
                settings["MenuHeight"] = i_menuHeight.ToString();
                settings["SliderWidth"] = i_sliderWidth.ToString();
                settings["TextboxWidth"] = i_textboxWidth.ToString();
                settings["MenuAlpha"] = f_menuAlpha.ToString();

                cheatSettings["DebugMode"] = b_DebugMode.ToString();
                cheatSettings["IsFirstLaunch"] = b_IsFirstLaunch.ToString();
                cheatSettings["PlayerESP"] = b_PlayerESP.ToString();
                cheatSettings["EnemyESP"] = b_EnemyESP.ToString();
                cheatSettings["ItemESP"] = b_ItemESP.ToString();
                cheatSettings["CartESP"] = b_CartESP.ToString();
                cheatSettings["ExtractionESP"] = b_ExtractionESP.ToString();
                cheatSettings["DeathHeadESP"] = b_DeathHeadESP.ToString();
                cheatSettings["TruckESP"] = b_TruckESP.ToString();
                cheatSettings["DisplayMapObjects"] = b_TruckESP.ToString();
                cheatSettings["DisplayDeathHeads"] = b_TruckESP.ToString();
                cheatSettings["DisplaySpectators"] = b_TruckESP.ToString();

                json["KeyBinds"] = JObject.FromObject(keybinds);
                json["Toggles"] = JObject.FromObject(toggles);
                json["Values"] = JObject.FromObject(cheatValues);
                json["CheatSettings"] = cheatSettings;
                json["Colors"] = colors;
                json["MenuSettings"] = settings;

                File.WriteAllText(conf, json.ToString());
            }

            public static void LoadConfig()
            {
                CreateConfigIfNotExists();

                string jsonStr = File.ReadAllText(config);
                JObject json = JObject.Parse(jsonStr);

                Debug.Log("Loading Keybinds...");
                if (json.TryGetValue("KeyBinds", out JToken keybindsToken))
                {
                    Cheat.instances.ForEach(c => c.keybind = c.defaultKeybind);
                    foreach (var item in keybindsToken.ToObject<Dictionary<string, string>>())
                    {
                        string s_cheat = item.Key;
                        string s_bind = item.Value;

                        KeyCode bind = Enum.Parse<KeyCode>(s_bind);

                        Cheat.instances.Find(c => c.GetType().Name == s_cheat).keybind = bind;
                    }
                }

                Debug.Log("Loading Toggles...");
                if (json.TryGetValue("Toggles", out JToken togglesToken))
                {
                    foreach (var item in togglesToken.ToObject<Dictionary<string, string>>())
                    {
                        string s_cheat = item.Key;
                        string s_bind = item.Value;

                        bool toggle = bool.TryParse(s_bind, out bool result) ? result : false;

                        if (toggle)
                        {
                            ToggleCheat c = Cheat.instances.Find(c => c.GetType().Name == s_cheat && c is ToggleCheat) as ToggleCheat;
                            if (!c.Enabled) c.Toggle();
                        }
                    }
                }

                Debug.Log("Loading Values...");
                if (json.TryGetValue("Values", out JToken valuesToken))
                {
                    foreach (var item in valuesToken.ToObject<Dictionary<string, string>>())
                    {
                        string s_cheat = item.Key;
                        string s_value = item.Value;

                        Cheat c = Cheat.instances.Find(c => c.GetType().Name == s_cheat);

                        if (c.GetType().GetInterface(typeof(IVariableCheat<>).FullName) != null)
                        {
                            FieldInfo valueField = c.GetType().GetField("Value", BindingFlags.Static | BindingFlags.Public);
                            valueField.SetValue(c, Convert.ChangeType(s_value, valueField.FieldType));
                        }
                    }
                }

                Debug.Log("Loading Cheat Settings...");
                if (json.TryGetValue("CheatSettings", out JToken cSettingsToken))
                {
                    JObject cheatSettings = cSettingsToken.ToObject<JObject>();

                    if (cheatSettings.TryGetValue("DebugMode", out JToken debugModeToken))
                    {
                        b_DebugMode = bool.Parse(debugModeToken.ToString());
                        HackMenu.Instance.ToggleDebugTab(b_DebugMode);
                    }

                    if (cheatSettings.TryGetValue("IsFirstLaunch", out JToken isFirstLaunchToken))
                        b_IsFirstLaunch = bool.Parse(isFirstLaunchToken.ToString());
                    if (cheatSettings.TryGetValue("PlayerESP", out JToken playerESPToken))
                        b_PlayerESP = bool.Parse(playerESPToken.ToString());
                    if (cheatSettings.TryGetValue("EnemyESP", out JToken enemyESPToken))
                        b_EnemyESP = bool.Parse(enemyESPToken.ToString());
                    if (cheatSettings.TryGetValue("ItemESP", out JToken itemESPToken))
                        b_ItemESP = bool.Parse(itemESPToken.ToString());
                    if (cheatSettings.TryGetValue("CartESP", out JToken cartESPToken))
                        b_CartESP = bool.Parse(cartESPToken.ToString());
                    if (cheatSettings.TryGetValue("ExtractionESP", out JToken extractionESPToken))
                        b_ExtractionESP = bool.Parse(extractionESPToken.ToString());
                    if (cheatSettings.TryGetValue("DeathHeadESP", out JToken deathHeadESPToken))
                        b_DeathHeadESP = bool.Parse(deathHeadESPToken.ToString());
                    if (cheatSettings.TryGetValue("TruckESP", out JToken truckESPToken))
                        b_TruckESP = bool.Parse(truckESPToken.ToString());
                    if (cheatSettings.TryGetValue("DisplayMapObjects", out JToken displayMapObjectsToken))
                        b_DisplayMapObjects = bool.Parse(displayMapObjectsToken.ToString());
                    if (cheatSettings.TryGetValue("DisplayDeathHeads", out JToken displayDeathHeadsToken))
                        b_DisplayDeathHeads = bool.Parse(displayDeathHeadsToken.ToString());
                    if (cheatSettings.TryGetValue("DisplaySpectators", out JToken displaySpectatorsToken))
                        b_DisplaySpectators = bool.Parse(displaySpectatorsToken.ToString());
                }

                    Debug.Log("Loading Colors...");
                if (json.TryGetValue("Colors", out JToken colorsToken))
                {
                    JObject colors = colorsToken.ToObject<JObject>();

                    if (colors.TryGetValue("MenuText", out JToken menuTextToken))
                        c_menuText = JsonConvert.DeserializeObject<RGBAColor>(menuTextToken.ToString());
                    if (colors.TryGetValue("ESPPlayers", out JToken espPlayersToken))
                        c_espPlayers = JsonConvert.DeserializeObject<RGBAColor>(espPlayersToken.ToString());
                    if (colors.TryGetValue("ESPItems", out JToken espItemsToken))
                        c_espItems = JsonConvert.DeserializeObject<RGBAColor>(espItemsToken.ToString());
                    if (colors.TryGetValue("ESPEnemies", out JToken espEnemiesToken))
                        c_espEnemies = JsonConvert.DeserializeObject<RGBAColor>(espEnemiesToken.ToString());
                    if (colors.TryGetValue("ESPCart", out JToken espCartToken))
                        c_espCart = JsonConvert.DeserializeObject<RGBAColor>(espCartToken.ToString());
                }

                Debug.Log("Loading Menu Settings...");
                if (json.TryGetValue("MenuSettings", out JToken settingsToken))
                {
                    JObject settings = settingsToken.ToObject<JObject>();

                    if (settings.TryGetValue("Language", out JToken languageToken))
                        LanguageUtil.SetLanguage(languageToken.ToString());
                    if (settings.TryGetValue("Theme", out JToken themeToken))
                        ThemeUtil.SetTheme(themeToken.ToString());
                    if (settings.TryGetValue("MenuFontSize", out JToken menuFontSizeToken))
                        i_menuFontSize = int.Parse(menuFontSizeToken.ToString());
                    if (settings.TryGetValue("MenuWidth", out JToken menuWidthToken))
                        i_menuWidth = int.Parse(menuWidthToken.ToString());
                    if (settings.TryGetValue("MenuHeight", out JToken menuHeightToken))
                        i_menuHeight = int.Parse(menuHeightToken.ToString());
                    if (settings.TryGetValue("SliderWidth", out JToken sliderWidthToken))
                        i_sliderWidth = int.Parse(sliderWidthToken.ToString());
                    if (settings.TryGetValue("TextboxWidth", out JToken textboxWidthToken))
                        i_textboxWidth = int.Parse(textboxWidthToken.ToString());
                    if (settings.TryGetValue("MenuAlpha", out JToken menuAlphaToken))
                        f_menuAlpha = float.Parse(menuAlphaToken.ToString());
                }
            }

            public static void OpenConfig()
            {
                CreateConfigIfNotExists();
                Process.Start("explorer.exe", config);
            }

            public static void RegenerateConfig()
            {
                if (HasConfig()) File.Delete(config);
                File.Copy(defaultConf, config);
                Cheat.instances.ForEach(c => c.keybind = c.defaultKeybind);
                LoadConfig();
            }
        }
    }
}
