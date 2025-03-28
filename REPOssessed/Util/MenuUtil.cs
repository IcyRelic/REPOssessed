﻿using UnityEngine;
using UnityEngine.InputSystem;
using REPOssessed.Menu.Core;

namespace REPOssessed.Util
{
    internal class MenuUtil 
    {
        public static bool showCursor = false;
        public static bool resizing = false;
        public static float MouseX => Input.mousePosition.x;
        public static float MouseY => Screen.height - Input.mousePosition.y;
        public static float maxWidth = Screen.width - (Screen.width * 0.1f);
        public static float maxHeight = Screen.height - (Screen.height * 0.1f);
        private static int oldWidth, oldHeight;

        public static void BeginResizeMenu()
        {
            if (resizing) return;
            WarpCursor();
            resizing = true;
            oldWidth = Settings.i_menuWidth;
            oldHeight = Settings.i_menuHeight;
        }

        public static void WarpCursor()
        {
            float currentX = HackMenu.Instance.windowRect.x + HackMenu.Instance.windowRect.width;
            float currentY = Screen.height - (HackMenu.Instance.windowRect.y + HackMenu.Instance.windowRect.height);
            Mouse.current.WarpCursorPosition(new Vector2(currentX, currentY));
        }

        public static void ResizeMenu()
        {
            if (!resizing) return;

            if (Input.GetMouseButtonDown(0))
            {
                resizing = false;
                Settings.Config.SaveConfig();
                return;
            }

            if (Input.GetMouseButtonDown(1))
            {
                resizing = false;
                Settings.i_menuWidth = oldWidth;
                Settings.i_menuHeight = oldHeight;
                Settings.Config.SaveConfig();
                return;
            }

            float currentX = HackMenu.Instance.windowRect.x + HackMenu.Instance.windowRect.width;
            float currentY = HackMenu.Instance.windowRect.y + HackMenu.Instance.windowRect.height;

            Settings.i_menuWidth = (int)Mathf.Clamp(MouseX - HackMenu.Instance.windowRect.x, 500, maxWidth);
            Settings.i_menuHeight = (int)Mathf.Clamp(MouseY - HackMenu.Instance.windowRect.y, 250, maxHeight);
            HackMenu.Instance.Resize();
        }

        public static void ShowCursor()
        {
            if (PlayerController.instance != null) PlayerController.instance.cameraAim.enabled = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        public static void HideCursor()
        {
            if (PlayerController.instance != null) PlayerController.instance.cameraAim.enabled = true;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        public static void ToggleCursor()
        {
            if (!Cursor.visible) ShowCursor();
            else HideCursor();
        }
    }
}
