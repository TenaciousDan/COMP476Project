using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

using System;

using Object = UnityEngine.Object;

namespace Tenacious.Scenes
{
    [Serializable]
    public class SceneReference : ISerializationCallbackReceiver
    {
        [SerializeField] private string scenePath;

        public SceneReference()
        {
            ScenePath = string.Empty;
        }

        public SceneReference(string scenePath)
        {
            ScenePath = scenePath;
        }

#if UNITY_EDITOR
        [SerializeField] private Object sceneAsset;

        private bool IsValidSceneAsset()
        {
            if (sceneAsset == null) return false;

            return sceneAsset is SceneAsset;
        }

        private SceneAsset GetSceneAssetFromPath()
        {
            return string.IsNullOrEmpty(scenePath) ? null : AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
        }

        private string GetPathFromSceneAsset()
        {
            return sceneAsset == null ? string.Empty : AssetDatabase.GetAssetPath(sceneAsset);
        }
#endif

        public void OnAfterDeserialize()
        {
#if UNITY_EDITOR
            // We cannot use AssetDatabase during serialization/deserialization, so we defer by a bit.
            EditorApplication.CallbackFunction handler = null;
            handler = () =>
            {
                EditorApplication.update -= handler;

                if (IsValidSceneAsset()) return;
                if (string.IsNullOrEmpty(scenePath)) return;

                // The SceneAsset is invalid but there is a path set
                sceneAsset = GetSceneAssetFromPath();

                // The path was invalid and no SceneAsset was found. Make sure we don't carry over the invalid path.
                if (sceneAsset == null) 
                    scenePath = string.Empty;

                // In case we change the scene during play in the editor.
                if (!Application.isPlaying) EditorSceneManager.MarkAllScenesDirty();
            };
            EditorApplication.update += handler;
#endif
        }

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            // The SceneAsset is invalid but the path is not empty
            if (!IsValidSceneAsset() && !string.IsNullOrEmpty(scenePath))
            {
                sceneAsset = GetSceneAssetFromPath();
                if (sceneAsset == null) scenePath = string.Empty;

                EditorSceneManager.MarkAllScenesDirty();
            }
            else // The SceneAsset is valid and takes precendence, so we change the path to match
            {
                scenePath = GetPathFromSceneAsset();
            }
#endif
        }

        public string ScenePath
        {
            get
            {
#if UNITY_EDITOR
                // In editor we always use the asset's path to avoid renaming problems
                return GetPathFromSceneAsset();
#else
                // At runtime we rely on the stored path value which we assume was serialized/deserialized correctly at build time.
                return scenePath;
#endif
            }
            set
            {
                scenePath = value;
#if UNITY_EDITOR
                sceneAsset = GetSceneAssetFromPath();
#endif
            }
        }

        public static implicit operator string(SceneReference sscene)
        {
            return sscene.ScenePath;
        }
    }
}
