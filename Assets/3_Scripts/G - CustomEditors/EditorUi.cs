using System;
using Cinemachine.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations.Rigging;

/// So far it works like this:
/// All variables get set up in the Setup function (needs to be manually called setting SetupDone to false
/// Setup is placed in DrawComponentTitle, since every editor should have this
/// The functions below are used to draw specific things such as headers or images

namespace Dave
{
    public class EditorUi : Editor
    {
        public static EditorUiData data;
        public static int hardcodedBgAlphaValue;

        // indent level
        public static int baseIndentLevel;

        // fonts
        public static Font font_bold;
        public static Font font_light;

        // styles
        public static GUIStyle style_component;
        public static GUIStyle style_header;
        public static GUIStyle style_subHeader;
        public static GUIStyle style_toggle;
        public enum Style
        {
            component,
            header
        }

        // layout
        public static float space_beforeHeader;
        public static float space_afterHeader;
        public static float space_beforeToggle;
        public static float space_afterToggle;

        // colors
        public static Dictionary<CustomColor, string> customColors = new Dictionary<CustomColor, string>();


        public enum CustomColor
        {
            none,

            white, black, gray,
            blue, lightBlue, darkBlue, electricBlue,
            green, lightGreen, mintGreen,
            yellow, orange,
            red, pink,
            purple, lightPurple,
            lavender,

            abilityPurple,
            movementActionOrange, rangedActionOrange, meleeActionOrange,
            statEffectBlue, cameraEffectBlue, animationEffectBlue, soundEffectBlue,

            lightBlueLs,
            orangeLs,
            purpleLs,
            darkGray
        }

        // special symbols
        public static string save = "▸▶▷▹○①②③④●";
        public static string numbers = "①②③④⑤⑥⑦⑧⑨⑩";

        private static bool setupDone = false;

        private static void Setup()
        {
            if (setupDone) return;

            Debug.Log("EditorUi Setup");

            data = Resources.Load<EditorUiData>("EditorUiSettings");

            // indent level
            baseIndentLevel = 1;
            hardcodedBgAlphaValue = 12;

            // fonts
            font_bold = Resources.Load<Font>("UniSansBold");
            font_light = Resources.Load<Font>("UniSansLight");

            // stlyes
            style_component = new GUIStyle (GUI.skin.label)
            {
                alignment = TextAnchor.MiddleLeft,
                margin = new RectOffset(),
                padding = new RectOffset(),
                fontSize = 32,
                font = font_bold,
                richText = true
            };

            style_header = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleLeft,
                margin = new RectOffset(),
                padding = new RectOffset(),
                fontSize = 24,
                font = font_bold,
                richText = true
            };

            style_subHeader = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleLeft,
                margin = new RectOffset(),
                padding = new RectOffset(),
                fontSize = 18,
                font = font_bold,
                richText = true
            };

            style_toggle = new GUIStyle(GUI.skin.toggle)
            {
                alignment = TextAnchor.MiddleLeft,
                margin = new RectOffset(),
                padding = new RectOffset(),
                fontSize = 18,
                font = font_light,
                richText = true
            };

            // layout
            space_beforeHeader = 30;
            space_afterHeader = 10;
            space_beforeToggle = 12;
            space_afterToggle = 7;

            // colors
            customColors.TryAdd(CustomColor.white, "#ffffff");
            customColors.TryAdd(CustomColor.black, "#000000");
            customColors.TryAdd(CustomColor.gray, "#BCC1BA");
            customColors.TryAdd(CustomColor.darkGray, "#414141");
            customColors.TryAdd(CustomColor.green, "#2ddd27");
            customColors.TryAdd(CustomColor.lightGreen, "#0cf574");
            customColors.TryAdd(CustomColor.mintGreen, "#91e0c8");
            customColors.TryAdd(CustomColor.pink, "#ff495c");
            customColors.TryAdd(CustomColor.orange, "#fc440f");
            customColors.TryAdd(CustomColor.blue, "#256eff");
            customColors.TryAdd(CustomColor.lightBlue, "#6efafb");
            customColors.TryAdd(CustomColor.darkBlue, "#0e0f19");
            customColors.TryAdd(CustomColor.electricBlue, "#90f3ff");
            customColors.TryAdd(CustomColor.red, "#bf211e");
            customColors.TryAdd(CustomColor.purple, "#712f79");
            customColors.TryAdd(CustomColor.lightPurple, "#9395D3");
            customColors.TryAdd(CustomColor.lavender, "#D8E4FF");
            customColors.TryAdd(CustomColor.yellow, "#F9D71C");

            /// specific colors
            customColors.TryAdd(CustomColor.abilityPurple, "#792359");
            customColors.TryAdd(CustomColor.movementActionOrange, "#F68E59");
            customColors.TryAdd(CustomColor.rangedActionOrange, "#F26B2B");
            customColors.TryAdd(CustomColor.meleeActionOrange, "#DE5126");
            customColors.TryAdd(CustomColor.cameraEffectBlue, "#A3E4F9");
            customColors.TryAdd(CustomColor.soundEffectBlue, "83CEFB");
            customColors.TryAdd(CustomColor.animationEffectBlue, "#8FDAFA");
            customColors.TryAdd(CustomColor.statEffectBlue, "#7AC1F9");

