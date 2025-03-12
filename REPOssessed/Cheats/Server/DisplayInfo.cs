using REPOssessed.Cheats.Core;
using REPOssessed.Handler;
using REPOssessed.Language;
using REPOssessed.Manager;
using REPOssessed.Util;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace REPOssessed.Cheats
{
    internal class InfoDisplay : ToggleCheat
    {
        public InfoDisplay() : base() { }

        public static void ToggleAll()
        {
            Settings.b_DisplayMapObjects = !Settings.b_DisplayMapObjects;
            Settings.b_DisplayDeathHeads = !Settings.b_DisplayDeathHeads;
            Settings.b_DisplaySpectators = !Settings.b_DisplaySpectators;
        }


        private static string info = "";
        private void Format(string label, params object[] args) => info += $"{string.Format(label.Localize(), args)}\n";

        public override void OnGui()
        {
            info = "";

            if (Settings.b_DisplayMapObjects) Format("Cheats.Info.DisplayMapObjects", GetMapObjectsCount(), GetMapObjectsValueCount());
            if (Settings.b_DisplayDeathHeads) Format("Cheats.Info.DisplayDeathHeads", GetDeathHeadsCount());
            if (Settings.b_DisplaySpectators)
            {
                Format("Cheats.Info.DisplaySpectators", GetSpectatorCount());
                Format("Cheats.Info.Spectators");
                GetSpectators().ToList().ForEach(p => Format(p.Handle().GetName()));
                Format("Cheats.Info.CurrentSpectators");
                GetLocalPlayerSpectators().ToList().ForEach(p => Format(p.Handle().GetName()));
            }

            VisualUtil.DrawString(new Vector2(Screen.width - 200, 10), info, Settings.c_primary, false, false, false, Settings.i_menuFontSize);
        }

        private List<PlayerAvatar> GetLocalPlayerSpectators()
        {
            return GameObjectManager.players.Where(p => p != null && p.Handle() != null && p.Handle().IsSpectating() && p.Handle().GetPlayerSpectating() == p.GetLocalPlayer()).ToList();
        }

        private List<PlayerAvatar> GetSpectators()
        {
            return GameObjectManager.players.Where(p => p != null && p.Handle() != null && p.Handle().IsSpectating()).ToList();
        }

        private int GetSpectatorCount()
        {
            return GameObjectManager.players.Where(p => p != null && p.Handle() != null && p.Handle().IsSpectating()).Count();
        }

        private float GetMapObjectsValueCount()
        {
            return GameObjectManager.items.Where(i => i != null && i.Handle().IsValuable()).Sum(i => i.Handle().GetValue());
        }

        private int GetMapObjectsCount()
        {
            return GameObjectManager.items.Where(i => i != null && i.Handle().IsValuable()).Count(); 
        }

        private int GetDeathHeadsCount()
        {
            return GameObjectManager.deathHeads.Where(d => d != null && d.playerAvatar != null && d.playerAvatar.Handle() != null && d.playerAvatar.Handle().IsDead()).Count();
        }
    }
}
