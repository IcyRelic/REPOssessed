using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using REPOssessed.Cheats;
using REPOssessed.Cheats.Components;
using REPOssessed.Manager;
using REPOssessed.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace REPOssessed.Handler
{
    public class PlayerHandler
    {
        private static List<string> rpcBlockedClients = new List<string>();
        public static Dictionary<string, Queue<RPCData>> rpcHistory = new Dictionary<string, Queue<RPCData>>();

        private PlayerAvatar player = null;
        public PlayerVoiceChat playerVoiceChat = null;
        public PhysGrabObject physGrabObject = null;

        public Player photonPlayer => player.photonView.Owner;
        public string steamId => player.Reflect().GetValue<string>("steamID");

        public PlayerHandler(PlayerAvatar player)
        {
            this.player = player;
            this.playerVoiceChat = player?.Reflect()?.GetValue<PlayerVoiceChat>("voiceChat") ?? null;
            this.physGrabObject = player.physGrabber?.Reflect()?.GetValue<PhysGrabObject>("grabbedPhysGrabObject") ?? null;
        }

        public static void ClearRPCHistory() => rpcHistory.Clear();

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
            if (!rpcHistory.ContainsKey(steamId)) rpcHistory.Add(steamId, new Queue<RPCData>());
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

                if (player.Handle().IsLocalPlayer()) return true;

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
                Settings.s_DebugMessage = "Msg: " + e.Message + "\nSrc: " + e.Source + "\n" + e.StackTrace;
                return true;
            }
        }

        private void CleanupRPCHistory()
        {
            var queue = GetRPCHistory();
            while (queue.Count > 0 && queue.Peek().IsExpired()) queue.Dequeue();
        }

        public Player PhotonPlayer() => player.photonView.Owner;
        public string GetSteamID() => player.Reflect().GetValue<string>("steamID");
        public bool IsLocalPlayer() => player.Reflect().GetValue<bool>("isLocal");
        public int GetHealth() => player.playerHealth?.Reflect().GetValue<int>("health") ?? 0;
        public int GetMaxHealth() => player.playerHealth?.Reflect().GetValue<int>("maxHealth") ?? 0;
        public bool IsDead() => player.Reflect().GetValue<bool>("deadSet");
        public bool IsCrowned() => SessionManager.instance != null && SessionManager.instance.Reflect().GetValue<string>("crownedPlayerSteamID") == GetSteamID();
        public void RevivePlayer()
        {
            if (IsDead()) player.Revive();
        }
        public void ForceTumble() => player.tumble?.TumbleSet(true, false);
        public bool IsMasterClient() => IsLocalPlayer() ? SemiFunc.IsMasterClientOrSingleplayer() : photonPlayer.IsMasterClient;
        public void Heal(int amount)
        {
            player.playerHealth.Reflect().SetValue("health", amount);
            StatsManager.instance.SetPlayerHealth(player.Handle().GetSteamID(), amount, false);
            if (GameManager.Multiplayer()) player.playerHealth.Reflect().GetValue<PhotonView>("photonView").RPC("UpdateHealthRPC", RpcTarget.Others, amount, GetMaxHealth(), false);
        }
        public void Hurt(int amount)
        {
            player.playerHealth.Reflect().SetValue("health", GetHealth() - amount);
            if (GetHealth() <= 0)
            {
                player.PlayerDeath(-1);
                player.playerHealth.Reflect().SetValue("health", 0);
                return;
            }
            StatsManager.instance.SetPlayerHealth(player.Handle().GetSteamID(), GetHealth(), false);
            if (GameManager.Multiplayer()) player.photonView.RPC("UpdateHealthRPC", RpcTarget.Others, GetHealth(), GetMaxHealth(), true);
        }
        public void Teleport(Vector3 position, Quaternion rotation)
        {
            if (!SemiFunc.IsMultiplayer())
            {
                player.Reflect().Invoke("SpawnRPC", position, rotation);
                return;
            }
            player.photonView.RPC("SpawnRPC", RpcTarget.All, position, rotation);
        }
        public bool IsTalking() => playerVoiceChat?.Reflect().GetValue<bool>("isTalking") ?? false;
        public string GetName() => player.Reflect().GetValue<string>("playerName");
    }

    public static class PlayerExtensions
    {
        public static Dictionary<PlayerAvatar, PlayerHandler> PlayerHandlers = new Dictionary<PlayerAvatar, PlayerHandler>();

        public static PlayerHandler Handle(this PlayerAvatar player)
        {
            if (player == null)
            {
                if (PlayerHandlers.ContainsKey(player)) PlayerHandlers.Remove(player);
                return null;
            }
            if (!PlayerHandlers.TryGetValue(player, out var handler))
            {
                handler = new PlayerHandler(player);
                PlayerHandlers[player] = handler;
            }
            return handler;
        }

        private static PlayerAvatar localPlayer;

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
        public static PlayerAvatar GamePlayer(this Player photonPlayer)
        {
            if (GameObjectManager.players == null) return null;
            return GameObjectManager.players.Find(x => x != null && x.Handle().PhotonPlayer() != null && x.Handle().PhotonPlayer().ActorNumber == photonPlayer.ActorNumber);
        }
    }
}
