using Photon.Pun;
using REPOssessed.Handler;
using REPOssessed.Manager;
using REPOssessed.Menu.Core;
using REPOssessed.Util;
using System.Linq;
using UnityEngine;

namespace REPOssessed.Menu.Tab
{
    internal class DebugTab : MenuTab
    {
        public DebugTab() : base("Debug") { }
        private Vector2 scrollPos = Vector2.zero;

        public override void Draw()
        {
            GUILayout.BeginVertical();
            MenuContent();
            GUILayout.EndVertical();         
        }

        private void MenuContent()
        {
            UI.VerticalSpace(ref scrollPos, () =>
            {

                if (GUILayout.Button("Clear Debug Message")) Settings.s_DebugMessage = "";
                GUILayout.TextArea(Settings.s_DebugMessage, GUILayout.Height(100));


                GUILayout.BeginHorizontal();
                GUILayout.Label("Master Client: ");
                GUILayout.FlexibleSpace();
                GUILayout.Label(PhotonNetwork.IsMasterClient ? "Yes" : "No");
                GUILayout.EndHorizontal();
                UI.Button("Log RPCS", () => PhotonNetwork.PhotonServerSettings.RpcList.ToList().ForEach(r => Debug.Log(r)));

                UI.Button("Debug all prefabs", () =>
                {
                    Object[] prefabs = Resources.LoadAll("", typeof(GameObject));
                    if (prefabs.Length == 0) return;
                    Debug.Log($"Found {prefabs.Length} prefabs!");
                    prefabs.Where(p => p != null).ToList().ForEach(p => Debug.Log($"Prefab: {p.name}"));
                });

                UI.Button("Raycast", () =>
                {
                    string debugMessage = "";
                    foreach (RaycastHit hit in SemiFunc.MainCamera().transform.SphereCastForward())
                    {
                        Collider collider = hit.collider;
                        debugMessage += $"Hit: {collider.name} => {collider.gameObject.name} => Layer {LayerMask.LayerToName(collider.gameObject.layer)} {collider.gameObject.layer}\n";
                    }
                    Debug.Log(debugMessage);
                });

                UI.Button("Test", () =>
                {
                    GameObjectManager.items.Where(i => i != null).ToList().ForEach(i => Debug.Log(i.Handle().GetName()));
                });

                UI.Button("Unload test", Loader.Unload);
            });
        }
    }
}