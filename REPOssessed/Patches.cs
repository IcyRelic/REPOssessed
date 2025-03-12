using ExitGames.Client.Photon;
using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using REPOssessed.Handler;
using REPOssessed.Manager;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace REPOssessed
{
    [HarmonyPatch]
    internal class Patches
    {
        internal static readonly object keyByteZero = (object)(byte)0;
        internal static readonly object keyByteOne = (object)(byte)1;
        internal static readonly object keyByteTwo = (object)(byte)2;
        internal static readonly object keyByteThree = (object)(byte)3;
        internal static readonly object keyByteFour = (object)(byte)4;
        internal static readonly object keyByteFive = (object)(byte)5;
        internal static readonly object keyByteSix = (object)(byte)6;
        internal static readonly object keyByteSeven = (object)(byte)7;
        internal static readonly object keyByteEight = (object)(byte)8;

        public static List<string> IgnoredRPCDebugs = new List<string>
        {
            "IsTalkingRPC",
            "ReceiveSyncData",
            "SetColorRPC",
        };

        [HarmonyPatch(typeof(PhotonNetwork), "ExecuteRpc"), HarmonyPrefix]
        public static bool ExecuteRPC(Hashtable rpcData, Player sender)
        {
            if (sender is null || sender?.GamePlayer() == null) return true;

            string rpc = rpcData.ContainsKey(keyByteFive) ?
                PhotonNetwork.PhotonServerSettings.RpcList[Convert.ToByte(rpcData[keyByteFive])]
                : rpcData[keyByteThree] as string;

            if (!IgnoredRPCDebugs.Contains(rpc)) Debug.LogWarning($"Processing RPC '{rpc}' From '{sender.NickName}'");

            if (!sender.IsLocal && sender.GamePlayer().Handle().IsRPCBlocked())
            {
                Debug.LogError($"RPC {rpc} was blocked from {sender.NickName}.");
                return false;
            }

            return sender.GamePlayer().Handle().OnReceivedRPC(rpc, rpcData);
        }

        [HarmonyPatch(typeof(LoadBalancingPeer), "OpRaiseEvent"), HarmonyPrefix]
        public static bool OpRaiseEvent(LoadBalancingPeer __instance, byte eventCode, object customEventContent, RaiseEventOptions raiseEventOptions, SendOptions sendOptions)
        {
            if (eventCode == 204) return false;
            return true;
        }

        [HarmonyPatch(typeof(MenuPageEsc), "ButtonEventQuitToMenu"), HarmonyPrefix]
        public static void ButtonEventQuitToMenu() => GameObjectManager.ClearObjects();

        [HarmonyPatch(typeof(SteamManager), "OnLobbyMemberJoined"), HarmonyPrefix]
        public static void OnLobbyMemberJoined() => REPOssessed.Instance.AlertUsingREPOssessed();
    }
}
