using REPOssessed.Language;

namespace REPOssessed.Menu.Core
{
    internal class MenuTab : MenuFragment
    {

        public string name;
        
        public MenuTab(string name)
        {
            this.name = name.Localize();
        }

        public virtual void Draw() { }
    }
}
