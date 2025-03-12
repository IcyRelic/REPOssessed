using Photon.Pun;
using REPOssessed.Util;
using System.Linq;
using UnityEngine;

namespace REPOssessed.Handler
{
    public class ObjectHandler
    {
        private PhysGrabObject physGrabObject;

        public ObjectHandler(PhysGrabObject physGrabObject)
        {
            this.physGrabObject = physGrabObject;
        }

        public string GetName()
        {
            if (IsShopItem()) return GetItemAttributes().item.itemName;
            return physGrabObject.name.Replace("(Clone)", "").Replace("Valuable", "").Trim();
        }
        public void Teleport(Vector3 position, Quaternion rotation) => physGrabObject.Teleport(position, rotation);
        public PhotonTransformView GetPhotonTransformView() => physGrabObject.GetComponent<PhotonTransformView>();

        public bool IsShopItem() => GetItemAttributes() != null;
        public ItemAttributes GetItemAttributes() => physGrabObject ? physGrabObject.GetComponent<ItemAttributes>() : null;
        public ValuableObject GetValuableObject() => physGrabObject ? physGrabObject.GetComponent<ValuableObject>() : null;
        public float GetValue() => GetValuableObject() != null ? GetValuableObject().dollarValueCurrent : 0f;
        public bool IsPlayer() => physGrabObject.Reflect().GetValue<bool>("isPlayer");
        public bool IsEnemy() => physGrabObject.Reflect().GetValue<bool>("isEnemy");
        public bool IsValuable() => physGrabObject.Reflect().GetValue<bool>("isValuable");
        public bool IsNonValuable() => physGrabObject.Reflect().GetValue<bool>("isNonValuable");
        public bool IsHinge() => physGrabObject.Reflect().GetValue<bool>("hasHinge");
        public bool IsCart() => physGrabObject.Reflect().GetValue<bool>("isCart");
        public bool IsTrap() => physGrabObject.GetComponent<Trap>();
    }

    public static class ObjectExtensions
    {
        public static ObjectHandler? Handle(this PhysGrabObject physGrabObject) => new ObjectHandler(physGrabObject);
    }
}
