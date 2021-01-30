using UnityEditor;

using Tenacious.UI;

namespace TenaciousEditor.UI
{
    [CustomEditor(typeof(UIButton))]
    public class UIButtonInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            UIButton target = (UIButton)this.target;

            SerializedProperty begin = serializedObject.GetIterator();
            if (begin != null)
            {
                SerializedProperty it = begin.Copy();
                if (it.NextVisible(true))
                {
                    do 
                    {
                        if (it.name == "label")
                        {
                            EditorGUI.BeginChangeCheck();
                            string txtLabel = EditorGUILayout.TextField("Label", target.Label);
                            if (EditorGUI.EndChangeCheck())
                            {
                                target.Label = txtLabel;
                                EditorUtility.SetDirty(target);
                            }
                        }
                        else
                            EditorGUILayout.PropertyField(it);
                    }
                    while (it.NextVisible(false));
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
