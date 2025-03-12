using REPOssessed.Cheats;
using REPOssessed.Cheats.Core;
using REPOssessed.Language;
using REPOssessed.Menu.Core;
using REPOssessed.Util;
using System;
using UnityEngine;

namespace REPOssessed.Menu.Popup
{
    internal class FirstSetupManagerWindow : PopupMenu
    {
        private Vector2 scrollPos = Vector2.zero;

        private int selectedLanguage = -1;
        private string[] languages;
        private bool disableBtns = false;

        public FirstSetupManagerWindow(int id) : base("FirstSetupManager.Title", new Rect(Screen.width / 2 - 150, Screen.height / 2 - 100f, 300f, 250f), id)
        {
            languages = LanguageUtil.GetLanguages();
            isOpen = true;
        }

        public override void DrawContent(int windowID)
        {
            if (disableBtns) GUI.enabled = false;
            windowRect.x = Screen.width / 2 - 150;
            windowRect.y = Screen.height / 2 - 100f;

            UI.Label("FirstSetupManager.Welcome");
            GUILayout.Space(20f);
            GUILayout.BeginVertical();
            UI.Label("FirstSetupManager.SelectKeybind");

            UI.Label("FirstSetupManager.Keybind", $"{GetMenuKeybindName()}");
            GUILayout.Space(20f);

            string btnText = Cheat.Instance<ToggleMenu>().WaitingForKeybind ? "General.Waiting" : "FirstSetupManager.ClickToChange";

            UI.Actions(new UIButton(btnText, () =>
            {
                disableBtns = true;
                KBUtil.BeginChangeKeybind(Cheat.Instance<ToggleMenu>(), () => { disableBtns = false; });
            }));

            GUILayout.EndVertical();
            GUILayout.Space(20f);
            GUILayout.BeginVertical();
            UI.Label("FirstSetupManager.SelectLanguage");
            GUILayout.Space(10f);

            Rect rect = new Rect(8, 255, 300f, 100f);
            GUI.Box(rect, GUIContent.none);
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.Width(300f), GUILayout.Height(100f));
            scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Width(300f), GUILayout.Height(100f));

            if (selectedLanguage == -1) selectedLanguage = Array.FindIndex(languages, x => x == LanguageUtil.Language.Name);

            for (int i = 0; i < languages.Length; i++)
            {
                if (selectedLanguage == i) GUI.contentColor = Settings.c_primary.GetColor();

                if (GUILayout.Button(languages[i], GUI.skin.label))
                {
                    selectedLanguage = i;
                    LanguageUtil.SetLanguage(languages[i]);
                }

                GUI.contentColor = Settings.c_menuText.GetColor();
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.Space(20f);

            UI.Actions(new UIButton("FirstSetupManager.Complete", () =>
            {
                if (GetMenuKeybindName() == "None") return;
                Settings.b_IsFirstLaunch = false;
                Settings.Config.SaveConfig();
            }));
            if (disableBtns) GUI.enabled = true;
            GUI.DragWindow();
        }

        public static string GetMenuKeybindName()
        {
            return Cheat.Instance<ToggleMenu>().HasKeybind ? Cheat.Instance<ToggleMenu>().keybind.ToString() : KeyCode.None.ToString();
        }
    }
}