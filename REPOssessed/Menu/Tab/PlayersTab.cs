using REPOssessed.Handler;
using REPOssessed.Language;
using REPOssessed.Manager;
using REPOssessed.Menu.Core;
using REPOssessed.Util;
using System.Linq;
using UnityEngine;

namespace REPOssessed.Menu.Tab
{
    internal class PlayersTab : MenuTab
    {
        public PlayersTab() : base("PlayersTab.Title") { }

        private Vector2 scrollPos = Vector2.zero;
        private Vector2 scrollPos2 = Vector2.zero;
        public static PlayerAvatar selectedPlayer = null;
        private string message = "REPOssessed on top!";
        private int color = -1;
        private int heal = 1;
        private int damage = 1;

        public override void Draw()
        {
            UI.VerticalSpace(ref scrollPos, () =>
            {
                PlayersList();
            }, GUILayout.Width(HackMenu.Instance.contentWidth * 0.3f - HackMenu.Instance.spaceFromLeft));
            UI.VerticalSpace(ref scrollPos2, () =>
            {
                GeneralActions();
                PlayerActions();
            }, GUILayout.Width(HackMenu.Instance.contentWidth * 0.7f - HackMenu.Instance.spaceFromLeft));
        }

        private void GeneralActions()
        {
            UI.Header("PlayersTab.GeneralActions");

            UI.Button("PlayersTab.ReviveAll", () => GameObjectManager.players.Where(p => p != null && p.Handle().IsDead()).ToList().ForEach(p => p.Handle().RevivePlayer()));
            UI.Button("PlayersTab.ReviveOthers", () => GameObjectManager.players.Where(p => p != null && p.Handle().IsDead() && !p.Handle().IsLocalPlayer()).ToList().ForEach(p => p.Handle().RevivePlayer()));
            UI.Button("PlayersTab.KillAll", () => GameObjectManager.players.Where(p => p != null).ToList().ForEach(p => p.PlayerDeath(-1)));
            UI.Button("PlayersTab.KillOthers", () => GameObjectManager.players.Where(p => p != null && !p.Handle().IsLocalPlayer()).ToList().ForEach(p => p.PlayerDeath(-1)));
            UI.TextboxAction("PlayersTab.TalkAll", ref message, 100, new UIButton("PlayersTab.Send", () => GameObjectManager.players.Where(p => p != null).ToList().ForEach(p => p.ChatMessageSend(message, false))));
            UI.TextboxAction("PlayersTab.TalkOthers", ref message, 100, new UIButton("PlayersTab.Send", () => GameObjectManager.players.Where(p => p != null && !p.Handle().IsLocalPlayer()).ToList().ForEach(p => p.ChatMessageSend(message, false))));
            UI.TextboxAction("PlayersTab.ChangeAllColors", ref color, 3, new UIButton("General.Set", () => GameObjectManager.players.Where(p => p != null).ToList().ForEach(p => p.PlayerAvatarSetColor(color))));
            UI.TextboxAction("PlayersTab.ChangeOthersColors", ref color, 3, new UIButton("General.Set", () => GameObjectManager.players.Where(p => p != null && !p.Handle().IsLocalPlayer()).ToList().ForEach(p => p.PlayerAvatarSetColor(color))));
            UI.Label("PlayersTab.Colors");
            UI.Label(string.Join(", ", Enumerable.Range(0, AssetManager.instance.playerColors.Count).Select(i => i)));
        }

