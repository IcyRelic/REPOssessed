using REPOssessed.Cheats.Core;
using REPOssessed.Util;
using UnityEngine;

namespace REPOssessed.Cheats
{
    internal class ToggleMenu : ExecutableCheat
    {
        public ToggleMenu() : base(KeyCode.Insert) { }

        public override void Execute() 
        {
            Settings.b_isMenuOpen = !Settings.b_isMenuOpen;
            MenuUtil.ToggleCursor();
        }
    }
}
