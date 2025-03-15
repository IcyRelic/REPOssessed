using HarmonyLib;
using Photon.Pun;
using REPOssessed.Cheats.Core;
using REPOssessed.Handler;
using Steamworks;

namespace REPOssessed.Cheats
{
    [HarmonyPatch]
    internal class NameSpoofer : ToggleCheat, IVariableCheat<string>
    {
        private string OriginalValue = SteamClient.Name;
        public static string Value = "";

        public override void Update()
        {
            if (!Enabled) return;
            SetName(Value);
        }

        public override void OnDisable() => SetName(OriginalValue);

        private static void SetName(string name)
        {
            PlayerAvatar player = PlayerAvatar.instance.GetLocalPlayer();
            if (player == null || player.Handle() == null || player.Handle().GetName() == name) return;
            if (!SemiFunc.IsMultiplayer()) PlayerAvatar.instance.AddToStatsManagerRPC(name, player.Handle().GetSteamID());
            else
            {
                PhotonNetwork.LocalPlayer.NickName = name;
                player.photonView.RPC("AddToStatsManagerRPC", RpcTarget.All, name, player.Handle().GetSteamID());
            }
        }
    }
}
