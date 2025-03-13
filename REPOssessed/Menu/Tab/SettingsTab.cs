using UnityEngine;
using REPOssessed.Menu.Core;
using REPOssessed.Util;
using REPOssessed.Language;
using System.Linq;
using System;
using System.Collections.Generic;
using UnityEngine.InputSystem.Controls;
using REPOssessed.Cheats.Core;
using REPOssessed.Cheats;

namespace REPOssessed.Menu.Tab
{
    internal class SettingsTab : MenuTab
    {
        public SettingsTab() : base("SettingsTab.Title") { }

        private Vector2 scrollPos = Vector2.zero;
        private Vector2 scrollPos2 = Vector2.zero;

        private int i_languageIndex = -1;
        private int i_themeIndex = -1;
        private float f_leftWidth;

        private string s_kbSearch = "";


        public override void Draw()
        {
            f_leftWidth = HackMenu.Instance.contentWidth * 0.55f - HackMenu.Instance.spaceFromLeft;

            if (i_languageIndex == -1) i_languageIndex = Array.IndexOf(LanguageUtil.GetLanguages(), LanguageUtil.Language.Name);
            if (i_themeIndex == -1) i_themeIndex = Array.IndexOf(ThemeUtil.GetThemes(), ThemeUtil.name);

            GUILayout.BeginVertical(GUILayout.Width(f_leftWidth));
            MenuContent();
            GUILayout.EndVertical();
            GUILayout.BeginVertical(GUILayout.Width(HackMenu.Instance.contentWidth * 0.45f - HackMenu.Instance.spaceFromLeft));
            KeybindContent();
            GUILayout.EndVertical();
        }

        private void MenuContent()
        {
            UI.VerticalSpace(ref scrollPos, () =>
            {
                UI.Header("SettingsTab.Title");

                UI.Actions(
                    new UIButton("SettingsTab.ResetSettings", () => Settings.Config.RegenerateConfig()),
                    new UIButton("SettingsTab.SaveSettings", () => Settings.Config.SaveConfig()),
                    new UIButton("SettingsTab.ReloadSettings", () => Settings.Config.LoadConfig())
                );

                UI.Actions(
                    new UIButton("SettingsTab.OpenSettings", () => Settings.Config.OpenConfig())
                );

                UI.Header("SettingsTab.GeneralSettings");

                UI.Select("SettingsTab.Theme", ref i_themeIndex, ThemeUtil.GetThemes().Select(x => new UIOption(x, () => ThemeUtil.SetTheme(x))).ToArray());
                UI.Select("SettingsTab.Language", ref i_languageIndex, LanguageUtil.GetLanguages().Select(x => new UIOption(x, () => LanguageUtil.SetLanguage(x))).ToArray());

                UI.Checkbox("SettingsTab.FPSCounter", Cheat.Instance<FPSCounter>());
                UI.Toggle("SettingsTab.DebugMode", ref Settings.b_DebugMode, "General.Enable", "General.Disable", HackMenu.Instance.ToggleDebugTab);
            });
        }

        private void KeybindContent()
        {
            UI.VerticalSpace(ref scrollPos2, () =>
            {
                UI.Header("SettingsTab.Keybinds");
                GUILayout.BeginVertical();

                UI.Textbox("SettingsTab.Search", ref s_kbSearch, big: false);

                List<Cheat> cheats = Cheat.instances.FindAll(c => !c.Hidden);
                foreach (Cheat cheat in cheats)
                {
                    if (!cheat.GetType().Name.ToLower().Contains(s_kbSearch.ToLower())) continue;

                    GUILayout.BeginHorizontal();

                    KeyCode bind = cheat.keybind;

                    string kb = cheat.HasKeybind ? bind.ToString() : "General.None".Localize();

                    GUILayout.Label(cheat.GetType().Name);
                    GUILayout.FlexibleSpace();

                    if (cheat.HasKeybind && GUILayout.Button("-")) cheat.keybind = KeyCode.None;

                    string btnText = cheat.WaitingForKeybind ? "General.Waiting" : kb;
                    if (GUILayout.Button(btnText, GUILayout.Width(85)))
                    {
                        GUI.FocusControl(null);
                        KBUtil.BeginChangeKeybind(cheat);
                    }

                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
            });
        }
    }
}
