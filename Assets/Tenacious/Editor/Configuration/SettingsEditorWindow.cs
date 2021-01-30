using TenaciousEditor.IMGUI;
using TenaciousEditor.Utilities;
using UnityEditor;

using UnityEngine;

namespace TenaciousEditor.Configuration
{
    public class SettingsEditorWindow : EditorWindowBase
    {
        private int selection;
        private readonly string[] selectionLabels;
        private float sidebarWidth;
        private Vector2 sidebarScrollPos;

        public SettingsEditorWindow()
        {
            selectionLabels = new string[] { "Editor" };
            sidebarWidth = 150f;
        }

        //[MenuItem("Tenacious/Settings", false, 0)]
        public static void MenuItemClick()
        {
            EditorWindow.GetWindow<SettingsEditorWindow>("Settings").Show();
        }

        public override void OnGUI()
        {
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();

            sidebarScrollPos = EditorGUILayout.BeginScrollView(sidebarScrollPos);

            for (int i = 0; i < selectionLabels.Length; i++)
            {
                if (GUILayout.Toggle(i == selection, selectionLabels[i], EditorStyles.toolbarButton, GUILayout.Width(sidebarWidth)))
                    selection = i;
            }

            EditorGUILayout.EndScrollView();

            GUILayout.EndVertical();

            EditorGUILayoutUtil.DrawLine(false, 2);

            GUILayout.BeginVertical();

            if (selection == 0)
            {
                Editor editor = Editor.CreateEditor(TenaciousEditorSettings.GetOrCreateSettings());
                editor.DrawHeader();
                editor.OnInspectorGUI();
            }

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }
    }
}
