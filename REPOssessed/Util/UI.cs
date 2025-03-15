using REPOssessed.Cheats;
using REPOssessed.Cheats.Core;
using REPOssessed.Extensions;
using REPOssessed.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace REPOssessed.Util
{
    public class UIButton
    {
        public string label;
        public Action action;
        private GUIStyle style = null;

        public UIButton(string label, Action action, GUIStyle style = null)
        {
            this.label = label.Localize();
            this.action = action;
            this.style = style;
        }

        public void Draw()
        {
            if (style != null ? GUILayout.Button(label, style) : GUILayout.Button(label)) action.Invoke();
        }
    }

    public class UIOption
    {
        public string label;
        public object value;
        public Action action;


        public UIOption(string label, object value)
        {
            this.label = label.Localize();
            this.value = value;
        }

        public UIOption(string label, Action action)
        {
            this.label = label.Localize();
            this.action = action;
        }

        public void Draw(ref object refValue)
        {
            if (GUILayout.Button(label.Localize())) refValue = value;
        }

        public void Draw()
        {
            if (GUILayout.Button(label.Localize())) action.Invoke();
        }
    }
    public class UI
    {
        public static void SetColor(ref RGBAColor color, string hexCode)
        {
            while (hexCode.Length < 6) hexCode += "0";
            color = new RGBAColor(hexCode);
            Settings.Config.SaveConfig();
        }
        public static void Image(Rect rect, Sprite image)
        {
            GUIUtility.RotateAroundPivot(180, image.pivot);
            GUI.DrawTexture(rect, image.texture);
            GUIUtility.RotateAroundPivot(180, image.pivot);
        }

        public static void Image(Rect rect, Texture image)
        {
            GUI.DrawTexture(rect, image);
        }

        public static void Header(string header, bool space = false)
        {
            if (space) GUILayout.Space(20);
            GUILayout.Label(header.Localize(), new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold });
        }

        public static void Header(string header, int size, bool space = false)
        {
            if (space) GUILayout.Space(20);
            GUILayout.Label(header.Localize(), new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = size });
        }

        public static void SubHeader(string header, bool space = false)
        {
            if (space) GUILayout.Space(20);
            GUILayout.Label(header.Localize(), new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
        }

        public static void Label(string header, string label, RGBAColor color = null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(header.Localize());
            GUILayout.FlexibleSpace();
            GUILayout.Label(color is null ? label.Localize() : color.AsString(label.Localize()));
            GUILayout.EndHorizontal();
        }

        public static void Label(string label, RGBAColor color = null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(color is null ? label.Localize() : color.AsString(label.Localize()));
            GUILayout.EndHorizontal();
        }

        public static void Checkbox(string header, ref bool value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(header.Localize());
            GUILayout.FlexibleSpace();
            value = GUILayout.Toggle(value, "");
            GUILayout.EndHorizontal();
        }

        public static void Checkbox(string[] header, ref bool value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(header.Localize());
            GUILayout.FlexibleSpace();
            value = GUILayout.Toggle(value, "");
            GUILayout.EndHorizontal();
        }

        public static void Checkbox(string header, ToggleCheat cheat)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(header.Localize());
            GUILayout.FlexibleSpace();
            cheat.Enabled = GUILayout.Toggle(cheat.Enabled, "");
            GUILayout.EndHorizontal();
        }

        public static void Checkbox(string[] header, ToggleCheat cheat)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(header.Localize());
            GUILayout.FlexibleSpace();
            cheat.Enabled = GUILayout.Toggle(cheat.Enabled, "");
            GUILayout.EndHorizontal();
        }

        public static void Dropdown(string label, ref bool drop, params UIButton[] buttons)
        {
            if (!drop)
                if (GUILayout.Button("< " + label.Localize(), new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter })) drop = true;
            if (drop)
            {
                if (GUILayout.Button("^ " + label.Localize(), new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter })) drop = false;
                buttons.ToList().ForEach(b => b.Draw());
            }
        }

        public static void ToggleSlider(string header, string displayValue, ref bool enable, ref float value, float min, float max, params object[] param)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(header.Localize() + " ( " + displayValue + " )");
            GUILayout.FlexibleSpace();

            GUIStyle slider = new GUIStyle(GUI.skin.horizontalSlider) { alignment = TextAnchor.MiddleCenter, fixedWidth = Settings.i_sliderWidth };

            value = GUILayout.HorizontalSlider(value, min, max, slider, GUI.skin.horizontalSliderThumb);

            enable = GUILayout.Toggle(enable, "");

            GUILayout.EndHorizontal();
        }

        public static void Toggle(string header, ref bool value, string enabled = "General.Enable", string disabled = "General.Disable", params Action<bool>[] action)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(header.Localize());
            GUILayout.FlexibleSpace();
            bool newValue = !value;
            if (GUILayout.Button(value ? disabled.Localize() : enabled.Localize()))
            {
                value = newValue;
                action.ToList().ForEach(a => a.Invoke(newValue));
            }
            GUILayout.EndHorizontal();
        }

        public static void CheatToggleSlider(ToggleCheat toggle, string header, string displayValue, ref float value, float min, float max, params object[] param)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(header.Localize() + " ( " + displayValue + " )");
            GUILayout.FlexibleSpace();

            GUIStyle slider = new GUIStyle(GUI.skin.horizontalSlider) { alignment = TextAnchor.MiddleCenter, fixedWidth = Settings.i_sliderWidth };

            value = GUILayout.HorizontalSlider(value, min, max, slider, GUI.skin.horizontalSliderThumb);

            toggle.Enabled = GUILayout.Toggle(toggle.Enabled, "");

            GUILayout.EndHorizontal();
        }

        public static void ExecuteSlider(string header, string displayValue, Action executable, ref float value, float min, float max, string execute = "General.execute", params object[] param)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(header.Localize() + " ( " + displayValue + " )");
            GUILayout.FlexibleSpace();
            GUIStyle slider = new GUIStyle(GUI.skin.horizontalSlider) { alignment = TextAnchor.MiddleCenter, fixedWidth = Settings.i_sliderWidth };
            value = GUILayout.HorizontalSlider(value, min, max, slider, GUI.skin.horizontalSliderThumb);
            if (GUILayout.Button(execute.Localize())) executable.Invoke();
            GUILayout.EndHorizontal();
        }

        public static void Button(string header, Action action, string btnText = "General.Execute")
        {
            if (!String.IsNullOrEmpty(btnText.Localize()))
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(header.Localize());
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(btnText.Localize())) action();
                GUILayout.EndHorizontal();
            }
            else if (GUILayout.Button(header.Localize())) action();
        }

        public static void Button(string[] header, Action action, string btnText = "General.Execute")
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(header.Localize());
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(btnText.Localize())) action();
            GUILayout.EndHorizontal();
        }

        public static void Slider(string header, string displayValue, ref float value, float min, float max)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(header.Localize() + " ( " + displayValue + " )");
            GUILayout.FlexibleSpace();
            value = GUILayout.HorizontalSlider(value, min, max, GUILayout.Width(Settings.i_sliderWidth));
            GUILayout.EndHorizontal();
        }
        public static void NumSelect(string header, ref int value, int min, int max)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(header.Localize());
            GUILayout.FlexibleSpace();
            GUILayout.Label(value.ToString());
            if (GUILayout.Button("-")) value = Mathf.Clamp(value - 1, min, max);
            if (GUILayout.Button("+")) value = Mathf.Clamp(value + 1, min, max);
            GUILayout.EndHorizontal();
        }

        public static void Select(string header, ref int index, params UIOption[] options)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(header.Localize());
            GUILayout.FlexibleSpace();

            options[index].Draw();

            if (GUILayout.Button("-")) index = Mathf.Clamp(index - 1, 0, options.Length - 1);
            if (GUILayout.Button("+")) index = Mathf.Clamp(index + 1, 0, options.Length - 1);


            GUILayout.EndHorizontal();
        }

        public static void Textbox<T>(string label, ref T value, bool big = true, int length = -1, params Action<T>[] onChanged) where T : struct, IConvertible, IComparable<T>
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label.Localize());
            GUILayout.FlexibleSpace();
            if (GUILayout.TextField(value.ToString(), length, GUILayout.Width(big ? Settings.i_textboxWidth * 3 : Settings.i_textboxWidth)).Parse<T>(out T result))
            {
                if (!value.Equals(result)) onChanged.ToList().ForEach(action => action.Invoke(result));
                value = result;
            }
            GUILayout.EndHorizontal();
        }

        public static void Textbox(string label, ref string value, bool big = true, int length = -1, params Action<string>[] onChanged)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label.Localize());
            GUILayout.FlexibleSpace();
            string s = GUILayout.TextField(value, length, GUILayout.Width(big ? Settings.i_textboxWidth * 3 : Settings.i_textboxWidth));
            if (s != value) onChanged.ToList().ForEach(action => action.Invoke(s));
            value = s;
            GUILayout.EndHorizontal();
        }

        public static void Textbox(string label, ref string value, string regex = "", int size = 3, bool big = true)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label.Localize());
            GUILayout.FlexibleSpace();
            value = GUILayout.TextField(value, GUILayout.Width(big ? Settings.i_textboxWidth * size : Settings.i_textboxWidth));
            value = Regex.Replace(value, regex, "");
            GUILayout.EndHorizontal();
        }

        public static void TextboxAction<T>(string label, ref T value, int length = -1, params UIButton[] buttons) where T : struct, IConvertible, IComparable<T>
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label.Localize());
            GUILayout.FlexibleSpace();
            if (GUILayout.TextField(value.ToString(), length, GUILayout.Width(Settings.i_textboxWidth)).Parse<T>(out T result))
                value = result;
            buttons.ToList().ForEach(btn => btn.Draw());
            GUILayout.EndHorizontal();
        }

        public static void TextboxAction(string label, ref string value, int length = 1, params UIButton[] buttons)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label.Localize());
            GUILayout.FlexibleSpace();
            value = GUILayout.TextField(value, length, GUILayout.Width(Settings.i_textboxWidth));
            buttons.ToList().ForEach(btn => btn.Draw());
            GUILayout.EndHorizontal();
        }

        public static void Actions(params UIButton[] buttons)
        {
            GUILayout.BeginHorizontal();
            buttons.ToList().ForEach(btn => btn.Draw());
            GUILayout.EndHorizontal();
        }
        public static void HorizontalSpace(string title, Action action)
        {
            if (title is not null) Header(title);
            GUILayout.BeginHorizontal();
            action.Invoke();
            GUILayout.EndHorizontal();
        }

        public static void VerticalSpace(ref Vector2 ScrollPosition, Action action, params GUILayoutOption[] options)
        {
            GUILayout.BeginVertical(options);
            ScrollPosition = GUILayout.BeginScrollView(ScrollPosition);
            action.Invoke();
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        public static void ButtonGrid<T>(List<T> objects, Func<T, string> textSelector, string search, Action<T> action, int numPerRow, int btnWidth = 175)
        {
            List<T> filtered = objects.FindAll(x => textSelector(x).ToLower().Contains(search.ToLower()));

            int rows = Mathf.CeilToInt(filtered.Count / (float)numPerRow);

            for (int i = 0; i < rows; i++)
            {
                GUILayout.BeginHorizontal();
                for (int j = 0; j < numPerRow; j++)
                {
                    int index = i * numPerRow + j;
                    if (index >= filtered.Count) break;
                    var obj = filtered[index];

                    if (GUILayout.Button(textSelector((T)obj), GUILayout.Width(btnWidth))) action((T)obj);
                }
                GUILayout.EndHorizontal();
            }
        }
    }
}
