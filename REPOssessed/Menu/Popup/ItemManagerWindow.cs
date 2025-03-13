using Photon.Pun;
using REPOssessed.Menu.Core;
using REPOssessed.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace REPOssessed.Menu.Popup
{
    internal class ItemManagerWindow : PopupMenu
    {
        public ItemManagerWindow(int id) : base("ItemManager.Title", new Rect(50f, 50f, 600f, 300f), id) { }

        private Vector2 scrollPos = Vector2.zero;
        private string s_search = "";
        private string s_amount = "1";

        public override void DrawContent(int windowID)
        {
            UI.VerticalSpace(ref scrollPos, () =>
            {
                if (!SemiFunc.IsMasterClientOrSingleplayer())
                {
                    UI.Label("General.HostRequired", Settings.c_error);
                    return;
                }

                GUILayout.BeginHorizontal();
                UI.Textbox("General.Search", ref s_search);
                UI.Textbox("ItemManager.Amount", ref s_amount, @"[^0-9]", 0, false);
                GUILayout.EndHorizontal();
                GUILayout.Space(20);
                UI.ButtonGrid(GetItems().Where(i => i.Key != null).OrderBy(i => GetName(i.Key.name)).ToList(), (i) => GetName(i.Key.name), s_search, (i) => SpawnItem(i.Key, i.Value), 3);
            });
            GUI.DragWindow();
        }

        public void SpawnItem(GameObject item, string path)
        {
            if (SemiFunc.MainCamera() == null || SemiFunc.MainCamera().transform == null) return;
            for (int i = 0; i < int.Parse(s_amount); i++)
            {
                if (GameManager.Multiplayer())
                {
                    if (path == "shop") PhotonNetwork.InstantiateRoomObject($"Items/{item.name}", SemiFunc.MainCamera().transform.position, SemiFunc.MainCamera().transform.rotation, 0);
                    else if (path == "surplus" || path == "enemy") PhotonNetwork.InstantiateRoomObject($"{GetValuablePath()}/{item.name}", SemiFunc.MainCamera().transform.position, SemiFunc.MainCamera().transform.rotation, 0);
                    else PhotonNetwork.InstantiateRoomObject($"{GetValuablePath()}/{path}/{item.name}", SemiFunc.MainCamera().transform.position, SemiFunc.MainCamera().transform.rotation, 0);
                }
                else Object.Instantiate(item, SemiFunc.MainCamera().transform.position, SemiFunc.MainCamera().transform.rotation);
            }
        }

        private string GetItemPath(string type)
        {
            switch (type.ToLower())
            {
                case "tiny":
                    return ValuableDirector.instance.Reflect().GetValue<string>("tinyPath");
                case "small":
                    return ValuableDirector.instance.Reflect().GetValue<string>("smallPath");
                case "medium":
                    return ValuableDirector.instance.Reflect().GetValue<string>("mediumPath");
                case "big":
                    return ValuableDirector.instance.Reflect().GetValue<string>("bigPath");
                case "wide":
                    return ValuableDirector.instance.Reflect().GetValue<string>("widePath");
                case "tall":
                    return ValuableDirector.instance.Reflect().GetValue<string>("tallPath");
                case "verytall":
                    return ValuableDirector.instance.Reflect().GetValue<string>("veryTallPath");
            }
            return null;
        }

        private Dictionary<GameObject, string> GetItems()
        {
            Dictionary<GameObject, string> items = new Dictionary<GameObject, string>();
            LevelGenerator.Instance.Level.ValuablePresets.ToList().ForEach(v =>
            {
                v.tiny.ToList().ForEach(v => items.Add(v, GetItemPath("tiny")));
                v.small.ToList().ForEach(v => items.Add(v, GetItemPath("small")));
                v.medium.ToList().ForEach(v => items.Add(v, GetItemPath("medium")));
                v.big.ToList().ForEach(v => items.Add(v, GetItemPath("big")));
                v.wide.ToList().ForEach(v => items.Add(v, GetItemPath("wide")));
                v.tall.ToList().ForEach(v => items.Add(v, GetItemPath("tall")));
                v.veryTall.ToList().ForEach(v => items.Add(v, GetItemPath("veryTall")));
            });
            items.Add(AssetManager.instance.surplusValuableSmall, "surplus");
            items.Add(AssetManager.instance.surplusValuableMedium, "surplus");
            items.Add(AssetManager.instance.surplusValuableBig, "surplus");
            items.Add(AssetManager.instance.enemyValuableSmall, "enemy");
            items.Add(AssetManager.instance.enemyValuableMedium, "enemy");
            items.Add(AssetManager.instance.enemyValuableBig, "enemy");
            StatsManager.instance.itemDictionary.ToList().ForEach(i => items.Add(i.Value.prefab, "shop"));
            return items;
        }

        private string GetValuablePath() => ValuableDirector.instance.Reflect().GetValue<string>("resourcePath").Replace("/", "");
        public string GetName(string name) => name.Replace("(Clone)", "").Replace("Valuable", "").Trim();
    }
}
