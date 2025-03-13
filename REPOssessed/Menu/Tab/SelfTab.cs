using REPOssessed.Cheats;
using REPOssessed.Cheats.Core;
using REPOssessed.Handler;
using REPOssessed.Language;
using REPOssessed.Manager;
using REPOssessed.Menu.Core;
using REPOssessed.Util;
using System.Linq;
using UnityEngine;

namespace REPOssessed.Menu.Tab
{
    internal class SelfTab : MenuTab
    {
        public SelfTab() : base("SelfTab.Title") { }
        private Vector2 scrollPos = Vector2.zero;
        private Vector2 scrollPos2 = Vector2.zero;

        public override void Draw()
        {
            SelfContent();
            TeleportContent();
        }

        public void SelfContent()
        {
            UI.VerticalSpace(ref scrollPos, () =>
            {
                UI.Header("SelfTab.Title");
                UI.Checkbox("SelfTab.UnlimitedEnergy", Cheat.Instance<UnlimitedStamina>());
                UI.Checkbox("SelfTab.Godmode", Cheat.Instance<Godmode>());

                UI.Checkbox("SelfTab.SafeGodmode", Cheat.Instance<SafeGodmode>());
                UI.Checkbox("SelfTab.NoTumble", Cheat.Instance<NoTumble>());
                UI.Checkbox("SelfTab.InfiniteJump", Cheat.Instance<InfiniteJump>());
                UI.Checkbox("SelfTab.Invisibility", Cheat.Instance<Invisibility>()); // doesn't work 
                UI.TextboxAction("SelfTab.ChangeColor", ref ColorChanger.Value, 1,
                    new UIButton("General.Set", Cheat.Instance<ColorChanger>().Execute)
                );
                UI.Checkbox("SelfTab.UnlimitedBattery", Cheat.Instance<UnlimitedBattery>());
                UI.Checkbox("SelfTab.RainbowMode", Cheat.Instance<SuitRainbowMode>());
                UI.Checkbox("SelfTab.UseSpoofedName", Cheat.Instance<NameSpoofer>());
                UI.Textbox("SelfTab.SpoofedName", ref NameSpoofer.Value, true, 100);
                UI.Checkbox("SelfTab.NoObjectMoneyLoss", Cheat.Instance<NoObjectMoneyLoss>()); // needs redone
                UI.CheatToggleSlider(Cheat.Instance<NoClip>(), "SelfTab.NoClip", NoClip.Value.ToString("#"), ref NoClip.Value, 1f, 20f);
                UI.CheatToggleSlider(Cheat.Instance<SuperSpeed>(), "SelfTab.SuperSpeed", SuperSpeed.Value.ToString("#"), ref SuperSpeed.Value, 1f, 100f);
            }, GUILayout.Width(HackMenu.Instance.contentWidth * 0.5f - HackMenu.Instance.spaceFromLeft));
        }

        public void TeleportContent()
        {
            UI.VerticalSpace(ref scrollPos2, () =>
            {
                UI.Header("SelfTab.TeleportTitle");
                UI.Button("SelfTab.TeleportToTruck", () => TeleportToTruck());
                ExtractionsTeleportLocations();

            }, GUILayout.Width(HackMenu.Instance.contentWidth * 0.5f - HackMenu.Instance.spaceFromLeft));
        }

        private void TeleportToTruck()
        {
            PlayerAvatar player = PlayerAvatar.instance.GetLocalPlayer();
            if (player == null) return;
            SpawnPoint spawnPoint = Object.FindObjectsOfType<SpawnPoint>().ToList().FirstOrDefault(s => s != null);
            if (spawnPoint == null || spawnPoint.transform == null) return;
            player.Handle().Teleport(spawnPoint.transform.position, spawnPoint.transform.rotation);
        }

        private void ExtractionsTeleportLocations()
        {
            PlayerAvatar player = PlayerAvatar.instance.GetLocalPlayer();
            if (player == null) return;
            int Index = 1;
            GameObjectManager.extractions.Where(e => e != null && e.transform != null && !e.Reflect().GetValue<bool>("isShop") && !e.StateIs(ExtractionPoint.State.Complete)).ToList().ForEach(e =>
            {
                UI.Button($"{"SelfTab.Extraction".Localize()} {Index++}", () => player.Handle().Teleport(e.transform.position, e.transform.rotation));
            });
        }
    }
}
