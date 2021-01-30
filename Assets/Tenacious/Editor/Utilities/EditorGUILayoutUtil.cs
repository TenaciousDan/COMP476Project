using TenaciousEditor.IMGUI;
using UnityEditor;

using UnityEngine;

namespace TenaciousEditor.Utilities
{
    public static class EditorGUILayoutUtil
    {
        public static void DrawLine(bool isVerticalScope = true, float drawWidth = -1, float drawHeight = -1)
        {
            DrawLine(new Color(0, 0, 0, 0.5f), isVerticalScope, drawWidth, drawHeight);
        }
        public static void DrawLine(Color color, bool isVerticalScope = true, float drawWidth = -1, float drawHeight = -1)
        {
            if (isVerticalScope)
                drawHeight = drawHeight < 0 ? 1 : drawHeight;
            else
                drawWidth = drawWidth < 0 ? 1 : drawWidth;

            GUILayoutOption widthLayout = drawWidth < 0 ? GUILayout.ExpandWidth(true) : GUILayout.Width(drawWidth);
            GUILayoutOption heightLayout = drawHeight < 0 ? GUILayout.ExpandHeight(true) : GUILayout.Height(drawHeight);

            Color previousBGColor = GUI.backgroundColor;
            GUI.backgroundColor = color;
            GUILayout.Box(GUIContent.none, TenaciousEditorStyles.LineSeperator, widthLayout, heightLayout);
            GUI.backgroundColor = previousBGColor;
        }
    }
}
