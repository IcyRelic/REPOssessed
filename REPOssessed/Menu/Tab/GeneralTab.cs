using UnityEngine;
using REPOssessed.Menu.Core;
using REPOssessed.Util;

namespace REPOssessed.Menu.Tab
{
    internal class GeneralTab : MenuTab
    {
        Vector2 scrollPos = Vector2.zero;
        public GeneralTab() : base("GeneralTab.Title") { }

        public override void Draw()
        {
            UI.VerticalSpace(ref scrollPos, () =>
            {
                UI.Header(Settings.c_primary.AsString("Welcome to REPOssessed!"), 30);
                GUILayout.Space(20);
                UI.Label("Developed by Dustin, receiving constant updates to better the menu!");
                GUILayout.Space(20);
            });
        }
    }
}
