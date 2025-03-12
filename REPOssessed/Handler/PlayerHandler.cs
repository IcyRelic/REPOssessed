using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using REPOssessed.Cheats.Components;
using REPOssessed.Manager;
using REPOssessed.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using Object = UnityEngine.Object;
using Vector3 = UnityEngine.Vector3;

namespace REPOssessed.Handler
{
    public class PlayerHandler
    {
        private static List<string> rpcBlockedClients = new List<string>();
        public static Dictionary<string, Queue<RPCData>> rpcHistory = new Dictionary<string, Queue<RPCData>>();
        public static List<PlayerAvatar> GetAlivePlayers() => GameObjectManager.players.Where(p => p != null && !p.Handle().IsDead()).ToList();

        private PlayerAvatar player;
        public Player photonPlayer => player.photonView.Owner;
        public string steamId => player.Reflect().GetValue<string>("steamID");

        public PlayerHandler(PlayerAvatar player)
        {
            this.player = player;
        }

        public static void ClearRPCHistory() => rpcHistory.Clear();

        public Enemy GetClosestEnemy() => GameObjectManager.enemies.OrderBy(x => Vector3.Distance(x.transform.position, player.transform.position)).FirstOrDefault();
        public PlayerAvatar GetClosestPlayer() => GetAlivePlayers().Where(x => x.GetInstanceID() != player.GetInstanceID()).OrderBy(x => Vector3.Distance(x.transform.position, player.transform.position)).FirstOrDefault();

        public void RPC(string name, RpcTarget target, params object[] args) => player.photonView.RPC(name, target, args);

        public bool IsRPCBlocked() => photonPlayer != null && rpcBlockedClients.Contains(steamId);

        public bool IsREPOssessedUser() => player != null && GameObjectManager.REPOssessedPlayers.Contains(player) || player.Handle().IsLocalPlayer();

        public void BlockRPC()
        {
            if (IsRPCBlocked() || photonPlayer == null) return;
            rpcBlockedClients.Add(steamId);
        }

        public void UnblockRPC()
        {
            if (!IsRPCBlocked() || photonPlayer == null) return;
            rpcBlockedClients.Remove(steamId);
        }

        public void ToggleRPCBlock()
        {
            if (photonPlayer is null) return;
            if (IsRPCBlocked()) rpcBlockedClients.Remove(steamId);
            else rpcBlockedClients.Add(steamId);
        }

        public Queue<RPCData> GetRPCHistory()
        {
            if (!rpcHistory.ContainsKey(steamId))
                rpcHistory.Add(steamId, new Queue<RPCData>());
            return rpcHistory[steamId];
        }
        public List<RPCData> GetRPCHistory(string rpc) => GetRPCHistory().ToList().FindAll(r => r.rpc.StartsWith(rpc));

