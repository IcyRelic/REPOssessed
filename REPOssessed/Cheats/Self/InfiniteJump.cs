using REPOssessed.Cheats.Core;
using REPOssessed.Util;

namespace REPOssessed.Cheats
{
    internal class InfiniteJump : ToggleCheat
    {
        public override void Update()
        {
            if (!Enabled) return;
            PlayerController.instance.Reflect().SetValue("JumpGroundedBuffer", 1f);
        }

        public override void OnDisable() 
        {
            PlayerController.instance.Reflect().SetValue("JumpGroundedBuffer", 0f);
        }
    }
}
