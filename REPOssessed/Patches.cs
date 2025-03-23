using ExitGames.Client.Photon;
using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using REPOssessed.Cheats;
using REPOssessed.Cheats.Core;
using REPOssessed.Handler;
using REPOssessed.Manager;
using REPOssessed.Util;
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

        [HarmonyPatch(typeof(PlayerAvatar), "OnPhotonSerializeView"), HarmonyPrefix]
        public static bool OnPhotonSerializeView(PlayerAvatar __instance, PhotonStream stream, PhotonMessageInfo info)
        {
            try
            {
                if (__instance == null || __instance.Handle() == null || !__instance.Handle().IsLocalPlayer()) return true;
                if (stream.IsWriting)
                {
                    stream.SendNext(__instance.Reflect().GetValue<bool>("isCrouching"));
                    stream.SendNext(__instance.Reflect().GetValue<bool>("isSprinting"));
                    stream.SendNext(__instance.Reflect().GetValue<bool>("isCrawling"));
                    stream.SendNext(__instance.Reflect().GetValue<bool>("isSliding"));
                    stream.SendNext(__instance.Reflect().GetValue<bool>("isMoving"));
                    stream.SendNext(__instance.Reflect().GetValue<bool>("isGrounded"));
                    stream.SendNext(__instance.Reflect().GetValue<bool>("Interact"));
                    stream.SendNext(__instance.Reflect().GetValue<Vector3>("InputDirection"));
                    stream.SendNext(PlayerController.instance.VelocityRelative);
                    stream.SendNext(__instance.Reflect().GetValue<Vector3>("rbVelocityRaw"));
                    stream.SendNext(Cheat.Instance<Invisibility>().Enabled ? new Vector3(9999f, 9999f, 9999f) : PlayerController.instance.transform.position);
                    stream.SendNext(PlayerController.instance.transform.rotation);
                    stream.SendNext(__instance.Reflect().GetValue<Vector3>("localCameraPosition"));
                    stream.SendNext(__instance.Reflect().GetValue<Quaternion>("localCameraRotation"));
                    stream.SendNext(PlayerController.instance.CollisionGrounded.physRiding);
                    stream.SendNext(PlayerController.instance.CollisionGrounded.physRidingID);
                    stream.SendNext(PlayerController.instance.CollisionGrounded.physRidingPosition);
                    stream.SendNext(__instance.flashlightLightAim.clientAimPoint);
                    stream.SendNext(__instance.Reflect().GetValue<int>("playerPing"));
                    return false;
                }
                __instance.Reflect().SetValue("isCrouching", (bool)stream.ReceiveNext());
                __instance.Reflect().SetValue("isSprinting", (bool)stream.ReceiveNext());
                __instance.Reflect().SetValue("isCrawling", (bool)stream.ReceiveNext());
                __instance.Reflect().SetValue("isSliding", (bool)stream.ReceiveNext());
                __instance.Reflect().SetValue("isMoving", (bool)stream.ReceiveNext());
                __instance.Reflect().SetValue("isGrounded", (bool)stream.ReceiveNext());
                __instance.Reflect().SetValue("Interact", (bool)stream.ReceiveNext());
                __instance.Reflect().SetValue("InputDirection", (Vector3)stream.ReceiveNext());
                __instance.Reflect().SetValue("rbVelocity", (Vector3)stream.ReceiveNext());
                __instance.Reflect().SetValue("rbVelocityRaw", (Vector3)stream.ReceiveNext());
                __instance.Reflect().SetValue("clientPosition", (Vector3)stream.ReceiveNext());
                __instance.Reflect().SetValue("clientRotation", (Quaternion)stream.ReceiveNext());
                __instance.Reflect().SetValue("clientPositionDelta", Vector3.Distance(__instance.Reflect().GetValue<Vector3>("clientPositionCurrent"), __instance.Reflect().GetValue<Vector3>("clientPosition")));
                __instance.Reflect().SetValue("clientRotationDelta", Quaternion.Angle(__instance.Reflect().GetValue<Quaternion>("clientRotationCurrent"), __instance.Reflect().GetValue<Quaternion>("clientRotation")));
                __instance.Reflect().SetValue("localCameraPosition", (Vector3)stream.ReceiveNext());
                __instance.Reflect().SetValue("localCameraRotation", (Quaternion)stream.ReceiveNext());
                __instance.Reflect().SetValue("clientPhysRiding", (bool)stream.ReceiveNext());
                __instance.Reflect().SetValue("clientPhysRidingID", (int)stream.ReceiveNext());
                __instance.Reflect().SetValue("clientPhysRidingPosition", (Vector3)stream.ReceiveNext());
                if (__instance.Reflect().GetValue<bool>("clientPhysRiding"))
                {
                    PhotonView photonView = PhotonView.Find(__instance.Reflect().GetValue<int>("clientPhysRidingID"));
                    if ((bool)photonView) __instance.Reflect().SetValue("clientPhysRidingTransform", photonView.transform);
                    else __instance.Reflect().SetValue("clientPhysRiding", false);
                }
                __instance.playerAvatarVisuals.PhysRidingCheck();
                __instance.flashlightLightAim.clientAimPoint = (Vector3)stream.ReceiveNext();
                __instance.Reflect().SetValue("playerPing", (int)stream.ReceiveNext());
                return false;
            }
            catch (Exception e)
            {
                Settings.s_DebugMessage = "Msg: " + e.Message + "\nSrc: " + e.Source + "\n" + e.StackTrace;
                return true;
            }
        }

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

        [HarmonyPatch(typeof(MenuPageLobby), "PlayerAdd"), HarmonyPostfix]
        public static void PlayerAdd()
        {
            if (!SemiFunc.RunIsLobbyMenu()) return;
            REPOssessed.Instance.AlertUsingREPOssessed();
        }

        public static Dictionary<PhysGrabObject, PlayerAvatar> LastGrabbedPhysObjects = new Dictionary<PhysGrabObject, PlayerAvatar>();

        [HarmonyPatch(typeof(PhysGrabObject), "GrabStarted"), HarmonyPostfix]
        public static void GrabStarted(PhysGrabObject __instance, PhysGrabber player)
        {
            if (!LastGrabbedPhysObjects.ContainsKey(__instance) && __instance.playerGrabbing.Count == 1) LastGrabbedPhysObjects.Add(__instance, player.playerAvatar);
            else if (LastGrabbedPhysObjects.ContainsKey(__instance) && LastGrabbedPhysObjects[__instance] != player.playerAvatar) LastGrabbedPhysObjects[__instance] = player.playerAvatar;
        }

        [HarmonyPatch(typeof(PhysGrabObject), "GrabEnded"), HarmonyPrefix]
        public static void GrabEnded(PhysGrabObject __instance, PhysGrabber player)
        {
            if (!LastGrabbedPhysObjects.ContainsKey(__instance) && __instance.playerGrabbing.Count == 1) LastGrabbedPhysObjects.Add(__instance, player.playerAvatar);
            else if (LastGrabbedPhysObjects.ContainsKey(__instance) && LastGrabbedPhysObjects[__instance] != player.playerAvatar) LastGrabbedPhysObjects[__instance] = player.playerAvatar;
        }
    }
}
