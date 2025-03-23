using HarmonyLib;
using REPOssessed.Cheats.Core;
using REPOssessed.Handler;
using REPOssessed.Manager;
using REPOssessed.Util;
using System.Linq;

namespace REPOssessed.Cheats
{
    [HarmonyPatch]
    internal class NonEnemyTargetable : ToggleCheat
    {
        public override void OnEnable()
        {
            PlayerAvatar player = PlayerAvatar.instance.GetLocalPlayer();
            if (player == null) return;
            GameObjectManager.enemies?.Where(e => e != null && e.Handle() != null && !e.Handle().IsDead() && !e.Handle().IsDisabled()).ToList().ForEach(e =>
            {
                if (e.Reflect().GetValue<PlayerAvatar>("TargetPlayerAvatar") == player) e.Reflect().SetValue("TargetPlayerAvatar", null);
                if (e.Reflect().GetValue<int>("TargetPlayerViewID") == player.photonView.ViewID) e.Reflect().SetValue("TargetPlayerViewID", null);
                EnemyVision enemyVision = e.Reflect().GetValue<EnemyVision>("Vision");
                if (enemyVision != null)
                {
                    if (enemyVision.Reflect().GetValue<PlayerAvatar>("onVisionTriggeredPlayer") == player) enemyVision.Reflect().SetValue("onVisionTriggeredPlayer", null);
                    if (enemyVision.Reflect().GetValue<int>("onVisionTriggeredID") == player.photonView.ViewID) enemyVision.Reflect().SetValue("onVisionTriggeredID", null);
                }
            });
        }

        [HarmonyPatch(typeof(Enemy), "SetChaseTarget"), HarmonyPrefix]
        public static bool SetChaseTarget(PlayerAvatar playerAvatar)
        {
            PlayerAvatar player = PlayerAvatar.instance.GetLocalPlayer();
            if (Cheat.Instance<NonEnemyTargetable>().Enabled && player != null && playerAvatar == player) return false;
            return true;
        }

        [HarmonyPatch(typeof(EnemyVision), "VisionTrigger"), HarmonyPrefix]
        public static bool VisionTrigger(int playerID, PlayerAvatar player, bool culled, bool playerNear)
        {
            PlayerAvatar _player = PlayerAvatar.instance.GetLocalPlayer();
            if (Cheat.Instance<NonEnemyTargetable>().Enabled && _player != null && _player == player) return false;
            return true;
        }
    }
}
