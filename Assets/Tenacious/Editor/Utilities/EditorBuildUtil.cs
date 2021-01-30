using UnityEditor;

using System.Collections.Generic;
using System.IO;

using Object = UnityEngine.Object;
using UnityEngine;

namespace TenaciousEditor.Utilities
{
    public static class EditorBuildUtil
    {
        /// <summary>
        /// For a given Scene Asset object reference, extract its build settings data, including buildIndex.
        /// </summary>
        public static EditorBuildScene GetBuildScene(Object sceneAssetObject)
        {
            EditorBuildScene bSceneInfo = new EditorBuildScene
            {
                buildIndex = -1,
                assetGUID = new GUID(string.Empty)
            };

            if (sceneAssetObject as SceneAsset == null) return bSceneInfo;

            bSceneInfo.assetPath = AssetDatabase.GetAssetPath(sceneAssetObject);
            bSceneInfo.assetGUID = new GUID(AssetDatabase.AssetPathToGUID(bSceneInfo.assetPath));

            var scenes = EditorBuildSettings.scenes;
            for (var index = 0; index < scenes.Length; ++index)
            {
                if (bSceneInfo.assetGUID.Equals(scenes[index].guid))
                {
                    bSceneInfo.scene = scenes[index];
                    bSceneInfo.buildIndex = index;
                    return bSceneInfo;
                }
            }

            return bSceneInfo;
        }

        public static EditorBuildScene AddSceneToBuild(string scenePath, int buildIndex = -1, bool enabled = true)
        {
            EditorBuildSettingsScene buildScene = new EditorBuildSettingsScene(scenePath, enabled);

            List<EditorBuildSettingsScene> buildScenesList = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            if (buildIndex < 0)
                buildScenesList.Add(buildScene);
            else
            {
                buildIndex = buildIndex <= buildScenesList.Count ? buildIndex : buildScenesList.Count;
                buildScenesList.Insert(buildIndex, buildScene);
            }

            EditorBuildSettings.scenes = buildScenesList.ToArray();

            return new EditorBuildScene
            {
                buildIndex = buildIndex,
                scene = buildScene,
                assetGUID = buildScene.guid,
                assetPath = scenePath
            };
        }

        public static void SetBuildIndex(ref EditorBuildScene editorBuildScene, int buildIndex)
        {
            buildIndex = Mathf.Clamp(buildIndex, -1, EditorBuildSettings.scenes.Length - 1);

            if (buildIndex >= 0)
            {
                List<EditorBuildSettingsScene> buildScenesList = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
                EditorBuildSettingsScene ebsScene = buildScenesList[editorBuildScene.buildIndex];
                buildScenesList.RemoveAt(editorBuildScene.buildIndex);

                buildScenesList.Insert(buildIndex, ebsScene);

                EditorBuildSettings.scenes = buildScenesList.ToArray();

                editorBuildScene.buildIndex = buildIndex;
                editorBuildScene.scene = EditorBuildSettings.scenes[buildIndex];
            }
            else
                RemoveSceneFromBuild(ref editorBuildScene);
        }

        public static void RemoveSceneFromBuild(ref EditorBuildScene editorBuildScene)
        {
            List<EditorBuildSettingsScene> buildScenesList = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            buildScenesList.RemoveAt(editorBuildScene.buildIndex);
            EditorBuildSettings.scenes = buildScenesList.ToArray();

            editorBuildScene.buildIndex = -1;
            editorBuildScene.scene = null;
        }
    }

    /// <summary>
    /// Build Scene information.
    /// </summary>
    public struct EditorBuildScene
    {
        public int buildIndex;
        public EditorBuildSettingsScene scene;
        public GUID assetGUID;
        public string assetPath;

        public string SceneName { get => Path.GetFileNameWithoutExtension(assetPath); }
    }
}
