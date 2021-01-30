using UnityEditor;

using UnityEngine;

namespace TenaciousEditor.IMGUI
{
    public static class TenaciousEditorStyles
    {
        public static GUIStyle Title
        {
            get => new GUIStyle(GUI.skin.label)
            {
                fontStyle = FontStyle.Bold,
                fontSize = 42,
                alignment = TextAnchor.MiddleCenter,
                richText = true
            };
        }

        public static GUIStyle ToolbarButton
        {
            get => new GUIStyle(EditorStyles.toolbarButton)
            {
                richText = true,
                fixedHeight = 0,
                border = new RectOffset(1, 1, 1, 1)
            };
        }

        public static GUIStyle LineSeperator
        {
            get => new GUIStyle(GUI.skin.box)
            {
                margin = new RectOffset(0, 0, 0, 0)
            };
        }
    }
}
