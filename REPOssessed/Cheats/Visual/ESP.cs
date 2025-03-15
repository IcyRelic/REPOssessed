using REPOssessed.Cheats.Core;
using REPOssessed.Extensions;
using REPOssessed.Handler;
using REPOssessed.Manager;
using REPOssessed.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace REPOssessed.Cheats
{
    internal class ESP : ToggleCheat, IVariableCheat<float>
    {
        public static float Value = 5000f;

        public override void OnGui()
        {
            try
            {
                if (!Cheat.Instance<ESP>().Enabled) return;
                if (Settings.b_PlayerESP) DisplayPlayers();
                if (Settings.b_ItemESP) DisplayItems();
                if (Settings.b_EnemyESP) DisplayEnemies();
                if (Settings.b_CartESP) DisplayCarts();
                if (Settings.b_ExtractionESP) DisplayExtractions();
                if (Settings.b_DeathHeadESP) DisplayDeathHeads();
                if (Settings.b_TruckESP) DisplayTruck();
            }
            catch (Exception e)
            {
                Settings.s_DebugMessage = "Msg: " + e.Message + "\nSrc: " + e.Source + "\n" + e.StackTrace;
            }
        }

        public static void ToggleAll()
        {
            Settings.b_PlayerESP = !Settings.b_PlayerESP;
            Settings.b_EnemyESP = !Settings.b_EnemyESP;
            Settings.b_ItemESP = !Settings.b_ItemESP;
            Settings.b_CartESP = !Settings.b_CartESP;
            Settings.b_ExtractionESP = !Settings.b_ExtractionESP;
            Settings.b_DeathHeadESP = !Settings.b_DeathHeadESP;
            Settings.b_TruckESP = !Settings.b_TruckESP;
        }

        private void DisplayObjects<T>(IEnumerable<T> objects, Func<T, string> labelSelector, Func<T, RGBAColor> colorSelector) where T : Component
        {
            if (objects == null) return;
            foreach (var obj in objects?.Where(o => o != null && o.gameObject.activeSelf))
            {
                if (obj.transform == null) continue;
                float distance = GetDistanceToPos(obj.transform.position);
                if (distance == 0f || distance > Value || !WorldToScreen(obj.transform.position, out var screen)) continue;
                VisualUtil.DrawDistanceString(screen, labelSelector(obj), colorSelector(obj), distance);
            }
        }

        private void DisplayPlayers()
        {
            DisplayObjects(
                GameObjectManager.players?.Where(p => p != null && p.Handle() != null && !p.Handle().IsLocalPlayer() && !p.Handle().IsDead()),
                player => $"{(player.Handle().IsTalking() ? "[VC]" : "")} {player.Handle().GetName()} ( {player.Handle().GetHealth()}/{player.Handle().GetMaxHealth()} )",
                player => Settings.c_espPlayers
            );
        }

        private void DisplayItems()
        {
            DisplayObjects(
                GameObjectManager.items?.Where(i => i != null && !i.Handle().IsCart() && i.Handle().IsShopItem() || i.Handle().IsValuable()),
                item => $"{item.Handle().GetName()} {(item.Handle().IsValuable() ? $"( {item.Handle().GetValue()} )" : "")} {(item.Handle().IsTrap() ? "( Trap )" : "")}",
                item => Settings.c_espItems
            );
        }

        private void DisplayEnemies()
        {
            DisplayObjects(
                GameObjectManager.enemies?.Where(e => e != null && !e.Handle().IsDead() && !e.Handle().IsDisabled()),
                enemy => $"{enemy.Handle().GetName()} ( {enemy.Handle().GetHealth()}/{enemy.Handle().GetMaxHealth()} )",
                enemy => Settings.c_espEnemies
            );
        }

        private void DisplayDeathHeads()
        {
            DisplayObjects(
                GameObjectManager.deathHeads?.Where(d => d != null && d.playerAvatar != null && d.playerAvatar.Handle().IsDead()),
                deathHead => $"{deathHead.playerAvatar.Handle().GetName()}'s Death Head",
                deathHead => Settings.c_espDeathHeads
            );
        }

        private void DisplayExtractions()
        {
            DisplayObjects(
                GameObjectManager.extractions?.Where(e => e != null && !e.Reflect().GetValue<bool>("isShop") && !e.StateIs(ExtractionPoint.State.Complete)), 
                extraction => "Extraction",
                extraction => Settings.c_espExtractions
            );           
        }

        private void DisplayCarts()
        {
            DisplayObjects(
                GameObjectManager.carts?.Where(c => c != null),
                cart => $"Cart",
                cart => Settings.c_espCart
            );
        }

        private void DisplayTruck()
        {
            DisplayObjects(
                new[] { GameObjectManager.truck },
                cart => $"Truck",
                cart => Settings.c_espTruck
            );
        }
    }
}
