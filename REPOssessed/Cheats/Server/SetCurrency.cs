using Photon.Pun;
using REPOssessed.Cheats.Core;
using REPOssessed.Util;

namespace REPOssessed.Cheats
{
    internal class SetCurrency : ExecutableCheat
    {
        public static int Currency = 3;

        public override void Execute()
        {
            if (SemiFunc.IsMultiplayer())
            {
                StatsManager.instance.runStats["currency"] = Currency;
                PunManager.instance.Reflect().GetValue<PhotonView>("photonView").RPC("SetRunStatRPC", RpcTarget.Others, "currency", Currency);
            }
            else StatsManager.instance.runStats["currency"] = Currency;
        }

        public void _SetCurrency(int currency)
        {
            Currency = currency / 1000;
            Execute();
        }
    }
}
