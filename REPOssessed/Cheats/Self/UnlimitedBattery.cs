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
            if (player == null) return;
            ItemEquippable itemEquippable = player.Handle().GetHeldItemEquippable();
            if (itemEquippable == null) return;
            ItemBattery itemBattery = itemEquippable.GetComponent<ItemBattery>();
            if (itemBattery == null) return;
            itemBattery.SetBatteryLife(100);
        }
    }
}
