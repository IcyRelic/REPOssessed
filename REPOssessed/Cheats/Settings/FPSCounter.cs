using REPOssessed.Cheats.Core;
using System.Collections;
using UnityEngine;

namespace REPOssessed.Cheats
{
    internal class FPSCounter : ToggleCheat
    {
        public int FPS = 0;

        public override void OnEnable()
        {
            REPOssessed.Instance.StartCoroutine(StartFPSCounter());
        }

        public override void OnDisable()
        {
            REPOssessed.Instance.StopCoroutine(StartFPSCounter());
        }

        public IEnumerator StartFPSCounter()
        {
            while (true)
            {
                FPS = (int)(1.0f / Time.deltaTime);
                yield return new WaitForSeconds(1f);
            }
        }
    }
}
