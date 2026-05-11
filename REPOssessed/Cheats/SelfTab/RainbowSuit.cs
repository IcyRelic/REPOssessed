using REPOssessed.Cheats.Core;
using REPOssessed.Handler;
using REPOssessed.Manager;
using System.Collections;
using UnityEngine;

namespace REPOssessed.Cheats.SelfTab
{
    internal class RainbowSuit : ToggleCheat, IVariableCheat<float>
    {
        private Coroutine? rainbowCoroutine;
        public static float Value = 0.1f;

        public override void OnEnable()
        {
            if (rainbowCoroutine == null) rainbowCoroutine = REPOssessed.Instance?.StartCoroutine(RainbowSuitCoroutine());
        }

        public override void OnDisable()
        {
            if (rainbowCoroutine == null) return;
            REPOssessed.Instance?.StopCoroutine(rainbowCoroutine);
            rainbowCoroutine = null;
        }

        private IEnumerator RainbowSuitCoroutine()
        {
            int colorIndex = 0;
            while (true)
            {
                PlayerHandler? localPlayerHandler = GameObjectManager.LocalPlayer?.Handle();
                if (localPlayerHandler != null) 
                {
                    colorIndex = (colorIndex + 1) % MetaManager.instance.colors.Count;
                    localPlayerHandler.SetColor(colorIndex);
                }
                yield return new WaitForSeconds(Value);
            }
        }
    }
}
