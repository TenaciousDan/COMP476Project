using UnityEditor;

using UnityEngine;

namespace TenaciousEditor.IMGUI
{
    public class InspectorBase : Editor
    {
        public override void OnInspectorGUI()
        {
            SerializedProperty begin = serializedObject.GetIterator();
            if (begin != null)
            {
                SerializedProperty it = begin.Copy();
                if (it.NextVisible(true))
                {
                    do EditorGUILayout.PropertyField(it);
                    while (it.NextVisible(false));
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        protected void DrawHorizontalLine(int height = 1)
        {
            Rect rect = EditorGUILayout.GetControlRect(false, height);
            rect.height = height;
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
        }
    }
}
