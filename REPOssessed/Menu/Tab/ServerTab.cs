using REPOssessed.Cheats;
using REPOssessed.Cheats.Core;
using REPOssessed.Extensions;
using REPOssessed.Handler;
using REPOssessed.Language;
using REPOssessed.Manager;
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
        private Vector2 scrollPos4 = Vector2.zero;
        private int currency = 3000;

        public override void Draw()
        {
            GUILayout.BeginVertical();
            ServerMenuContent();
            ExtractionContent();
            GUILayout.EndVertical();
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
                UI.TextboxAction("ServerTab.SetCurrency", ref currency, 5, new UIButton("General.Set", () =>
                {
                    SetCurrency.Currency = currency / 1000;
                    Cheat.Instance<SetCurrency>().Execute();
                }));
                UI.Button("ServerTab.BreakAllObjects", () => GameObjectManager.items.Where(i => i != null && i.Handle() != null).ToList().ForEach(i => i.Handle().Break()));
                UI.Button(["ServerTab.ForceThiefPunishment", "General.HostTag"], () => Cheat.Instance<ForceThiefPunishment>().Execute());

            }, GUILayout.Width(HackMenu.Instance.contentWidth * 0.5f - HackMenu.Instance.spaceFromLeft));
        }


        private void ExtractionContent()
        {
            UI.VerticalSpace(ref scrollPos2, () =>
            {
                UI.Header("ServerTab.Extraction");
                UI.Button("ServerTab.CompleteAllExtractions", () => GameObjectManager.extractions.Where(e => e != null && !e.StateIs(ExtractionPoint.State.Complete)).ToList().ForEach(e => e.CompleteExtraction()));
                ExtractionsCompleteContent();

            }, GUILayout.Width(HackMenu.Instance.contentWidth * 0.5f - HackMenu.Instance.spaceFromLeft));
        }

        private void InfoMenuContent()
        {
            UI.VerticalSpace(ref scrollPos3, () =>
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
            UI.VerticalSpace(ref scrollPos4, () =>
            {
                UI.Header("ServerTab.Managers");
                UI.Toggle("LootManager.Title", ref HackMenu.Instance.LootManagerWindow.isOpen, "General.Open", "General.Close");
                UI.Toggle("ItemManager.Title", ref HackMenu.Instance.ItemManagerWindow.isOpen, "General.Open", "General.Close");
                UI.Toggle("LevelManager.Title", ref HackMenu.Instance.LevelManagerWindow.isOpen, "General.Open", "General.Close");
            }, GUILayout.Width(HackMenu.Instance.contentWidth * 0.5f - HackMenu.Instance.spaceFromLeft));
        }

        private void ExtractionsCompleteContent()
        {
            int Index = 1;
            GameObjectManager.extractions.Where(e => e != null && e.transform != null && !e.Reflect().GetValue<bool>("isShop") && !e.StateIs(ExtractionPoint.State.Complete)).ToList().ForEach(e =>
            {
                UI.Button($"{"ServerTab.Extraction".Localize()} {Index++}", () => e.CompleteExtraction(), "General.Complete");
            });
        }
    }
}
