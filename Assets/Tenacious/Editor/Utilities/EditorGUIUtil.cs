using UnityEditor;

using UnityEngine;

namespace TenaciousEditor.Utilities
{
    public static class EditorGUIUtil
    {
        public static Vector2 CalcTextSize(string text)
        {
            return GUI.skin.label.CalcSize(new GUIContent(text));
        }
    }
}
