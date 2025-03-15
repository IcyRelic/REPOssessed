using REPOssessed.Cheats;
using REPOssessed.Cheats.Core;
using REPOssessed.Handler;
using REPOssessed.Menu.Core;
using REPOssessed.Util;
using System.Linq;
using UnityEngine;

namespace REPOssessed.Menu.Tab
{
    internal class ServerTab : MenuTab
    {
        public ServerTab() : base("ServerTab.Title") { }
        private Vector2 scrollPos = Vector2.zero;
        private Vector2 scrollPos2 = Vector2.zero;
        private Vector2 scrollPos3 = Vector2.zero;

        public override void Draw()
        {
            ServerMenuContent();
            GUILayout.BeginVertical();
            ManagersContent();
            InfoMenuContent();
            GUILayout.EndVertical();
        }

        private void ServerMenuContent()
        {
            UI.VerticalSpace(ref scrollPos, () =>
            {
                UI.Header("ServerTab.ServerCheats");
                
            }, GUILayout.Width(HackMenu.Instance.contentWidth * 0.5f - HackMenu.Instance.spaceFromLeft));
        }

        private void InfoMenuContent()
        {
            UI.VerticalSpace(ref scrollPos2, () =>
            {
                UI.Header("ServerTab.InfoDisplay");
                UI.Checkbox("ServerTab.ToggleInfoDisplay", Cheat.Instance<InfoDisplay>());
                UI.Button("ServerTab.ToggleAllInfoDisplays", InfoDisplay.ToggleAll);
                UI.Checkbox("ServerTab.DisplayMapObjects", ref Settings.b_DisplayMapObjects);
                UI.Checkbox("ServerTab.DisplayDeathHeads", ref Settings.b_DisplayDeathHeads);
                UI.Checkbox("ServerTab.DisplayPlayers", ref Settings.b_DisplayPlayers);
                UI.Checkbox("ServerTab.DisplayEnemies", ref Settings.b_DisplayEnemies);
            }, GUILayout.Width(HackMenu.Instance.contentWidth * 0.5f - HackMenu.Instance.spaceFromLeft));
        }

        private void ManagersContent()
        {
            UI.VerticalSpace(ref scrollPos3, () =>
            {
                UI.Header("ServerTab.Managers");
                UI.Toggle("LootManager.Title", ref HackMenu.Instance.LootManagerWindow.isOpen, "General.Open", "General.Close");
                UI.Toggle("ItemManager.Title", ref HackMenu.Instance.ItemManagerWindow.isOpen, "General.Open", "General.Close");
            }, GUILayout.Width(HackMenu.Instance.contentWidth * 0.5f - HackMenu.Instance.spaceFromLeft));
        }
    }
}
