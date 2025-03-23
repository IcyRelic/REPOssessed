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
        public PhysGrabObjectImpactDetector physGrabObjectImpactDetector;

        public ObjectHandler(PhysGrabObject physGrabObject)
        {
            if (physGrabObject == null) return;
            this.physGrabObject = physGrabObject;
            itemAttributes = physGrabObject?.gameObject.GetComponentHierarchy<ItemAttributes>();
            valuableObject = physGrabObject?.gameObject.GetComponentHierarchy<ValuableObject>();
            physGrabObjectImpactDetector = physGrabObject?.gameObject.GetComponentHierarchy<PhysGrabObjectImpactDetector>();
        }

        public string GetName()
        {
            if (IsShopItem()) return itemAttributes?.item?.itemName;
            return physGrabObject?.name.Replace("(Clone)", "").Replace("Valuable", "").Trim();
        }
        public void Break(bool effects = true)
        {
            if (!IsValuable() && IsEnemy() && IsPlayer()) return;
            if (!SemiFunc.IsMultiplayer())
            {
                physGrabObjectImpactDetector?.DestroyObjectRPC(effects);
                return;
            }
            physGrabObjectImpactDetector?.Reflect().GetValue<PhotonView>("photonView")?.RPC("DestroyObjectRPC", RpcTarget.All, effects);
        }
        public void Teleport(Vector3 position, Quaternion rotation) => physGrabObject.Teleport(position, rotation);
        public bool IsShopItem() => itemAttributes != null;
        public float GetValue() => valuableObject != null ? valuableObject.dollarValueCurrent : 0f;
        public bool IsPlayer() => physGrabObject?.Reflect().GetValue<bool>("isPlayer") ?? false;
        public bool IsEnemy() => physGrabObject?.Reflect().GetValue<bool>("isEnemy") ?? false;
        public bool IsValuable() => physGrabObject?.Reflect().GetValue<bool>("isValuable") ?? false;
        public bool IsNonValuable() => physGrabObject?.Reflect().GetValue<bool>("isNonValuable") ?? false;
        public bool IsHinge() => physGrabObject?.Reflect().GetValue<bool>("hasHinge") ?? false;
        public bool IsCart() => physGrabObject?.Reflect().GetValue<bool>("isCart") ?? false;
        public bool IsTrap() => physGrabObject?.GetComponent<Trap>() ?? false;
        public PlayerAvatar GetLastPlayerHeld() => Patches.LastGrabbedPhysObjects.ToList().FirstOrDefault(p => p.Key == physGrabObject).Value;
    }

    public static class ObjectExtensions
    {
        public static ObjectHandler? Handle(this PhysGrabObject physGrabObject) => new ObjectHandler(physGrabObject);
    }
}
