using Photon.Pun;
using REPOssessed.Extensions;
using REPOssessed.Util;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace REPOssessed.Handler
{
    public class ObjectHandler
    {
        private PhysGrabObject physGrabObject = null;
        public ItemAttributes itemAttributes = null;
        public ValuableObject valuableObject = null;
        public PhysGrabObjectImpactDetector physGrabObjectImpactDetector = null;
        public Trap trap = null;
        public EnemyRigidbody enemyRigidbody = null;

        public ObjectHandler(PhysGrabObject physGrabObject)
        {
            this.physGrabObject = physGrabObject;
            this.itemAttributes = physGrabObject?.GetComponentHierarchy<ItemAttributes>();
            this.valuableObject = physGrabObject?.GetComponentHierarchy<ValuableObject>();
            this.physGrabObjectImpactDetector = physGrabObject?.GetComponentHierarchy<PhysGrabObjectImpactDetector>();
            this.trap = physGrabObject.GetComponentHierarchy<Trap>();
            this.enemyRigidbody = physGrabObject.GetComponentHierarchy<EnemyRigidbody>();
        }

        public string GetName() => IsShopItem() ? itemAttributes?.item?.itemName : physGrabObject?.name.Replace("(Clone)", "").Replace("Valuable", "").Trim();
        public void Break(bool effects = true)
        {
            if (IsEnemy() || IsPlayer()) return;
            if (!SemiFunc.IsMultiplayer())
            {
                physGrabObjectImpactDetector?.DestroyObjectRPC(effects);
                return;
            }
            physGrabObjectImpactDetector?.Reflect()?.GetValue<PhotonView>("photonView")?.RPC("DestroyObjectRPC", RpcTarget.All, effects);
        }
        public void Teleport(Vector3 position, Quaternion rotation) => physGrabObject?.Teleport(position, rotation);
        public bool IsShopItem() => itemAttributes != null;
        public float GetValue() => valuableObject != null ? valuableObject.dollarValueCurrent : 0f;
        public bool IsPlayer() => physGrabObject?.Handle()?.GetName()?.Contains("Player") ?? false;
        public bool IsEnemy() => enemyRigidbody != null || (physGrabObject?.Reflect()?.GetValue<bool>("isEnemy") ?? false);
        public bool IsValuable() => physGrabObject?.Reflect().GetValue<bool>("isValuable") ?? false;
        public bool IsNonValuable() => physGrabObject?.Reflect().GetValue<bool>("isNonValuable") ?? false;
        public bool IsHinge() => physGrabObject?.Reflect().GetValue<bool>("hasHinge") ?? false;
        public bool IsCart() => physGrabObject?.Reflect().GetValue<bool>("isCart") ?? false;
        public bool IsTrap() => trap != null;
        public PlayerAvatar GetLastPlayerHeld() => Patches.LastGrabbedPhysObjects?.ToList().FirstOrDefault(p => p.Key == physGrabObject).Value ?? null;
    }

    public static class ObjectExtensions
    {
        public static Dictionary<PhysGrabObject, ObjectHandler> ObjectHandlers = new Dictionary<PhysGrabObject, ObjectHandler>();

        public static ObjectHandler Handle(this PhysGrabObject physGrabObject)
        {
            if (physGrabObject == null)
            {
                if (ObjectHandlers.ContainsKey(physGrabObject)) ObjectHandlers.Remove(physGrabObject);
                return null;
            }
            if (!ObjectHandlers.TryGetValue(physGrabObject, out var handler))
            {
                handler = new ObjectHandler(physGrabObject);
                ObjectHandlers[physGrabObject] = handler;
            }
            return handler;
        }
    }
}
