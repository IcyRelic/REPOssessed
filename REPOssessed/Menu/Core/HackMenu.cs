using REPOssessed.Menu.Popup;
using REPOssessed.Menu.Tab;
using REPOssessed.Util;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace REPOssessed.Menu.Core
{
    internal class HackMenu : MenuFragment
    {

        public Rect windowRect = new Rect(50f, 50f, 700f, 450f);

        public PopupMenu FirstSetupManagerWindow = new FirstSetupManagerWindow(1);
        public PopupMenu ItemManagerWindow = new ItemManagerWindow(2);
        public PopupMenu LootManagerWindow = new LootManagerWindow(3);
        public PopupMenu LevelManagerWindow = new LevelManagerWindow(4);

        private Vector2 scrollPos = Vector2.zero;
        private List<MenuTab> tabs = new List<MenuTab>();
        private int selectedTab = 0;
        public float contentWidth;
        public float contentHeight;
        public int spaceFromTop = 60;
        public int spaceFromLeft = 10;

        private static HackMenu instance;
        public static HackMenu Instance
        {
            get
            {
                if (instance == null) instance = new HackMenu();
                return instance;
            }
        }

        public HackMenu()
        {
            instance = this;
            tabs.Add(new SettingsTab());
            tabs.Add(new GeneralTab());
            tabs.Add(new SelfTab());
            tabs.Add(new VisualTab());
            tabs.Add(new PlayersTab());
            tabs.Add(new EnemyTab());
            tabs.Add(new ServerTab());
            if (Settings.b_DebugMode) tabs.Add(new DebugTab());
        }

        public void ToggleDebugTab(bool enabled)
        {
            if (enabled && !tabs.Any(t => t is DebugTab)) tabs.Add(new DebugTab());
            else tabs.RemoveAll(t => t is DebugTab);
        }

        public void Resize()
        {
            windowRect.width = Settings.i_menuWidth;
            windowRect.height = Settings.i_menuHeight;
            contentWidth = windowRect.width - (spaceFromLeft * 2);
            contentHeight = windowRect.height - spaceFromTop;
        }

        public void ResetMenuSize()
        {
            Settings.i_menuFontSize = 14;
            Settings.i_menuWidth = 810;
            Settings.i_menuHeight = 410;
            Settings.i_sliderWidth = 100;
            Settings.i_textboxWidth = 85;
        }

        public void Stylize()
        {
            GUI.skin = ThemeUtil.Skin;
            GUI.color = Color.white;

            GUI.skin.label.fontSize = Settings.i_menuFontSize;
            GUI.skin.button.fontSize = Settings.i_menuFontSize;
            GUI.skin.toggle.fontSize = Settings.i_menuFontSize;
            GUI.skin.box.fontSize = Settings.i_menuFontSize;
            GUI.skin.textField.fontSize = Settings.i_menuFontSize;
            GUI.skin.horizontalSlider.fontSize = Settings.i_menuFontSize;
            GUI.skin.horizontalSliderThumb.fontSize = Settings.i_menuFontSize;
            GUI.skin.verticalSlider.fontSize = Settings.i_menuFontSize;
            GUI.skin.verticalSliderThumb.fontSize = Settings.i_menuFontSize;

            GUI.skin.customStyles.Where(x => x.name == "TabBtn").First().fontSize = Settings.i_menuFontSize;
            GUI.skin.customStyles.Where(x => x.name == "SelectedTab").First().fontSize = Settings.i_menuFontSize;

            Resize();
        }


        public void Draw()
        {
            if (Settings.b_IsFirstLaunch || Settings.b_isMenuOpen) Stylize(); else return;
            if (Settings.b_IsFirstLaunch) FirstSetupManagerWindow.Draw();
            else
            {
                GUI.color = new Color(1f, 1f, 1f, Settings.f_menuAlpha);
                windowRect = GUILayout.Window(0, windowRect, new GUI.WindowFunction(DrawContent), "REPOssessed");
                ItemManagerWindow.Draw();
                LootManagerWindow.Draw();
                LevelManagerWindow.Draw();
                GUI.color = Color.white;
            }
            if (!Cursor.visible) Cursor.visible = true;
            if (Cursor.lockState != CursorLockMode.None) Cursor.lockState = CursorLockMode.None;
        }

        private void DrawContent(int windowID)
        {
            GUI.color = new Color(1f, 1f, 1f, 0.1f);
            GUIStyle watermark = new GUIStyle(GUI.skin.label) { fontSize = 20, fontStyle = FontStyle.Bold };
            string text = "Dustin";

            GUI.Label(new Rect(windowRect.width - watermark.CalcSize(new GUIContent(text)).x - 10, windowRect.height - watermark.CalcSize(new GUIContent(text)).y - 10, watermark.CalcSize(new GUIContent(text)).x, watermark.CalcSize(new GUIContent(text)).y), text, watermark);
            GUI.color = new Color(1f, 1f, 1f, Settings.f_menuAlpha);

            GUILayout.BeginVertical();

            GUILayout.BeginArea(new Rect(0, 25, windowRect.width, 25), style: "Toolbar");

            GUILayout.BeginHorizontal();
            selectedTab = GUILayout.Toolbar(selectedTab, tabs.Select(x => x.name).ToArray(), style: "TabBtn");
            GUILayout.EndHorizontal();

            GUILayout.EndArea();

            GUILayout.Space(spaceFromTop);

            GUILayout.BeginArea(new Rect(spaceFromLeft, spaceFromTop, windowRect.width - spaceFromLeft, contentHeight - 15));

            scrollPos = GUILayout.BeginScrollView(scrollPos);

            GUILayout.BeginHorizontal();
            tabs[selectedTab].Draw();
            GUILayout.EndHorizontal();

            GUILayout.EndScrollView();

            GUILayout.EndArea();

            GUI.color = Color.white;

            GUI.DragWindow();
        }
    }
}
