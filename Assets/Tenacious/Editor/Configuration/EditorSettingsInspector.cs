using UnityEditor;

namespace TenaciousEditor.Configuration
{
    [CustomEditor(typeof(TenaciousEditorSettings))]
    public class EditorSettingsInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();

            Editor.DrawPropertiesExcluding(this.serializedObject, "m_Script");

            this.serializedObject.ApplyModifiedProperties();
        }
    }
}
