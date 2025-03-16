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

        public override void DrawContent(int windowID)
        {
            UI.VerticalSpace(ref scrollPos, () =>
            {
                if (!REPOssessed.Instance.IsIngame || RunManager.instance == null || RunManager.instance.levelCurrent == null || RoundDirector.instance == null || RunManager.instance.levels == null)
                {
                    UI.Label("General.MustBeIngame", Settings.c_error);
                    GUI.DragWindow();
                    return;
                }

                Level level = RunManager.instance.levelCurrent;

                UI.Header("LevelManager.CurrentLevel");
                UI.Label("LevelManager.Level", level.NarrativeName);
                UI.Label("LevelManager.Extractions", RoundDirector.instance.Reflect().GetValue<int>("extractionPoints").ToString());

                UI.Header("LevelManager.ChangeLevel");

                GetLevels().Where(l => l != null).ToList().ForEach(l => UI.Button(l.NarrativeName, () => SetLevel(l), "LevelManager.Visit"));
            });
            GUI.DragWindow();
        }

        private void SetLevel(Level level)
        {
            if (level == null) return;
            RunManager.instance.levelCurrent = level;
            if (GameManager.Multiplayer()) RunManager.instance.Reflect().GetValue<RunManagerPUN>("runManagerPUN").Reflect().GetValue<PhotonView>("photonView").RPC("UpdateLevelRPC", RpcTarget.OthersBuffered, level.name, RunManager.instance.levelsCompleted, false);
            else RunManager.instance.UpdateLevel(level.name, RunManager.instance.levelsCompleted, false);
            if (level == RunManager.instance.levelShop) RunManager.instance.Reflect().SetValue("saveLevel", 1);
            else RunManager.instance.Reflect().SetValue("saveLevel", 0);
            SemiFunc.StatSetSaveLevel(RunManager.instance.Reflect().GetValue<int>("saveLevel"));
            RunManager.instance.RestartScene();
        }

        private List<Level> GetLevels()
        {
            List <Level> levels = new List<Level>();
            levels.AddRange(RunManager.instance.levels);
            levels.Add(RunManager.instance.levelShop);
            levels.Add(RunManager.instance.levelTutorial);
            levels.Add(RunManager.instance.levelRecording);
            levels.Add(RunManager.instance.levelArena);
            return levels;
        }
    }
}
