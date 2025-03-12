using REPOssessed.Cheats.Core;

namespace REPOssessed.Cheats
{
    internal class SuperSpeed : ToggleCheat, IVariableCheat<float>
    {
        public static float Value = .5f;

        public override void Update()
        {
            if (!Enabled) return;
            PlayerController.instance.MoveSpeed = Value / 5;
        }

        public override void OnDisable()
        {
            PlayerController.instance.MoveSpeed = .5f;
        }
    }
}