        public List<RPCData> GetRPCHistory(string rpc, int seconds) => GetRPCHistory().ToList().FindAll(r => r.rpc.StartsWith(rpc) && r.IsRecent(seconds));
        public List<RPCData> GetRPCHistory(string rpc, int seconds, bool suspected) => GetRPCHistory().ToList().FindAll(r => r.rpc.StartsWith(rpc) && r.IsRecent(seconds) && r.suspected == suspected);
        public RPCData GetRPCMatch(string rpc, int seconds, object data) => GetRPCHistory().FirstOrDefault(r => r.rpc.StartsWith(rpc) && r.IsRecent(seconds) && r.data.Equals(data));
        public RPCData GetRPCMatch(string rpc, int seconds, object data, bool suspected) => GetRPCHistory().FirstOrDefault(r => r.rpc.StartsWith(rpc) && r.IsRecent(seconds) && r.data.Equals(data) && r.suspected == suspected);
        public RPCData GetRPCMatch(string rpc, int seconds, Func<object, bool> predicate) => GetRPCHistory().FirstOrDefault(r => r.rpc.StartsWith(rpc) && r.IsRecent(seconds) && predicate(r.data));
        public RPCData GetRPCMatch(string rpc, int seconds, Func<object, bool> predicate, bool suspected) => GetRPCHistory().FirstOrDefault(r => r.rpc.StartsWith(rpc) && r.IsRecent(seconds) && predicate(r.data) && r.suspected == suspected);
        public bool HasSentRPC(string rpc, int seconds) => GetRPCHistory().ToList().FindAll(r => r.rpc.StartsWith(rpc) && r.IsRecent(seconds)).Count > 0;
        public bool HasSentRPC(string rpc, int seconds, bool suspected) => GetRPCHistory().ToList().FindAll(r => r.rpc.StartsWith(rpc) && r.IsRecent(seconds) && r.suspected == suspected).Count > 0;
        public bool HasSentRPC(string rpc, int seconds, object data) => GetRPCHistory().ToList().FindAll(r => r.rpc.StartsWith(rpc) && r.IsRecent(seconds) && r.data.Equals(data)).Count > 0;
        public bool HasSentRPC(string rpc, int seconds, Func<object, bool> predicate) => GetRPCHistory().ToList().FindAll(r => r.rpc.StartsWith(rpc) && r.IsRecent(seconds) && predicate(r.data)).Count > 0;
        public bool HasSentRPC(string rpc, int seconds, Func<object, bool> predicate, bool suspected) => GetRPCHistory().ToList().FindAll(r => r.rpc.StartsWith(rpc) && r.IsRecent(seconds) && predicate(r.data) && r.suspected == suspected).Count > 0;
        public List<RPCData> GetAllRPCHistory() => rpcHistory.Values.SelectMany(x => x).ToList();
        public List<RPCData> GetAllRPCHistory(int seconds) => GetAllRPCHistory().FindAll(r => r.IsRecent(seconds));
        public List<RPCData> GetAllRPCHistory(string rpc, int seconds) => GetAllRPCHistory().FindAll(r => r.rpc.StartsWith(rpc) && r.IsRecent(seconds));
        public List<RPCData> GetAllRPCHistory(string rpc, int seconds, bool suspected) => GetAllRPCHistory().FindAll(r => r.rpc.StartsWith(rpc) && r.IsRecent(seconds) && r.suspected == suspected);
        public RPCData GetAnyRPCMatch(string rpc, int seconds, object data) => GetAllRPCHistory().FirstOrDefault(r => r.rpc.StartsWith(rpc) && r.IsRecent(seconds) && r.data.Equals(data));
        public RPCData GetAnyRPCMatch(string rpc, int seconds, object data, bool suspected) => GetAllRPCHistory().FirstOrDefault(r => r.rpc.StartsWith(rpc) && r.IsRecent(seconds) && r.data.Equals(data) && r.suspected == suspected);
        public RPCData GetAnyRPCMatch(string rpc, int seconds, Func<object, bool> predicate) => GetAllRPCHistory().FirstOrDefault(r => r.rpc.StartsWith(rpc) && r.IsRecent(seconds) && predicate(r.data));
        public RPCData GetAnyRPCMatch(string rpc, int seconds, Func<object, bool> predicate, bool suspected) => GetAllRPCHistory().FirstOrDefault(r => r.rpc.StartsWith(rpc) && r.IsRecent(seconds) && predicate(r.data) && r.suspected == suspected);
        public bool HasAnySentRPC(string rpc, int seconds) => GetAllRPCHistory().FindAll(r => r.rpc.StartsWith(rpc) && r.IsRecent(seconds)).Count > 0;
        public bool HasAnySentRPC(string rpc, int seconds, bool suspected) => GetAllRPCHistory().FindAll(r => r.rpc.StartsWith(rpc) && r.IsRecent(seconds) && r.suspected == suspected).Count > 0;
        public bool HasAnySentRPC(string rpc, int seconds, object data) => GetAllRPCHistory().FindAll(r => r.rpc.StartsWith(rpc) && r.IsRecent(seconds) && r.data.Equals(data)).Count > 0;
        public bool HasAnySentRPC(string rpc, int seconds, Func<object, bool> predicate) => GetAllRPCHistory().FindAll(r => r.rpc.StartsWith(rpc) && r.IsRecent(seconds) && predicate(r.data)).Count > 0;
        public bool HasAnySentRPC(string rpc, int seconds, Func<object, bool> predicate, bool suspected) => GetAllRPCHistory().FindAll(r => r.rpc.StartsWith(rpc) && r.IsRecent(seconds) && predicate(r.data) && r.suspected == suspected).Count > 0;

