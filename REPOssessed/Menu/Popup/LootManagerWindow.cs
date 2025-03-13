﻿using REPOssessed.Handler;
using REPOssessed.Manager;
using REPOssessed.Menu.Core;
using REPOssessed.Util;
using System.Linq;
using UnityEngine;

namespace REPOssessed.Menu.Popup
{
    internal class LootManagerWindow : PopupMenu
    {
        public LootManagerWindow(int id) : base("LootManager.Title", new Rect(50f, 50f, 600f, 300f), id) { }

        private string s_search = "";
        private Vector2 scrollPos = Vector2.zero;

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
                UI.Button("LootManager.TeleportAllItems", () => TeleportAll());
                GUILayout.EndHorizontal();
                GUILayout.Space(20);
                UI.ButtonGrid(GameObjectManager.items.Where(i => i != null && i.Handle() != null && !i.Handle().IsCart() && i.Handle().IsValuable() || i.Handle().IsShopItem()).GroupBy(i => i.Handle().GetName()).Select(g => g.First()).ToList(), i => $"{i.Handle().GetName()} {GameObjectManager.items.Count(ii => ii != null && ii.Handle() != null && ii.Handle().GetName() == i?.Handle()?.GetName())}x", s_search, i => i?.Handle().Teleport(SemiFunc.MainCamera().transform.position, SemiFunc.MainCamera().transform.rotation), 3);
            });
            GUI.DragWindow();
        }

        public static void TeleportAll()
        {
            if (SemiFunc.MainCamera() == null || SemiFunc.MainCamera().transform == null) return;
            GameObjectManager.items.Where(i => i != null && i.Handle() != null && i.Handle().IsValuable() || i.Handle().IsShopItem()).ToList().ForEach(i => i.Handle().Teleport(SemiFunc.MainCamera().transform.position, SemiFunc.MainCamera().transform.rotation));
        }
    }
}
