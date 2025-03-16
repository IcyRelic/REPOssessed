using REPOssessed.Cheats.Core;
using REPOssessed.Handler;
using System.Collections;
using UnityEngine;

namespace REPOssessed.Cheats
{
    internal class RainbowSuit : ToggleCheat
    {
        private Coroutine rainbowSuitCoroutine;

        public override void OnEnable()
        {
            if (PlayerAvatar.instance.GetLocalPlayer() != null && rainbowSuitCoroutine == null) rainbowSuitCoroutine = REPOssessed.Instance.StartCoroutine(RainbowSuitStart());
        }

        public override void OnDisable()
        {
            if (rainbowSuitCoroutine == null) return;
            REPOssessed.Instance.StopCoroutine(rainbowSuitCoroutine);
            rainbowSuitCoroutine = null;
        }

        private IEnumerator RainbowSuitStart()
        {
            int colors = AssetManager.instance.playerColors.Count;
            int index = 0;
            while (true) 
            {
                if (PlayerAvatar.instance.GetLocalPlayer() == null) yield break;
                PlayerAvatar.instance.GetLocalPlayer().PlayerAvatarSetColor(index);
                index = (index + 1) % colors;
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
