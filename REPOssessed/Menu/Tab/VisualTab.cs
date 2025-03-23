using REPOssessed.Cheats;
using REPOssessed.Cheats.Core;
using REPOssessed.Menu.Core;
using REPOssessed.Util;
using UnityEngine;

namespace REPOssessed.Menu.Tab
{
    internal class VisualTab : MenuTab
    {
        public VisualTab() : base("VisualTab.Title") { }
        private Vector2 scrollPos = Vector2.zero;
        private Vector2 scrollPos2 = Vector2.zero;

        public override void Draw()
        {
            VisualContent();
            ESPContent();
        }

        private void VisualContent()
        {
            UI.VerticalSpace(ref scrollPos, () =>
            {
                UI.Header("VisualTab.Title");
                UI.CheatToggleSlider(Cheat.Instance<FOV>(), "VisualTab.FOV", FOV.Value.ToString(), ref FOV.Value, 1, 140);

            }, GUILayout.Width(HackMenu.Instance.contentWidth * 0.5f - HackMenu.Instance.spaceFromLeft));
        }

        private void ESPContent()
        {
            UI.VerticalSpace(ref scrollPos2, () =>
            {
                UI.Header("VisualTab.ESP");
                UI.CheatToggleSlider(Cheat.Instance<ESP>(), "VisualTab.ToggleESP", ESP.Value.ToString("F1"), ref ESP.Value, 0f, 5000f);
                UI.Button("VisualTab.ToggleAllESP", ESP.ToggleAll);
                UI.Checkbox("VisualTab.PlayerESP", ref Settings.b_PlayerESP);
                UI.Checkbox("VisualTab.EnemyESP", ref Settings.b_EnemyESP);
                UI.Checkbox("VisualTab.ItemESP", ref Settings.b_ItemESP);
                UI.Checkbox("VisualTab.CartESP", ref Settings.b_CartESP);
                UI.Checkbox("VisualTab.ExtractionESP", ref Settings.b_ExtractionESP);
                UI.Checkbox("VisualTab.DeathHeadESP", ref Settings.b_DeathHeadESP);
                UI.Checkbox("VisualTab.TruckESP", ref Settings.b_TruckESP);
            }, GUILayout.Width(HackMenu.Instance.contentWidth * 0.5f - HackMenu.Instance.spaceFromLeft));
        }
    }
}