        private void PlayerActions()
        {
            if (selectedPlayer == null || selectedPlayer.Handle() == null) return;

            string PhysGrabObject = selectedPlayer.Handle().physGrabObject == null ? "General.None".Localize() : selectedPlayer.Handle().physGrabObject.Handle().GetName();

            UI.Header("PlayersTab.PlayerActions");

            UI.Label("PlayersTab.SteamId", selectedPlayer.Handle().GetSteamID().ToString());
            UI.Label("PlayersTab.Status", selectedPlayer.Handle().IsDead() ? "Dead" : "Alive");
            UI.Label("PlayersTab.Health", selectedPlayer.Handle().GetHealth().ToString());
            UI.Label("PlayersTab.HoldingItem", PhysGrabObject);
            UI.Label("PlayersTab.IsMasterClient", selectedPlayer.Handle().IsMasterClient().ToString());
            UI.Label("PlayersTab.REPOssessedUser", selectedPlayer.Handle().IsREPOssessedUser().ToString());
            UI.Label("PlayersTab.Crowned", selectedPlayer.Handle().IsCrowned().ToString());
            UI.Button("PlayersTab.Heal", () => selectedPlayer.playerHealth.Heal(selectedPlayer.Handle().GetMaxHealth(), false));
            UI.Button("PlayersTab.Crown", () => PunManager.instance.CrownPlayerSync(selectedPlayer.Handle().GetSteamID()));
            UI.Button("PlayersTab.Kill", () => selectedPlayer.PlayerDeath(-1));
            UI.Button("PlayersTab.Revive", () => selectedPlayer.Handle().RevivePlayer());
            UI.Button("PlayersTab.ForceTumble", () => selectedPlayer.Handle().ForceTumble());
            UI.TextboxAction("PlayersTab.Heal", ref heal, 3, new UIButton("General.Set", () => selectedPlayer.Handle().Heal(heal)));
            UI.TextboxAction("PlayersTab.Damage", ref damage, 3, new UIButton("General.Set", () => selectedPlayer.Handle().Hurt(damage)));
            UI.Button("PlayersTab.BreakHeldItem", () =>
            {
                if (selectedPlayer.Handle().physGrabObject != null) selectedPlayer.Handle().physGrabObject.Handle().Break();
            });
            UI.TextboxAction("PlayersTab.ChatMessage", ref message, 100, new UIButton("PlayersTab.Send", () => selectedPlayer.ChatMessageSend(message, false)));
            UI.TextboxAction("PlayersTab.ChangeColor", ref color, 2, new UIButton("General.Set", () => selectedPlayer.PlayerAvatarSetColor(color)));
            if (!selectedPlayer.Handle().IsLocalPlayer())
            {
                UI.Button("PlayersTab.TeleportToPlayer", () =>
                {
                    if (selectedPlayer != null && selectedPlayer.transform != null) PlayerAvatar.instance.GetLocalPlayer().Handle().Teleport(selectedPlayer.transform.position, selectedPlayer.transform.rotation);
                }, "SelfTab.Teleport");
                UI.Button("PlayersTab.TeleportPlayerToYou", () =>
                {
                    if (selectedPlayer != null && selectedPlayer.transform != null) selectedPlayer.Handle().Teleport(SemiFunc.MainCamera().transform.position, SemiFunc.MainCamera().transform.rotation);
                }, "SelfTab.Teleport");
                UI.Button("PlayersTab.BlockRPCs", () => selectedPlayer.Handle().ToggleRPCBlock(), selectedPlayer.Handle().IsRPCBlocked() ? "PlayersTab.Unblock" : "PlayersTab.Block");
            }
        }

        private void PlayersList()
        {
            float width = HackMenu.Instance.contentWidth * 0.3f - HackMenu.Instance.spaceFromLeft * 2;
            float height = HackMenu.Instance.contentHeight - 20;
            Rect rect = new Rect(0, 0, width, height);
            GUI.Box(rect, "PlayersTab.PlayerList".Localize());
            GUILayout.BeginVertical(GUILayout.Width(width), GUILayout.Height(height));
            GUILayout.Space(25);
            foreach (PlayerAvatar player in GameObjectManager.players.Where(p => p != null))
            {
                if (selectedPlayer == null) selectedPlayer = player;
                if (selectedPlayer.GetInstanceID() == player.GetInstanceID()) GUI.contentColor = Settings.c_success.GetColor();
                string name = selectedPlayer.Handle().IsREPOssessedUser() ? $"[REPOssessed] {player.Handle().GetName()}" : player.Handle().GetName();
                if (GUILayout.Button(name, GUI.skin.label)) selectedPlayer = player;
                GUI.contentColor = Settings.c_menuText.GetColor();
            }
            GUILayout.EndVertical();
        }
    }
}
