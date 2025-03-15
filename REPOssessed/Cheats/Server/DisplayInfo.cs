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
            Settings.b_DisplayPlayers = !Settings.b_DisplayPlayers;
            Settings.b_DisplayEnemies = !Settings.b_DisplayEnemies;
        }


        private static string info = "";
        private void Format(string label, params object[] args) => info += $"{string.Format(label.Localize(), args)}\n";

        public override void OnGui()
        {
            info = "";

            if (Settings.b_DisplayMapObjects) Format("Cheats.Info.DisplayMapObjects", GetMapObjectsCount(), GetMapObjectsValueCount());
            if (Settings.b_DisplayDeathHeads) Format("Cheats.Info.DisplayDeathHeads", GetDeathHeadsCount());
            if (Settings.b_DisplayPlayers) Format("Cheats.Info.DisplayPlayers", GetPlayersCount());
            if (Settings.b_DisplayEnemies) Format("Cheats.Info.DisplayEnemies", GetEnemiesCount());

            VisualUtil.DrawString(new Vector2(Screen.width - 200, 10), info, Settings.c_primary, false, false, false, Settings.i_menuFontSize);
        }

        private int GetEnemiesCount()
        {
            return GameObjectManager.enemies.Where(p => p != null && p.Handle() != null && !p.Handle().IsDead() && !p.Handle().IsDisabled()).Count();
        }

        private int GetPlayersCount()
        {
            return GameObjectManager.players.Where(p => p != null && p.Handle() != null).Count();
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
