using REPOssessed.Cheats.Core;
using REPOssessed.Handler;

namespace REPOssessed.Cheats
{
    internal class UnlimitedBattery : ToggleCheat
    {
        public override void Update()
        {
            if (!Enabled) return;
            PlayerAvatar player = PlayerAvatar.instance.GetLocalPlayer();
            if (player == null || player.Handle() == null || !player.Handle().IsMasterClient()) return;
            ItemEquippable itemEquippable = player.Handle().physGrabObject?.GetComponent<ItemEquippable>() ?? null;
            if (itemEquippable == null) return;
            ItemBattery itemBattery = itemEquippable?.GetComponent<ItemBattery>() ?? null;
            if (itemBattery == null) return;
            itemBattery.SetBatteryLife(100);
        }
    }
}
