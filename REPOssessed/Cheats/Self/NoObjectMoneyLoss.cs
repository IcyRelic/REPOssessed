using HarmonyLib;
using REPOssessed.Cheats.Core;
using REPOssessed.Handler;
using REPOssessed.Util;
using UnityEngine;

namespace REPOssessed.Cheats
{
    [HarmonyPatch]
    internal class NoObjectMoneyLoss : ToggleCheat
    {
        [HarmonyPatch(typeof(PhysGrabObjectImpactDetector), "Break"), HarmonyPrefix]
        public static bool Break(PhysGrabObjectImpactDetector __instance, float valueLost, Vector3 _contactPoint, int breakLevel)
        {
            if (Cheat.Instance<NoObjectMoneyLoss>().Enabled && __instance.isValuable && !__instance.isEnemy && SemiFunc.IsMasterClientOrSingleplayer())
            {
                PhysGrabObject physGrabObject = __instance.Reflect().GetValue<PhysGrabObject>("physGrabObject");
                if (physGrabObject?.Handle() == null) return true;
                PlayerAvatar player = physGrabObject.Handle().GetLastPlayerHeld();
                if (player == null || player.Handle() == null) return true;
                if (PlayerAvatar.instance.GetLocalPlayer().Handle().physGrabObject == physGrabObject || player.Handle().IsLocalPlayer()) return false;
            }
            return true;
        }
    }
}
