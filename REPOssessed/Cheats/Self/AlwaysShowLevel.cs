using REPOssessed.Cheats.Core;
using REPOssessed.Util;
using UnityEngine;

namespace REPOssessed.Cheats
{
    internal class AlwaysShowLevel : ToggleCheat
    {
        public override void OnGui()
        {
            if (!Enabled) return;
            string levelsCompleted = $"LEVEL {RunManager.instance.levelsCompleted + 1}";
            string levelName = LevelGenerator.Instance.Level.NarrativeName.ToUpper();
            float levelsCompletedSize = GUI.skin.label.CalcSize(new GUIContent(levelsCompleted)).x;
            float levelNameSize = GUI.skin.label.CalcSize(new GUIContent(levelName)).x;
            Vector2 levelsCompletedPosition = new Vector2((Screen.width - levelsCompletedSize) / 2, 10);
            Vector2 levelNamePosition = new Vector2((Screen.width - levelNameSize) / 2, levelsCompletedPosition.y + GUI.skin.label.CalcSize(new GUIContent(levelsCompleted)).y + 5);
            VisualUtil.DrawString(levelsCompletedPosition, levelsCompleted, Settings.c_primary, false, false, false, Settings.i_menuFontSize);
            VisualUtil.DrawString(levelNamePosition, levelName, Settings.c_primary, false, false, false, Settings.i_menuFontSize);
        }
    }
}
