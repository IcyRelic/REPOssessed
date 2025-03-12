using REPOssessed.Cheats.Core;
using REPOssessed.Handler;
using System.Collections;
using UnityEngine;

namespace REPOssessed.Cheats
{
    internal class SuitRainbowMode : ToggleCheat
    {
        public override void OnEnable()
        {
            REPOssessed.Instance.StartCoroutine(RainbowSuit());
        }

        public override void OnDisable()
        {
            REPOssessed.Instance.StopCoroutine(RainbowSuit());
        }

        private IEnumerator RainbowSuit()
        {
            int colors = AssetManager.instance.playerColors.Count;
            int index = 0;
            while (true)
            {
                if (PlayerAvatar.instance.GetLocalPlayer() != null) PlayerAvatar.instance.GetLocalPlayer().PlayerAvatarSetColor(index);
                index = (index + 1) % colors;
                yield return new WaitForSeconds(0.1f); 
            }
        }
    }
}
