using Photon.Pun;
using REPOssessed.Menu.Core;
using REPOssessed.Util;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace REPOssessed.Menu.Popup
{
    internal class LevelManagerWindow : PopupMenu
    {
        public LevelManagerWindow(int id) : base("LevelManager.Title", new Rect(50f, 50f, 350f, 250f), id) { }

        private Vector2 scrollPos = Vector2.zero;
        public List<Level>? levels;

        public override void DrawContent(int windowID)
        {
            UI.VerticalGroup(ref scrollPos, () =>
            {
                if (!GameUtil.IsInGame() || RunManager.instance == null)
                {
                    UI.Label("General.MustBeIngame", null, false, -1, false, Settings.c_error);
                    GUI.DragWindow();
                    return;
                }
                if (!GameUtil.IsMasterClient())
                {
                    UI.Label("General.HostRequired", null, false, -1, false, Settings.c_error);
                    GUI.DragWindow();
                    return;
                }

                Level currentLevel = RunManager.instance.levelCurrent;

                UI.Label("LevelManager.CurrentLevel", null, true);
                UI.Label("LevelManager.Level", currentLevel.NarrativeName);
                UI.Label("LevelManager.Extractions", RoundDirector.instance.Reflect().GetValue<int>("extractionPoints").ToString());

                UI.Label("LevelManager.ChangeLevel", null, true);

                if (levels == null) levels = GameUtil.GetLevels();
                levels.Where(l => l != null && l != currentLevel).ToList().ForEach(l => UI.Button(l.NarrativeName, () =>
                {
                    Level newLevel = l;
                    if (newLevel == null || RunManager.instance == null) return;
                    RunManager.instance.levelCurrent = newLevel;
                    if (GameManager.Multiplayer()) RunManager.instance.Reflect()?.GetValue<RunManagerPUN>("runManagerPUN").Reflect().GetValue<PhotonView>("photonView")?.RPC("UpdateLevelRPC", RpcTarget.OthersBuffered, newLevel.name, RunManager.instance.levelsCompleted, false);
                    else RunManager.instance.UpdateLevel(newLevel.name, RunManager.instance.levelsCompleted, false);
                    if (RunManager.instance.levelShop.Any(ls => newLevel)) RunManager.instance.Reflect().SetValue("saveLevel", 1);
                    else RunManager.instance.Reflect().SetValue("saveLevel", 0);
                    SemiFunc.StatSetSaveLevel(RunManager.instance.Reflect().GetValue<int>("saveLevel"));
                    RunManager.instance.RestartScene();
                }, "LevelManager.Visit"));
            });
            GUI.DragWindow();
        }
    }
}
