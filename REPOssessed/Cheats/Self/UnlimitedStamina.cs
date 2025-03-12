using REPOssessed.Cheats.Core;

namespace REPOssessed.Cheats
{
    internal class UnlimitedStamina : ToggleCheat
    {
        public override void Update()
        {
            if (!Enabled) return;
            PlayerController.instance.DebugEnergy = true;
        }

        public override void OnDisable()
        {
            PlayerController.instance.DebugEnergy = false;
        }
    }
}
