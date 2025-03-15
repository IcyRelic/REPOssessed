using UnityEngine;
using REPOssessed.Cheats.Components;
using REPOssessed.Cheats.Core;

namespace REPOssessed.Cheats
{
    internal class NoClip : ToggleCheat, IVariableCheat<float>
    {
        private bool OriginalFreezeRotation;
        private bool OriginalUseGravity;
        private bool OriginalIsKinematic;
        private RigidbodyConstraints OriginalRigidbodyConstraints;
        private KBInput movement = null;
        public static float Value = 10f;

        public override void Update()
        {
            if (!Enabled) return;
            Settings.f_inputMovementSpeed = Value;
            PlayerController player = PlayerController.instance;
            if (player == null) return;
            Rigidbody rb = player.rb;
            if (rb == null) return;
            if (movement == null) movement = player.gameObject.AddComponent<KBInput>();
            player.transform.transform.position = movement.transform.position;
            rb.constraints = RigidbodyConstraints.None;
            rb.freezeRotation = false;
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        public override void OnEnable()
        {
            PlayerController player = PlayerController.instance;
            if (player == null) return;
            Rigidbody rb = player.rb;
            if (rb == null) return;
            OriginalRigidbodyConstraints = rb.constraints;
            OriginalFreezeRotation = rb.freezeRotation;
            OriginalUseGravity = rb.useGravity;
            OriginalIsKinematic = rb.isKinematic;
        }

        public override void OnDisable()
        {
            Destroy(movement);
            movement = null;
            PlayerController player = PlayerController.instance;
            if (player == null) return;
            Rigidbody rb = player.rb;
            if (rb == null) return;
            rb.constraints = OriginalRigidbodyConstraints;
            rb.freezeRotation = OriginalFreezeRotation;
            rb.useGravity = OriginalUseGravity;
            rb.isKinematic = OriginalIsKinematic;
        }
    }
}
