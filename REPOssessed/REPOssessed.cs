using HarmonyLib;
using Photon.Pun;
using REPOssessed.Cheats.Core;
using REPOssessed.Handler;
using REPOssessed.Language;
using REPOssessed.Manager;
using REPOssessed.Menu.Core;
using REPOssessed.Menu.Popup;
using REPOssessed.Util;
using System;
using System.Collections;
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
        public int fps;

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
            ThemeUtil.SetTheme();
            LoadCheats();
            DoPatching();
            AlertUsingREPOssessed();
            GameObjectManager.CollectObjects();
            this.StartCoroutine(this.FPSCounter());
        }

        private void DoPatching()
        {
            try
            {
                harmony = new Harmony("REPOssessed");
                Harmony.DEBUG = true;
                harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (Exception e)
            {
                Debug.Log($"Error in DoPatching: {e}");
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
                Debug.Log($"Error in FixedUpdate: {e}");
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
                if (!SemiFunc.IsMainMenu()) cheats.ForEach(cheat => cheat.Update());
            }
            catch (Exception e)
            {
                Debug.LogError($"Error in Update: {e}");
            }
        }

        public void OnGUI()
        {
            try
            {
                if (Event.current.type == EventType.Repaint)
                {
                    string REPOssessedTitle = $"REPOssessed {Settings.s_Version} By Dustin | Menu Toggle: {FirstSetupManagerWindow.GetMenuKeybindName()}";
                    REPOssessedTitle += Settings.b_FPSCounter ? $" | FPS: {fps}" : "";
                    VisualUtil.DrawString(new Vector2(5f, 2f), REPOssessedTitle, Settings.c_primary.GetColor(), false, false, true, 14);
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
                if (!SemiFunc.IsMainMenu()) cheats.ForEach(cheat => { if (cheat.Enabled) cheat.OnGui(); });
                menu.Draw();
            }
            catch (Exception e)
            {
                Debug.Log($"Error in OnGUI: {e}");
            }
        }

        public IEnumerator FPSCounter()
        {
            while (true)
            {
                fps = (int)(1.0f / Time.deltaTime);
                yield return new WaitForSeconds(1f);
            }
        }

        public void AlertUsingREPOssessed() => PlayerAvatar.instance?.GetLocalPlayer()?.photonView.RPC("ChatMessageSendRPC", RpcTarget.All, "", false);
    }
}
