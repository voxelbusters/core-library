using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace VoxelBusters.CoreLibrary.Editor
{
    public static class CustomEditorStyles
    {
        #region Static properties

        public static GUIStyle Heading1 { get; private set; }

        public static GUIStyle Heading2 { get; private set; }

        public static GUIStyle Heading3 { get; private set; }

        public static GUIStyle Normal { get; private set; }

        public static GUIStyle Options { get; private set; }

        public static GUIStyle Button { get; private set; }

        public static GUIStyle Link { get; private set; }

        public static GUIStyle ItemBackground { get; private set; }

        public static Color BorderColor { get; private set; }

        #endregion

        #region Constructors

        static CustomEditorStyles()
        {
            Heading1            = CreateStyle(original: EditorStyles.boldLabel,    fontSize: 18);
            Heading2            = CreateStyle(original: EditorStyles.boldLabel,    fontSize: 16);
            Heading3            = CreateStyle(original: EditorStyles.boldLabel,    fontSize: 14);
            Normal              = CreateStyle(original: EditorStyles.label,        fontSize: 14);
            Options             = CreateStyle(original: EditorStyles.label,        fontSize: 12);
            Button              = CreateStyle(original: "Button",                  fontSize: 14);
            Link                = CreateStyle(original: EditorStyles.linkLabel,    fontSize: 12);
            ItemBackground      = new GUIStyle("AnimItemBackground");
            BorderColor         = new Color(0.15f, 0.15f, 0.15f, 1f);
        }

        #endregion

        #region Static methods

        private static GUIStyle CreateStyle(GUIStyle original, Font font = null,
            int fontSize = -1, bool wordWrap = true,
            bool richText = true)
        {
            var      style  = new GUIStyle(original)
            {
                wordWrap        = wordWrap,
                richText        = richText,
            };
            if (font != null)
            {
                style.font      = font;
            }
            if (fontSize != -1)
            {
                style.fontSize  = fontSize;
            }
            return style;
        }

        #endregion
    }
}