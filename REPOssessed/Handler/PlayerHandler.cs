using Photon.Pun;
using Photon.Realtime;
using REPOssessed.Cheats.Components;
using REPOssessed.Manager;
using REPOssessed.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace REPOssessed.Handler
{
    public class PlayerHandler
    {
        private static List<string> rpcBlockedClients = new List<string>();
        public static Dictionary<string, Queue<RPCData>> rpcHistory = new Dictionary<string, Queue<RPCData>>();

        public PlayerAvatar player;
        public PhotonView? photonView;
        public Player? photonPlayer;
        public Rigidbody? rigidbody;
        public PlayerTumble? tumble;
        public PlayerCosmetics cosmetics;
        private PlayerDeathHead? playerDeathHead;

        public PlayerHandler(PlayerAvatar player)
        {
            this.player = player;
            this.photonView = player.photonView;
            this.photonPlayer = photonView?.Owner;
            this.playerDeathHead = player.Reflect().GetValue<PlayerDeathHead>("playerDeathHead");
            tumble = player.Reflect()?.GetValue<PlayerTumble>("tumble");
            this.rigidbody = tumble?.Reflect().GetValue<Rigidbody>("rb");
            this.cosmetics = player.playerCosmetics;
        }

        public static void ClearRPCHistory() => rpcHistory.Clear();

        public bool IsRPCBlocked()
        {
            string? steamID = GetSteamID();
            return !string.IsNullOrEmpty(steamID) && photonPlayer != null && rpcBlockedClients.Contains(steamID);
        }

        public void BlockRPC()
        {
            string? steamID = GetSteamID();
            if (string.IsNullOrEmpty(steamID)) return;
            if (IsRPCBlocked()) return;
            rpcBlockedClients.Add(steamID);
        }

        public void UnblockRPC()
        {
            string? steamID = GetSteamID();
            if (string.IsNullOrEmpty(steamID)) return;
            if (!IsRPCBlocked()) return;
            rpcBlockedClients.Remove(steamID);
        }

        public void ToggleRPCBlock()
        {
            string? steamID = GetSteamID();
            if (string.IsNullOrEmpty(steamID)) return;
            if (IsRPCBlocked()) rpcBlockedClients.Remove(steamID);
            else rpcBlockedClients.Add(steamID);
        }

        public Queue<RPCData>? GetRPCHistory()
        {
            string? steamID = GetSteamID();
            if (string.IsNullOrEmpty(steamID)) return null;
            if (!rpcHistory.ContainsKey(steamID)) rpcHistory.Add(steamID, new Queue<RPCData>());
            return rpcHistory[steamID];
        }
        public List<RPCData> GetRPCHistory(string rpc) => GetRPCHistory().ToList().FindAll(r => r.rpc.StartsWith(rpc));

        public List<RPCData> GetRPCHistory(string rpc, int seconds) => GetRPCHistory().ToList().FindAll(r => r.rpc.StartsWith(rpc) && r.IsRecent(seconds));
        public List<RPCData> GetRPCHistory(string rpc, int seconds, bool suspected) => GetRPCHistory().ToList().FindAll(r => r.rpc.StartsWith(rpc) && r.IsRecent(seconds) && r.suspected == suspected);
        public RPCData GetRPCMatch(string rpc, int seconds, object data) => GetRPCHistory().FirstOrDefault(r => r.rpc.StartsWith(rpc) && r.IsRecent(seconds) && r.data != null && r.data.Equals(data));
        public RPCData GetRPCMatch(string rpc, int seconds, object data, bool suspected) => GetRPCHistory().FirstOrDefault(r => r.rpc.StartsWith(rpc) && r.IsRecent(seconds) && r.data != null && r.data.Equals(data) && r.suspected == suspected);
        public RPCData GetRPCMatch(string rpc, int seconds, Func<object, bool> predicate) => GetRPCHistory().FirstOrDefault(r => r.rpc.StartsWith(rpc) && r.IsRecent(seconds) && r.data != null && predicate(r.data));
        public RPCData GetRPCMatch(string rpc, int seconds, Func<object, bool> predicate, bool suspected) => GetRPCHistory().FirstOrDefault(r => r.rpc.StartsWith(rpc) && r.IsRecent(seconds) && r.data != null && predicate(r.data) && r.suspected == suspected);
        public bool HasSentRPC(string rpc, int seconds) => GetRPCHistory().ToList().FindAll(r => r.rpc.StartsWith(rpc) && r.IsRecent(seconds)).Count > 0;
        public bool HasSentRPC(string rpc, int seconds, bool suspected) => GetRPCHistory().ToList().FindAll(r => r.rpc.StartsWith(rpc) && r.IsRecent(seconds) && r.suspected == suspected).Count > 0;
        public bool HasSentRPC(string rpc, int seconds, object data) => GetRPCHistory().ToList().FindAll(r => r.rpc.StartsWith(rpc) && r.IsRecent(seconds) && r.data != null && r.data.Equals(data)).Count > 0;
        public bool HasSentRPC(string rpc, int seconds, Func<object, bool> predicate) => GetRPCHistory().ToList().FindAll(r => r.rpc.StartsWith(rpc) && r.IsRecent(seconds) && r.data != null && predicate(r.data)).Count > 0;
        public bool HasSentRPC(string rpc, int seconds, Func<object, bool> predicate, bool suspected) => GetRPCHistory().ToList().FindAll(r => r.rpc.StartsWith(rpc) && r.IsRecent(seconds) && r.data != null && predicate(r.data) && r.suspected == suspected).Count > 0;
        public List<RPCData> GetAllRPCHistory() => rpcHistory.Values.SelectMany(x => x).ToList();
        public List<RPCData> GetAllRPCHistory(int seconds) => GetAllRPCHistory().FindAll(r => r.IsRecent(seconds));
        public List<RPCData> GetAllRPCHistory(string rpc, int seconds) => GetAllRPCHistory().FindAll(r => r.rpc.StartsWith(rpc) && r.IsRecent(seconds));
        public List<RPCData> GetAllRPCHistory(string rpc, int seconds, bool suspected) => GetAllRPCHistory().FindAll(r => r.rpc.StartsWith(rpc) && r.IsRecent(seconds) && r.suspected == suspected);
        public RPCData GetAnyRPCMatch(string rpc, int seconds, object data) => GetAllRPCHistory().FirstOrDefault(r => r.rpc.StartsWith(rpc) && r.IsRecent(seconds) && r.data != null && r.data.Equals(data));
        public RPCData GetAnyRPCMatch(string rpc, int seconds, object data, bool suspected) => GetAllRPCHistory().FirstOrDefault(r => r.rpc.StartsWith(rpc) && r.IsRecent(seconds) && r.data != null && r.data.Equals(data) && r.suspected == suspected);
        public RPCData GetAnyRPCMatch(string rpc, int seconds, Func<object, bool> predicate) => GetAllRPCHistory().FirstOrDefault(r => r.rpc.StartsWith(rpc) && r.IsRecent(seconds) && r.data != null && predicate(r.data));
        public RPCData GetAnyRPCMatch(string rpc, int seconds, Func<object, bool> predicate, bool suspected) => GetAllRPCHistory().FirstOrDefault(r => r.rpc.StartsWith(rpc) && r.IsRecent(seconds) && r.data != null && predicate(r.data) && r.suspected == suspected);
        public bool HasAnySentRPC(string rpc, int seconds) => GetAllRPCHistory().FindAll(r => r.rpc.StartsWith(rpc) && r.IsRecent(seconds)).Count > 0;
        public bool HasAnySentRPC(string rpc, int seconds, bool suspected) => GetAllRPCHistory().FindAll(r => r.rpc.StartsWith(rpc) && r.IsRecent(seconds) && r.suspected == suspected).Count > 0;
        public bool HasAnySentRPC(string rpc, int seconds, object data) => GetAllRPCHistory().FindAll(r => r.rpc.StartsWith(rpc) && r.IsRecent(seconds) && r.data != null && r.data.Equals(data)).Count > 0;
        public bool HasAnySentRPC(string rpc, int seconds, Func<object, bool> predicate) => GetAllRPCHistory().FindAll(r => r.rpc.StartsWith(rpc) && r.IsRecent(seconds) && r.data != null && predicate(r.data)).Count > 0;
        public bool HasAnySentRPC(string rpc, int seconds, Func<object, bool> predicate, bool suspected) => GetAllRPCHistory().FindAll(r => r.rpc.StartsWith(rpc) && r.IsRecent(seconds) && r.data != null && predicate(r.data) && r.suspected == suspected).Count > 0;

        public bool OnReceivedRPC(string rpc, Hashtable rpcHash)
        {
            try
            {
                if (rpcHash == null || photonPlayer == null) return true;
                RPCData rpcData = new RPCData(photonPlayer, rpc, rpcHash);
                if (rpcData == null) return true;

                if (!Patches.IgnoredRPCDebugs.Contains(rpc) && rpcHash.TryGetValue(Patches.keyByteFour, out object? rpcParameters) && rpcParameters is object[] parameters) Debug.Log($"RPC Params '{string.Join(", ", parameters.Select(p => p?.ToString() ?? "null"))}'");

                if (IsLocalPlayer()) return true;

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
            Queue<RPCData>? queue = GetRPCHistory();
            if (queue != null) while (queue.Count > 0 && queue.Peek().IsExpired()) queue.Dequeue();
        }

        public PhysGrabObject? GetHeldPhysGrabObject()
        {
            PhysGrabObject? physGrabObject = player?.physGrabber?.Reflect()?.GetValue<PhysGrabObject>("grabbedPhysGrabObject");
            return (physGrabObject != null && physGrabObject.grabbed) ? physGrabObject : null;
        }
        public string? GetSteamID() => player.Reflect()?.GetValue<string>("steamID");
        public bool IsLocalPlayer() => player.Reflect()?.GetValue<bool>("isLocal") ?? false;
        public int GetHealth() => player.playerHealth?.Reflect().GetValue<int>("health") ?? 0;
        public int GetMaxHealth() => player.playerHealth?.Reflect().GetValue<int>("maxHealth") ?? 0;
        public bool IsDead() => player.Reflect().GetValue<bool>("deadSet");
        public bool IsCrowned() => SessionManager.instance.crownedPlayerSteamID == GetSteamID();
        public void RevivePlayer()
        {
            if (!GameUtil.IsMasterClient() || !IsDead()) return;
            player.Revive();
        }
        public void ForceTumble() => tumble?.TumbleRequest(true, false); 
        public bool IsMasterClient() => IsLocalPlayer() ? SemiFunc.IsMasterClientOrSingleplayer() : photonPlayer?.IsMasterClient ?? false;
        public void Heal(int amount) => player.playerHealth?.HealOther(amount, false);
        public void Hurt(int amount)
        {
            if (!GameUtil.IsMasterClient() && !IsLocalPlayer()) return;
            player.playerHealth?.HurtOther(amount, Vector2.zero, false);
        }
        public void Kill() => Hurt(GetHealth());
        public void Teleport(Vector3 position, Quaternion rotation)
        {
            if (!GameUtil.IsMasterClient())
            {
                if (IsLocalPlayer())
                {
                    PlayerController.instance?.transform?.SetPositionAndRotation(position, rotation);
                    player.Reflect()?.SetValue("clientPosition", position);
                    player.Reflect()?.SetValue("clientPositionCurrent", position);
                    player.Reflect()?.SetValue("clientRotation", rotation);
                    player.Reflect()?.SetValue("clientRotationCurrent", rotation);
                    player.transform?.SetPositionAndRotation(position, rotation);
                    rigidbody?.MovePosition(position);
                    rigidbody?.MoveRotation(rotation);
                    player.Reflect()?.SetValue("spawnPosition", position);
                    player.Reflect()?.SetValue("spawnRotation", rotation);
                    player.playerAvatarVisuals?.Reflect()?.SetValue("visualPosition", position);
                }
            }
            else player.Spawn(position, rotation);
        }
        public void Crown()
        {
            if (!GameUtil.IsMasterClient()) return;
            PunManager.instance?.CrownPlayerSync(GetSteamID());
        }

        public void SendMessage(string message)
        {
            if (!GameUtil.IsMasterClient() && !IsLocalPlayer()) return;
            player.ChatMessageSend(message);
        }
        public void SetColor(int colorIndex)
        {
            if (!IsLocalPlayer()) return;
            if (GameManager.Multiplayer()) cosmetics.Reflect()?.GetValue<PhotonView>("photonView")?.RPC("SetupColorsAllRPC", RpcTarget.All, colorIndex);
            else cosmetics.Reflect().Invoke("SetupColorsAllLogic", colorIndex);
        }
        public bool IsTalking() => GetPlayerVoiceChat()?.Reflect().GetValue<bool>("isTalking") ?? false;
        public string GetName() => player.Reflect()?.GetValue<string>("playerName") ?? "Unknown";
        public PlayerVoiceChat? GetPlayerVoiceChat() => RunManager.instance?.Reflect()?.GetValue<List<PlayerVoiceChat>>("voiceChats")?.FirstOrDefault(v => v.Reflect()?.GetValue<PlayerAvatar>("playerAvatar") == player);
        public PlayerDeathHead? GetPlayerDeathHead() => IsDead() ? playerDeathHead : null;
    }

    public static class PlayerHandlerExtensions
    {
        public static Dictionary<int, PlayerHandler> PlayerHandlers = new Dictionary<int, PlayerHandler>();

        public static PlayerHandler? Handle(this PlayerAvatar playerAvatar)
        {
            if (playerAvatar == null) return null;
            int id = playerAvatar.GetInstanceID();
            if (!PlayerHandlers.TryGetValue(id, out PlayerHandler playerHandler))
            {
                playerHandler = new PlayerHandler(playerAvatar);
                PlayerHandlers[id] = playerHandler;
            }
            return playerHandler;
        }

        public static PlayerAvatar? GamePlayer(this Player photonPlayer)
        {
            return GameObjectManager.players.FirstOrDefault(p => p?.Handle()?.photonPlayer?.ActorNumber == photonPlayer.ActorNumber);
        }
    }
}
