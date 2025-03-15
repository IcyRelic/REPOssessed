using Photon.Pun;
using REPOssessed.Extensions;
using REPOssessed.Util;
using System;
using System.Linq;
using UnityEngine;

namespace REPOssessed.Handler
{
    public class ObjectHandler
    {
        private PhysGrabObject physGrabObject;
        public ItemAttributes itemAttributes;
        public ValuableObject valuableObject;

        public ObjectHandler(PhysGrabObject physGrabObject)
        {
            if (physGrabObject == null) return;
            this.physGrabObject = physGrabObject;
            itemAttributes = physGrabObject.gameObject.GetComponentHierarchy<ItemAttributes>();
            valuableObject = physGrabObject.gameObject.GetComponentHierarchy<ValuableObject>();
        }

        public string GetName()
        {
            if (IsShopItem()) return itemAttributes.item.itemName;
            return physGrabObject.name.Replace("(Clone)", "").Replace("Valuable", "").Trim();
        }
        public void Teleport(Vector3 position, Quaternion rotation) => physGrabObject.Teleport(position, rotation);
        public bool IsShopItem() => itemAttributes != null;
        public float GetValue() => valuableObject != null ? valuableObject.dollarValueCurrent : 0f;
        public bool IsPlayer() => physGrabObject != null && physGrabObject.Reflect().GetValue<bool>("isPlayer");
        public bool IsEnemy() => physGrabObject != null && physGrabObject.Reflect().GetValue<bool>("isEnemy");
        public bool IsValuable() => physGrabObject != null && physGrabObject.Reflect().GetValue<bool>("isValuable");
        public bool IsNonValuable() => physGrabObject != null && physGrabObject.Reflect().GetValue<bool>("isNonValuable");
        public bool IsHinge() => physGrabObject != null && physGrabObject.Reflect().GetValue<bool>("hasHinge");
        public bool IsCart() => physGrabObject != null && physGrabObject.Reflect().GetValue<bool>("isCart");
        public bool IsTrap() => physGrabObject != null && physGrabObject.GetComponent<Trap>();
        public PlayerAvatar GetLastPlayerHeld() => Patches.LastGrabbedPhysObjects.ToList().FirstOrDefault(p => p.Key == physGrabObject).Value;
    }

    public static class ObjectExtensions
    {
        public static ObjectHandler? Handle(this PhysGrabObject physGrabObject) => new ObjectHandler(physGrabObject);
    }
}
