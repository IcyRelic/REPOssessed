using REPOssessed.Cheats.Core;

namespace REPOssessed.Cheats
{
    internal class NoTumble : ToggleCheat
    {
        public override void OnEnable()
        {
            PlayerController.instance.DebugNoTumble = true;
        }
        public override void OnDisable()
        {
            PlayerController.instance.DebugNoTumble = false;
        }
    }
}
