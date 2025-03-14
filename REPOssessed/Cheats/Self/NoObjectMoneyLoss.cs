using HarmonyLib;
using REPOssessed.Cheats.Core;
using REPOssessed.Util;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace REPOssessed.Cheats
{
    [HarmonyPatch]
    internal class NoObjectMoneyLoss : ToggleCheat
    {
        public static Dictionary<PhysGrabObject, Vector3> droppedPhysGrabObjects = new Dictionary<PhysGrabObject, Vector3>();

        [HarmonyPatch(typeof(PhysGrabObjectImpactDetector), "Break"), HarmonyPrefix]
        public static bool Break(PhysGrabObjectImpactDetector __instance, float valueLost, Vector3 _contactPoint, int breakLevel)
        {
            if (Cheat.Instance<NoObjectMoneyLoss>().Enabled && !__instance.isEnemy)
            {
                if (SemiFunc.IsMultiplayer()) droppedPhysGrabObjects.Add(__instance.Reflect().GetValue<PhysGrabObject>("physGrabObject"), _contactPoint);
                else return false;
            }
            return true;
        }

        public static PhysGrabObject GetPhysGrabObject(Vector3 position)
        {
            return droppedPhysGrabObjects.FirstOrDefault(x => x.Value == position).Key;
        }
    }
}
