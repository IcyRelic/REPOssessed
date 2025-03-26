using REPOssessed.Handler;
using REPOssessed.Manager;
using REPOssessed.Menu.Core;
using REPOssessed.Util;
using Steamworks.Ugc;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace REPOssessed.Menu.Popup
{
    internal class LootManagerWindow : PopupMenu
    {
        public LootManagerWindow(int id) : base("LootManager.Title", new Rect(50f, 50f, 600f, 300f), id) { }

        private string s_search = "";
        private Vector2 scrollPos = Vector2.zero;

        public override void DrawContent(int windowID)
        {
            if (!REPOssessed.Instance.IsIngame)
            {
                UI.Label("General.MustBeIngame", Settings.c_error);
                GUI.DragWindow();
                return;
            }
            List<GroupedPhysGrabObject> groupedPhysGrabObject = GameObjectManager.items?.Where(i => i != null && i.Handle()?.IsValuable() == true || i.Handle()?.IsShopItem() == true).GroupBy(i => i.Handle()?.GetName()).Select(g => new GroupedPhysGrabObject { physGrabObject = g.FirstOrDefault(), Count = g.Count() }).ToList() ?? new List<GroupedPhysGrabObject>();
            if (groupedPhysGrabObject == null) groupedPhysGrabObject = new List<GroupedPhysGrabObject>();
            UI.VerticalSpace(ref scrollPos, () =>
            {
                GUILayout.BeginHorizontal();
                UI.Textbox("General.Search", ref s_search);
                UI.Button("LootManager.TeleportAllItems", () => TeleportAll(groupedPhysGrabObject));
                GUILayout.EndHorizontal();
                GUILayout.Space(20);
                UI.ButtonGrid(groupedPhysGrabObject, p => $"{p?.physGrabObject?.Handle()?.GetName()} {p.Count}x", s_search, p =>
                {
                    List<GroupedPhysGrabObject> items = groupedPhysGrabObject.Where(gp => gp == p).ToList();
                    Teleport(items[Random.Range(0, items.Count)]);
                }, 3);
                
            });
            GUI.DragWindow();
        }

        public static void TeleportAll(List<GroupedPhysGrabObject> groupedPhysGrabObject)
        {
            if (SemiFunc.MainCamera() == null || SemiFunc.MainCamera().transform == null) return;
            groupedPhysGrabObject.Where(i => i != null && i.physGrabObject != null).ToList().ForEach(i => Teleport(i));
        }

        private static void Teleport(GroupedPhysGrabObject groupedPhysGrabObject)
        {
            if (SemiFunc.MainCamera() == null || SemiFunc.MainCamera().transform == null || groupedPhysGrabObject == null || groupedPhysGrabObject.physGrabObject == null || groupedPhysGrabObject.physGrabObject.Handle() == null) return;
            groupedPhysGrabObject.physGrabObject.Handle().Teleport(SemiFunc.MainCamera().transform.position, SemiFunc.MainCamera().transform.rotation);
        }

        public class GroupedPhysGrabObject
        {
            public PhysGrabObject physGrabObject { get; set; }
            public int Count { get; set; }
        }
    }
}
