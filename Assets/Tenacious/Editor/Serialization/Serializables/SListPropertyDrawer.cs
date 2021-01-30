using UnityEditor;

using UnityEngine;

using TenaciousEditor.IMGUI;

using Tenacious.Collections;

namespace TenaciousEditor.Serialization.Serializables
{
    [CustomPropertyDrawer(typeof(SList<>), true)]
    public class SListPropertyDrawer : PropertyDrawerBase
    {
        private const string SELF_REFERENCE_FIELD_NAME = "list";
        private const string TYPENAME_FIELD_NAME = "typeName";

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(position, property, label);

            string typeName = property.FindPropertyRelative(TYPENAME_FIELD_NAME).stringValue;

            label.text = ObjectNames.NicifyVariableName(this.fieldInfo.Name) + " <" + typeName + ">";

            EditorGUI.BeginProperty(position, label, property);

            property.isExpanded = EditorGUI.Foldout(GetRect(ref position), property.isExpanded, label, true);

            if (property.isExpanded)
            {
                BeginPadding(ref position, new Vector4(0, 3, 0, 0));

                SerializedProperty list = property.FindPropertyRelative(SELF_REFERENCE_FIELD_NAME);

                EditorGUIUtility.labelWidth = 50f;
                int size = EditorGUI.DelayedIntField(GetRect(ref position), new GUIContent("Size", "The size of the list."), list.arraySize);
                EditorGUIUtility.labelWidth = originalLabelWidth;
                GetRect(ref position, -1, 5f);

                size = Mathf.Max(size, 0);

                while (size > list.arraySize)
                    list.arraySize++;

                while (size < list.arraySize && size >= 0)
                    list.arraySize--;

                Indent(ref position);

                for (int i = 0; i < list.arraySize; i++)
                {
                    if (i == 0)
                        DrawLine(ref position, -1, 2);

                    SerializedProperty item = list.GetArrayElementAtIndex(i);

                    float buttonWidth = 20f;
                    float itemWidth = position.width - buttonWidth - horizontalSpacing;
                    float itemHeight = EditorGUI.GetPropertyHeight(item);

                    BeginHorizontal(ref position);

                    if (!item.hasVisibleChildren) EditorGUIUtility.labelWidth = 75f;
                    EditorGUI.PropertyField(
                        GetRect(ref position, itemWidth, itemHeight),
                        item,
                        new GUIContent("[" + i.ToString() + "]"),
                        true
                    );
                    if (!item.hasVisibleChildren) EditorGUIUtility.labelWidth = originalLabelWidth;

                    GUI.backgroundColor = Color.red;
                    if (GUI.Button(GetRect(ref position, buttonWidth), "x"))
                    {
                        GUI.FocusControl(null);
                        list.DeleteArrayElementAtIndex(i);
                    }
                    GUI.backgroundColor = originalBackgroundColor;

                    EndHorizontal(ref position);

                    DrawLine(ref position, -1, 1);
                }

                GetRect(ref position, -1, 5f);
                if (GUI.Button(GetRect(ref position), "Add"))
                {
                    list.arraySize++;
                }

                Indent(ref position, -1);

                EndPadding(ref position, new Vector4(0, 3, 0, 0));
                GUI.backgroundColor = new Color(1, 1, 1, 0);
                EditorGUI.HelpBox(new Rect(startPosition, new Vector2(position.width, height)), "", MessageType.None);
                GUI.backgroundColor = originalBackgroundColor;
            }

            EditorGUI.EndProperty();
        }
    }
}
