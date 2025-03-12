using REPOssessed.Cheats.Core;
using REPOssessed.Util;

namespace REPOssessed.Cheats
{
    internal class ToggleMenu : ExecutableCheat
    {
        public ToggleMenu() : base() { }

        public override void Execute() 
        {
            Settings.b_isMenuOpen = !Settings.b_isMenuOpen;
            MenuUtil.ToggleCursor();
        }
    }
}
