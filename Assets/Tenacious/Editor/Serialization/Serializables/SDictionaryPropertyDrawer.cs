using UnityEditor;

using UnityEngine;

using TenaciousEditor.IMGUI;

using Tenacious.Collections;

namespace TenaciousEditor.Serialization.Serializables
{
    [CustomPropertyDrawer(typeof(SDictionary<,>), true)]
    public class SDictionaryPropertyDrawer : PropertyDrawerBase
    {
        private const string KEY_COLLISIONS_FIELD_NAME = "keyCollisions";
        private const string KEY_TYPENAME_FIELD_NAME = "keyTypeName";
        private const string VALUE_TYPENAME_FIELD_NAME = "valueTypeName";
        private const string ENTRIES_FIELD_NAME = "entries";
        private const string ENTRY_KEY_FIELD_NAME = "key";
        private const string ENTRY_VALUE_FIELD_NAME = "value";

        private bool duplicateKeys;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(position, property, label);

            string keyTypeName = property.FindPropertyRelative(KEY_TYPENAME_FIELD_NAME).stringValue;
            string valueTypeName = property.FindPropertyRelative(VALUE_TYPENAME_FIELD_NAME).stringValue;

            label.text = ObjectNames.NicifyVariableName(this.fieldInfo.Name) + " <" + keyTypeName + ", " + valueTypeName + ">";

            EditorGUI.BeginProperty(position, label, property);

            property.isExpanded = EditorGUI.Foldout(GetRect(ref position), property.isExpanded, label, true);

            if (property.isExpanded)
            {
                BeginPadding(ref position, new Vector4(0, 5, 0, 0));

                Indent(ref position);

                SerializedProperty entries = property.FindPropertyRelative(ENTRIES_FIELD_NAME);
                SerializedProperty keyCollisions = property.FindPropertyRelative(KEY_COLLISIONS_FIELD_NAME);

                EditorGUIUtility.labelWidth = 50f;
                int size = EditorGUI.DelayedIntField(GetRect(ref position), new GUIContent("Size", "The size of the dictionary."), entries.arraySize);
                EditorGUIUtility.labelWidth = originalLabelWidth;
                GetRect(ref position, -1, 5f);

                size = Mathf.Max(size, 0);

                while (size > entries.arraySize)
                    entries.arraySize++;

                while (size < entries.arraySize && size >= 0)
                    entries.arraySize--;

                bool foundDuplicateKeys = false;
                for (int i = 0; i < entries.arraySize; i++)
                {
                    if (i == 0)
                        DrawLine(ref position, -1, 2);

                    SerializedProperty entry = entries.GetArrayElementAtIndex(i);
                    SerializedProperty key = entry.FindPropertyRelative(ENTRY_KEY_FIELD_NAME);
                    SerializedProperty value = entry.FindPropertyRelative(ENTRY_VALUE_FIELD_NAME);

                    float seperatorWidth = 2f;
                    float buttonWidth = 20f;
                    float keyWidth = (position.width - buttonWidth - seperatorWidth - (horizontalSpacing * 2)) / 2f;
                    float keyHeight = EditorGUI.GetPropertyHeight(key);
                    float valueWidth = keyWidth;
                    float valueHeight = EditorGUI.GetPropertyHeight(value);

                    if (keyHeight > valueHeight)
                        valueHeight = keyHeight;
                    else
                        keyHeight = valueHeight;

                    if (i < keyCollisions.arraySize && keyCollisions.GetArrayElementAtIndex(i).boolValue)
                    {
                        foundDuplicateKeys = true;
                        EditorGUI.DrawRect(new Rect(position.x, position.y, position.width, keyHeight), new Color(1, 0, 0, 0.15f));
                        GUI.backgroundColor = Color.red;
                    }
                    else
                        GUI.backgroundColor = originalBackgroundColor;

                    BeginHorizontal(ref position);

                    if (key.hasVisibleChildren) EditorGUIUtility.labelWidth = indentSpacing;
                    EditorGUI.PropertyField(
                        GetRect(ref position, keyWidth, keyHeight),
                        key,
                        key.hasVisibleChildren ? new GUIContent(keyTypeName) : GUIContent.none,
                        true
                    );
                    if (key.hasVisibleChildren) EditorGUIUtility.labelWidth = originalLabelWidth;

                    DrawLine(ref position, new Color(0, 0, 0, 0.5f), seperatorWidth, keyHeight);

                    if (value.hasVisibleChildren) EditorGUIUtility.labelWidth = indentSpacing;
                    EditorGUI.PropertyField(
                        GetRect(ref position, valueWidth, valueHeight),
                        value,
                        value.hasVisibleChildren ? new GUIContent(valueTypeName) : GUIContent.none,
                        true
                    );
                    if (value.hasVisibleChildren) EditorGUIUtility.labelWidth = originalLabelWidth;

                    GUI.backgroundColor = Color.red;
                    if (GUI.Button(GetRect(ref position, buttonWidth), "x"))
                    {
                        GUI.FocusControl(null);
                        entries.DeleteArrayElementAtIndex(i);
                    }
                    GUI.backgroundColor = originalBackgroundColor;

                    EndHorizontal(ref position);

                    DrawLine(ref position, -1, 1);
                }
                duplicateKeys = foundDuplicateKeys;

                GetRect(ref position, -1, 5f);
                if (GUI.Button(GetRect(ref position), "Add"))
                {
                    entries.arraySize++;
                }

                Indent(ref position, -1);

                EndPadding(ref position, new Vector4(0, 5, 0, 0));
                GUI.backgroundColor = new Color(1, 1, 1, 0);
                EditorGUI.HelpBox(new Rect(startPosition, new Vector2(position.width, height)), "", MessageType.None);
                GUI.backgroundColor = originalBackgroundColor;
            }

            if (duplicateKeys)
            {
                GetRect(ref position, -1, 1);

                // there are key collisions, so we render a warning box.
                EditorGUI.HelpBox(
                    GetRect(ref position, -1, EditorGUIUtility.singleLineHeight * 3),
                    "There are duplicate keys in the dictionary. Duplicates will be excluded.",
                    MessageType.Warning
                );
            }

            EditorGUI.EndProperty();
        }
    }
}
