using HarmonyLib;
using Photon.Pun;
using REPOssessed.Cheats;
using REPOssessed.Cheats.Core;
using REPOssessed.Handler;
using REPOssessed.Language;
using REPOssessed.Manager;
using REPOssessed.Menu.Core;
using REPOssessed.Menu.Popup;
using REPOssessed.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace REPOssessed
{
    public class REPOssessed : MonoBehaviour
    {
        private List<ToggleCheat> cheats;
        private Harmony harmony;
        private HackMenu menu;

        public bool IsIngame => !SemiFunc.IsMainMenu() && !SemiFunc.RunIsLobby() && !SemiFunc.RunIsLobbyMenu();

        private static REPOssessed instance;
        public static REPOssessed Instance
        {
            get
            {
                if (instance == null) instance = new REPOssessed();
                return instance;
            }
        }

        public void Start()
        {
            instance = this;
            LanguageUtil.Initialize();
            ThemeUtil.Initialize();
            LoadCheats();
            DoPatching();
            AlertUsingREPOssessed();
            GameObjectManager.CollectObjects();
        }

        private void DoPatching()
        {
            try
            {
                harmony = new Harmony("REPOssessed");
                Harmony.DEBUG = false;
                harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (Exception e)
            {
                Settings.s_DebugMessage = "Msg: " + e.Message + "\nSrc: " + e.Source + "\n" + e.StackTrace;
            }
        }

        private void LoadCheats()
        {
            cheats = new List<ToggleCheat>();
            menu = new HackMenu();
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes().Where(t => String.Equals(t.Namespace, "REPOssessed.Cheats", StringComparison.Ordinal) && t.IsSubclassOf(typeof(Cheat))))
            {
                if (type.IsSubclassOf(typeof(ToggleCheat))) cheats.Add((ToggleCheat)Activator.CreateInstance(type));
                else Activator.CreateInstance(type);
                Debug.Log($"Loaded Cheat: {type.Name}");
            }
            Settings.Config.SaveDefaultConfig();
            Settings.Config.LoadConfig();
        }

        public void FixedUpdate()
        {
            try
            {
                cheats.ForEach(cheat => cheat.FixedUpdate());
            }
            catch (Exception e)
            {
                Settings.s_DebugMessage = "Msg: " + e.Message + "\nSrc: " + e.Source + "\n" + e.StackTrace;
            }
        }

        public void Update()
        {
            try
            {
                if (Cheat.instances.Where(c => c.WaitingForKeybind).Count() == 0)
                {
                    Cheat.instances.FindAll(c => c.HasKeybind && Input.GetKeyDown(c.keybind)).ForEach(c =>
                    {
                        if (c.GetType().IsSubclassOf(typeof(ToggleCheat))) ((ToggleCheat)c).Toggle();
                        else if (c.GetType().IsSubclassOf(typeof(ExecutableCheat))) ((ExecutableCheat)c).Execute();
                        else Debug.LogError($"REPOssessed Cheat Type: {c.GetType().Name}");
                    });
                }
                if (IsIngame) cheats.ForEach(cheat => cheat.Update());
            }
            catch (Exception e)
            {
                Settings.s_DebugMessage = "Msg: " + e.Message + "\nSrc: " + e.Source + "\n" + e.StackTrace;
            }
        }

        public void OnGUI()
        {
            try
            {
                if (Event.current.type == EventType.Repaint)
                {
                    VisualUtil.DrawString(new Vector2(5f, 2f), $"REPOssessed {Settings.s_Version} By Dustin | Menu Toggle: {FirstSetupManagerWindow.GetMenuKeybindName()}{(Cheat.Instance<FPSCounter>().Enabled ? $" | FPS: {Cheat.Instance<FPSCounter>().FPS}" : "")}", Settings.c_primary.GetColor(), false, false, true, 14);
                    if (MenuUtil.resizing)
                    {
                        VisualUtil.DrawString(new Vector2(Screen.width / 2, 35f), new string[] {"SettingsTab.ResizeTitle", "SettingsTab.ResizeConfirm"}.Localize(), Settings.c_primary, true, true, true, 22);
                        MenuUtil.ResizeMenu();
                    }
                    if (Settings.b_DebugMode)
                    {
                        VisualUtil.DrawString(new Vector2(5f, 20f), "[DEBUG MODE]", new RGBAColor(50, 205, 50, 1f), false, false, true, 10);
                        VisualUtil.DrawString(new Vector2(10f, 65f), new RGBAColor(255, 195, 0, 1f).AsString(Settings.s_DebugMessage), false, false, false, 22);
                    }
                }
                if (IsIngame) cheats.ForEach(cheat => { if (cheat.Enabled) cheat.OnGui(); });
                menu.Draw();
            }
            catch (Exception e)
            {
                Settings.s_DebugMessage = "Msg: " + e.Message + "\nSrc: " + e.Source + "\n" + e.StackTrace;
            }
        }

        public void AlertUsingREPOssessed() => PlayerAvatar.instance?.GetLocalPlayer()?.photonView.RPC("ChatMessageSendRPC", RpcTarget.All, "", false);
    }
}
