using HarmonyLib;
using REPOssessed.Cheats.Core;
using REPOssessed.Handler;
using REPOssessed.Manager;
using REPOssessed.Util;
using System.Linq;

namespace REPOssessed.Cheats
{
    [HarmonyPatch]
    internal class ForceThiefPunishment : ExecutableCheat
    {
        private static bool DisallowShopGetTotalCost = false;

        [HarmonyPatch(typeof(SemiFunc), "ShopGetTotalCost"), HarmonyPrefix]
        public static bool ShopGetTotalCost(ref int __result)
        {
            if (DisallowShopGetTotalCost)
            {
                DisallowShopGetTotalCost = false;
                __result = 1;
                return false;
            }
            return true;
        }

        public override void Execute()
        {
            PlayerAvatar player = PlayerAvatar.instance.GetLocalPlayer();
            if (player == null || player.Handle() == null || !player.Handle().IsMasterClient()) return;
            GameObjectManager.extractions.Where(e => e != null && e.Reflect().GetValue<bool>("isShop")).ToList().ForEach(e =>
            {
                DisallowShopGetTotalCost = true;
                e.Reflect().Invoke("ThiefPunishment");
            });
        }
    }
}