            /// light text variations of colors
            customColors.TryAdd(CustomColor.orangeLs, "#ffffff");
            customColors.TryAdd(CustomColor.lightBlueLs, "#ffffff");
            customColors.TryAdd(CustomColor.purpleLs, "#ffffff");
            //customColors.TryAdd(CustomColor.orangeLs, "#fff1ed");
            //customColors.TryAdd(CustomColor.lightBlueLs, "#e8fafa");

            setupDone = true;
        }

        public static GUIStyle GetStyle(Style style)
        {
            Debug.Log("EditorUi GetStyle");

            // component
            if(style == Style.component)
                return style_component;

            // header
            else if (style == Style.header)
                return style_header;

            return null;
        }

        public static void DrawComponentTitle(EditorUiData.Component componentType) { DrawComponentTitle(componentType, CustomColor.white); }
        public static void DrawComponentTitle(EditorUiData.Component componentType, CustomColor overrideColor = CustomColor.none, string overrideName = "")
        {
            //setupDone = false;
            //Debug.LogError("Setup Done is false!");
            // I should probably not call setup every single time haha
            Setup();
            EditorGUI.indentLevel = baseIndentLevel;


            ComponentDesignData designData = data.cd_componentDesingLookup[componentType];
            CustomColor realColor = designData.color;

            DrawBgTexture(designData.icon, designData.baseHeigth, designData.bgAlphaValue);

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();

            string rcLabPrefix = "<color=#7d7d7d>RcLab: </color>";
            string colorPrefix = "<color=" + customColors[realColor] + ">";
            string colorSuffix = "</color>";
            string realName = overrideName == "" ? designData.name : overrideName;
            GUILayout.Label(rcLabPrefix + colorPrefix + realName + colorSuffix, GetStyle(Style.component));

            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            DrawHeaderBar();
            DrawHeaderBarExtensionRight();
            DrawIconWithBorder(designData.icon, realColor);

            if (designData.drawSoonAvailableInspectorNote)
                DrawSoonAvailableInspectorNote();

            if (designData.drawSpaceBeforeFirstVariable)
                DrawSpaceBeforeFirstVariable();
        }

        private static void DrawHeaderBar()
        {
            Texture bgBar = data.texture_bgBar;
            EditorGUI.DrawPreviewTexture(new Rect(0, 8, 1000, 10), bgBar, null, ScaleMode.StretchToFill);
            EditorGUI.DrawPreviewTexture(new Rect(0, 58, 1000, 10), bgBar, null, ScaleMode.StretchToFill);
        }

        private static void DrawHeaderBarExtensionRight()
        {
            Texture barExtension = data.texture_headerBarExtension;
            Rect rectBgBar = new Rect(EditorGUIUtility.currentViewWidth - 240, -21, barExtension.width * 1f, barExtension.height * 1f);
            GUI.DrawTexture(rectBgBar, barExtension, ScaleMode.ScaleAndCrop, true, 0, Color.white, 100, 12);
        }

        public static void DrawHeader(string title, int number) { DrawHeader(title, number, Style.header, CustomColor.white); }
        public static void DrawHeader(string title, int number, CustomColor color) { DrawHeader(title, number, Style.header, color); }
        public static void DrawHeader(string title, int number, Style style, CustomColor color)
        {
            GUILayout.Space(space_beforeHeader);

            GUILayout.BeginHorizontal();

            string colorPrefix = "<color=" + customColors[color] + ">";
            string colorSuffix = "</color>";
            string symbol = "<size=22>" + numbers.ToCharArray()[number-1] + "  " +  "</size>";
            GUILayout.Label(symbol + colorPrefix + title + colorSuffix, GetStyle(style));

            GUILayout.EndHorizontal();

            GUILayout.Space(space_afterHeader);
        }

        public static void DrawSubHeader(string title)
        {
            GUILayout.Space(space_beforeToggle);

            GUILayout.BeginHorizontal();

            GUILayout.Space(baseIndentLevel * 14);

            GUILayout.Label(title, style_subHeader);

            GUILayout.EndHorizontal();

            GUILayout.Space(space_afterToggle - 5);
        }

        public static void DrawToggle(ref bool enabled, string title)
        {
            GUILayout.Space(space_beforeToggle);

            bool result = GUILayout.Toggle(enabled, "     " + title, style_toggle);

            GUILayout.Space(space_afterToggle);

            enabled = result;
        }

        public static void DrawIconWithBorder(Texture texture, CustomColor borderColor)
        {
            Rect rect = new Rect(EditorGUIUtility.currentViewWidth - 155, 20, 122, 87);
            GUI.DrawTexture(rect, texture, ScaleMode.ScaleAndCrop, true, 0, Color.white, 100, 12);

            Color color = HexToColor(customColors[borderColor]);
            GUI.DrawTexture(rect, data.texture_white, ScaleMode.ScaleAndCrop, true, 0, color, 1, 12);
        }

        public static void DrawBgTexture(Texture texture, float height, float alphaValue)
        {
            Color oldColor = GUI.color;
            alphaValue = hardcodedBgAlphaValue;
            GUI.color = new Color(1, 1, 1, alphaValue / 100f);

            Rect rect = new Rect(0, 64, EditorGUIUtility.currentViewWidth, height);
            GUI.DrawTexture(rect, texture, ScaleMode.ScaleAndCrop);
            //GUI.DrawTexture(rect, texture, ScaleMode.ScaleToFit, true, 0, UnityEngine.Color.white, 1, 1);

            GUI.color = oldColor;
        }

        public static Color HexToColor(string hex)
        {
            hex = hex.Replace("0x", "");
            hex = hex.Replace("#", "");
            int r = int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            int g = int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            int b = int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            int a = hex.Length == 8 ? int.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber) : 255;
            return new Color32((byte)r, (byte)g, (byte)b, (byte)a);
        }

        private static void DrawSoonAvailableInspectorNote()
        {
            int height = 50;

            GUILayout.Space(height);

            Rect rect = EditorGUILayout.GetControlRect(false, 2);
            rect.height = height;
            rect.width = EditorGUIUtility.currentViewWidth - 50;
            EditorGUI.DrawRect(rect, HexToColor(customColors[CustomColor.darkGray]));

            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.font = font_bold;

            string helloText = "Work in Progress!";
            Vector2 helloSize = labelStyle.CalcSize(new GUIContent(helloText));

            Rect helloRect = new Rect(rect.x + 25, rect.y + 1, helloSize.x + 5, rect.height);
            GUI.Label(helloRect, helloText, labelStyle);

            Rect worldRect = new Rect(helloRect.xMax, rect.y - 2, rect.width - helloRect.width - 10, rect.height);
            GUI.Label(worldRect, "I am currently developing this component." + "\n" + "You can use it, but some bugs may appear.");

            GUILayout.Space(35);
        }

        #region Specifics

        public static void DrawImpactEffectSelection(SerializedProperty impactEffectType, SerializedProperty impactEffectSize)
        {
            /*
            EditorGUILayout.PropertyField(impactEffectType);

            /// load variations if needed
            if (_lastImpactEffectType != script.impactEffect.impactEffectType)
            {
                _impactEffectVariations = LoadEffectVariationSelection(script.impactEffect.impactEffectType);
                if (_impactEffectVariations != null)
                    script.impactEffect.effectVariation = _impactEffectVariations[0];
            }

            /// dropdown menu
            if (_impactEffectVariations != null)
            {
                EditorGUI.BeginChangeCheck();

                _selected = EditorGUILayout.Popup("Effect Variations", _selected, _impactEffectVariations.ToArray());

                if (EditorGUI.EndChangeCheck())
                {
                    Debug.Log("Dropdown selection: " + _impactEffectVariations[_selected]);
                    script.impactEffect.effectVariation = _impactEffectVariations[_selected];
                }

                EditorGUILayout.PropertyField(impactEffectSize);
            }
            */
        }

        public static List<string> LoadEffectVariationSelection(GameAssets.ImpactEffect impactEffectType)
        {
            GameAssets gameAssets = Resources.Load<GameObject>("GameAssets").GetComponent<GameAssets>();

            List<string> variations = new List<string>();

            ImpactEffectPref impactEffectPref = null;

            // find impactEffectPref
            for (int i = 0; i < gameAssets.impactEffectPrefs.Count; i++)
            {
                if (gameAssets.impactEffectPrefs[i].impactEffectType == impactEffectType)
                    impactEffectPref = gameAssets.impactEffectPrefs[i];
            }

            if (impactEffectPref == null) return null;

            // find all variations
            for (int i = 0; i < impactEffectPref.effectObjects.Count; i++)
            {
                variations.Add(impactEffectPref.effectObjects[i].effectVariation);
            }

            Debug.Log("loaded all impact variations");

            return variations;
        }

        #endregion

        #region Spacing and Height

        public static void DrawSpaceBeforeFirstVariable()
        {
            GUILayout.Space(52);
        }

        public static void DrawMonoBehaviourOutroSpace()
        {
            GUILayout.Space(12);
        }

        public static void DrawLargeMonoBehaviourOutroSpace()
        {
            GUILayout.Space(52);
        }

        public static void CalculateTotalHeight()
        {
            // I guess I could use EditorUi.PropertyField instead of the normal function to store how many properties are drawn
            // Then I could also somehow calculate or hardcode the heights of title, headers and subheaders
            // Then save all spaces and calculate everything together
            // -> If there was an easier way that would be awesome though

            /// and what about EditorGUI.GetPropertyHeight()

            ///   Rect groupRect = GUILayoutUtility.GetRect(stayAtAttackPoint, new GUIStyle());
            /// float groupHeight = groupRect.height;
            ///Debug.Log("group Height: " + groupHeight + " singleLine " + EditorGUIUtility.singleLineHeight);
        }

        #endregion
    }
}