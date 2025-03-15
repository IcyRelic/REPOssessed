using REPOssessed.Cheats.Core;
using REPOssessed.Handler;
using System.Collections;
using UnityEngine;

namespace REPOssessed.Cheats
{
    internal class RainbowSuit : ToggleCheat
    {
        public override void OnEnable()
        {
            REPOssessed.Instance.StartCoroutine(RainbowSuitStart());
        }

        public override void OnDisable()
        {
            REPOssessed.Instance.StopCoroutine(RainbowSuitStart());
        }

        private IEnumerator RainbowSuitStart()
        {
            int colors = AssetManager.instance.playerColors.Count;
            int index = 0;
            while (true)
            {
                if (PlayerAvatar.instance.GetLocalPlayer() == null) yield return new WaitForSeconds(1f);
                PlayerAvatar.instance.GetLocalPlayer().PlayerAvatarSetColor(index);
                index = (index + 1) % colors;
                yield return new WaitForSeconds(0.1f); 
            }
        }
    }
}
