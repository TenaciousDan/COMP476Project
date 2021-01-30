using UnityEditor;

using UnityEngine;

using TenaciousEditor.IMGUI;
using TenaciousEditor.Utilities;

using Tenacious.Scenes;

namespace TenaciousEditor.Serialization.Serializables
{
    [CustomPropertyDrawer(typeof(SceneReference))]
    public class SceneReferencePropertyDrawer : PropertyDrawerBase
    {
        private const string SCENE_ASSET_FIELD_NAME = "sceneAsset";
        private const string SCENE_PATH_FIELD_NAME = "scenePath";

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(position, property, label);

            SerializedProperty spSceneAsset = property.FindPropertyRelative(SCENE_ASSET_FIELD_NAME);
            EditorBuildScene editorBuildScene = EditorBuildUtil.GetBuildScene(spSceneAsset.objectReferenceValue);

            EditorGUI.BeginProperty(position, GUIContent.none, property);
            {
                label.text = label.text + " {" + editorBuildScene.SceneName + ", " + editorBuildScene.buildIndex + "}";
                property.isExpanded = EditorGUI.Foldout(GetRect(ref position), property.isExpanded, label, true);

                if (property.isExpanded)
                {
                    BeginPadding(ref position, new Vector4(0, 3, 0, 0));

                    Indent(ref position);

                    BeginHorizontal(ref position);
                    {
                        EditorGUI.BeginChangeCheck();
                        {
                            spSceneAsset.objectReferenceValue = EditorGUI.ObjectField(
                                GetRect(ref position, position.width - (EditorGUIUtil.CalcTextSize("Remove From Build").x + 20f)),
                                spSceneAsset.objectReferenceValue,
                                typeof(SceneAsset),
                                false
                            );
                        }
                        if (EditorGUI.EndChangeCheck())
                        {
                            string sceneAssetPath = null;
                            if (spSceneAsset.objectReferenceValue as SceneAsset != null)
                                sceneAssetPath = AssetDatabase.GetAssetPath(spSceneAsset.objectReferenceValue);

                            if (string.IsNullOrEmpty(sceneAssetPath)) property.FindPropertyRelative(SCENE_PATH_FIELD_NAME).stringValue = string.Empty;
                        }

                        if (editorBuildScene.buildIndex < 0)
                        {
                            if (GUI.Button(GetRect(ref position), "Add To Build"))
                                EditorBuildUtil.AddSceneToBuild(editorBuildScene.assetPath);
                        }
                        else
                        {
                            if (GUI.Button(GetRect(ref position), "Remove From Build"))
                                EditorBuildUtil.RemoveSceneFromBuild(ref editorBuildScene);
                        }
                    }
                    EndHorizontal(ref position);

                    DrawEditorBuildScene(ref position, ref editorBuildScene);

                    Indent(ref position, -1);

                    EndPadding(ref position, new Vector4(0, 3, 0, 0));
                    GUI.backgroundColor = new Color(1, 1, 1, 0);
                    EditorGUI.HelpBox(new Rect(startPosition, new Vector2(position.width, height)), "", MessageType.None);
                    GUI.backgroundColor = originalBackgroundColor;
                }
            }
            EditorGUI.EndProperty();
        }

        private void DrawEditorBuildScene(ref Rect position, ref EditorBuildScene editorBuildScene)
        {
            if (editorBuildScene.buildIndex < 0)
                GUI.color = Color.red;
            else
                GUI.color = Color.green;

            GUIContent buildIndexlabel = new GUIContent(
                "Build Index",
                "The index of the scene asset in the build settings. " +
                "\nRed or -1 means it's NOT included in build. " +
                "\nGreen or positive means it's included in build."
            );
            int buildIndex = EditorGUI.DelayedIntField(GetRect(ref position), buildIndexlabel, editorBuildScene.buildIndex);
            buildIndex = Mathf.Clamp(buildIndex, -1, EditorBuildSettings.scenes.Length - 1);

            if (editorBuildScene.buildIndex != buildIndex)
            {
                if (editorBuildScene.buildIndex < 0)
                    EditorBuildUtil.AddSceneToBuild(editorBuildScene.assetPath, buildIndex);
                else
                    EditorBuildUtil.SetBuildIndex(ref editorBuildScene, buildIndex);
            }

            GUI.color = originalColor;
        }
    }
}