        public bool OnReceivedRPC(string rpc, Hashtable rpcHash)
        {
            try
            {
                if (rpcHash == null || player == null || photonPlayer == null) return true;

                RPCData rpcData = new RPCData(photonPlayer, rpc, rpcHash);

                if (rpcData == null) return true;

                object[] parameters = null;
                if (rpcHash.ContainsKey(Patches.keyByteFour)) parameters = (object[])rpcHash[Patches.keyByteFour];

                if (!Patches.IgnoredRPCDebugs.Contains(rpc) && parameters != null) Debug.LogWarning($"RPC Params '{string.Join(", ", parameters.Select(p => p?.ToString() ?? "null"))}'");

                if (player.Handle().GetSteamID() == player.GetLocalPlayer().Handle().GetSteamID()) return true;

                /*
                if (rpc.Equals("OutroStartRPC"))
                {
                    Debug.LogError($"{photonPlayer.NickName} is probably trying to crash you!");
                    rpcData.SetSuspected();
                    return false;
                }

                if (rpc.Equals("SetDisabledRPC"))
                {
                    Debug.LogError($"{photonPlayer.NickName} is probably trying to disable you!");
                    rpcData.SetSuspected();
                    return false;
                }
                */

                GetRPCHistory()?.Enqueue(rpcData);
                CleanupRPCHistory();
                return true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return true;
            }
        }

        private void CleanupRPCHistory()
        {
            var queue = GetRPCHistory();
            while (queue.Count > 0 && queue.Peek().IsExpired()) queue.Dequeue();
        }

        public PhysGrabObject GetLastHeldPhysGrabObject() => player.physGrabber.Reflect().GetValue<PhysGrabObject>("grabbedPhysGrabObject");
        public PhysGrabObject GetHeldPhysGrabObject() => player.physGrabber.grabbed ? player.physGrabber.Reflect().GetValue<PhysGrabObject>("grabbedPhysGrabObject") : null;
        public ItemEquippable GetHeldItemEquippable() => GetHeldPhysGrabObject()?.GetComponent<ItemEquippable>();
        public Player PhotonPlayer() => player.photonView.Owner;
        public string GetSteamID() => player.Reflect().GetValue<string>("steamID");
        public bool IsLocalPlayer() => player.Reflect().GetValue<bool>("isLocal");
        public int GetHealth() => player.playerHealth.Reflect().GetValue<int>("health");
        public bool IsDead() => player.Reflect().GetValue<bool>("deadSet");
        public bool IsSpectating() => player.spectateCamera.GetComponentHierarchy<SpectateCamera>().CheckState(SpectateCamera.State.Normal);
        public PlayerAvatar GetPlayerSpectating() => player.spectateCamera.GetComponentHierarchy<SpectateCamera>().Reflect().GetValue<PlayerAvatar>("player");
        public void RevivePlayer()
        {
            if (player.Handle().IsDead()) player.Revive();
        }

        public string GetName() => string.IsNullOrEmpty(player.Reflect().GetValue<string>("playerName")) ? player.name : player.Reflect().GetValue<string>("playerName");
    }

    public static class PlayerExtensions
    {
        private static PlayerAvatar localPlayer;
        public static PlayerHandler Handle(this PlayerAvatar player) => new PlayerHandler(player);
        public static PlayerAvatar GetLocalPlayer(this PlayerAvatar player)
        {
            if (localPlayer == null)
            {
                localPlayer = GameObjectManager.players?.FirstOrDefault(p => p != null && p.Handle().IsLocalPlayer());
                if (localPlayer == null) return null;
            }
            return localPlayer;
        }
    }

    public static class PhotonPlayerExtensions
    {
        public static string GetSteamID(this Player photonPlayer)
        {
            return photonPlayer.GamePlayer().Handle().GetSteamID();
        }
        public static PlayerAvatar GamePlayer(this Player photonPlayer)
        {
            if (GameObjectManager.players == null) return null;
            return GameObjectManager.players.Find(x => x != null && x.Handle().PhotonPlayer() != null && x.Handle().PhotonPlayer().ActorNumber == photonPlayer.ActorNumber);
        }
    }
}
