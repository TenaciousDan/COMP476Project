using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEditor;
using UnityEditor.SceneManagement;

namespace TenaciousEditor.Scenes
{
    [InitializeOnLoad]
    public static class AutoPreloadScene
    {
        private const string MENU_ITEM_LOAD_PRELOAD_SCENE_ON_PLAY = "Tenacious/AutoPreloadScene/Load Preload Scene On Play";
        private const string MENU_ITEM_DONT_LOAD_PRELOAD_SCENE_ON_PLAY = "Tenacious/AutoPreloadScene/Don't Load Preload Scene On Play";

        static AutoPreloadScene()
        {
            // bind to OnPlayModeChanged callback
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        [MenuItem(MENU_ITEM_LOAD_PRELOAD_SCENE_ON_PLAY)]
        private static void EnableLoadPreloadSceneOnPlay()
        {
            string preloadSceneName = EditorUtility.OpenFilePanel("Select Preload Scene", Application.dataPath, "unity");
            preloadSceneName = preloadSceneName.Replace(Application.dataPath, "Assets");

            if (!string.IsNullOrEmpty(preloadSceneName))
            {
                PreloadScenePath = preloadSceneName;
                LoadPreloadSceneOnPlay = true;
            }
        }

        [MenuItem(MENU_ITEM_DONT_LOAD_PRELOAD_SCENE_ON_PLAY, true)]
        private static bool ShowDontLoadPreloadSceneOnPlay()
        {
            return LoadPreloadSceneOnPlay;
        }
        [MenuItem(MENU_ITEM_DONT_LOAD_PRELOAD_SCENE_ON_PLAY)]
        private static void DisableLoadPreloadSceneOnPlay()
        {
            LoadPreloadSceneOnPlay = false;
        }

        // PlayModeChanged callback
        private static void OnPlayModeChanged(PlayModeStateChange state)
        {
            if (!LoadPreloadSceneOnPlay) return;

            if (!EditorApplication.isPlaying)
            {
                if (EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    // User pressed play
                    PreviousActiveScenePath = SceneManager.GetActiveScene().path;

                    // autoload preload scene but make sure user saves changes
                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    {
                        try
                        {
                            Scene preloadScene = EditorSceneManager.OpenScene(PreloadScenePath, OpenSceneMode.Additive);
                            SceneManager.SetActiveScene(preloadScene);
                        }
                        catch
                        {
                            Debug.LogError("Preload Scene not found: " + PreloadScenePath);
                            EditorApplication.isPlaying = false;
                        }
                    }
                    else
                    {
                        // User cancelled the save operation, cancel play as well
                        EditorApplication.isPlaying = false;
                    }
                }

                if (!EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    // we were about to play, but user pressed stop or canceled play
                    SceneManager.SetActiveScene(SceneManager.GetSceneByPath(PreviousActiveScenePath));
                }
            }

            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                SceneManager.SetActiveScene(SceneManager.GetSceneByPath(PreviousActiveScenePath));
            }
        }

        // editor preferences
        public static readonly string EDITOR_PREFERENCE_LOAD_PRELOAD_SCENE_ON_PLAY = PlayerSettings.productGUID.ToString() + "TenaciousEditor.Scenes.AutoPreloadScene.LoadPreloadSceneOnPlay";
        public static readonly string EDITOR_PREFERENCE_PRELOAD_SCENE = PlayerSettings.productGUID.ToString() + "TenaciousEditor.Scenes.AutoPreloadScene.PreloadScene";

        private static bool LoadPreloadSceneOnPlay
        {
            get { return EditorPrefs.GetBool(EDITOR_PREFERENCE_LOAD_PRELOAD_SCENE_ON_PLAY, false); }
            set { EditorPrefs.SetBool(EDITOR_PREFERENCE_LOAD_PRELOAD_SCENE_ON_PLAY, value); }
        }

        private static string PreloadScenePath
        {
            get { return EditorPrefs.GetString(EDITOR_PREFERENCE_PRELOAD_SCENE, "Assets/_Scenes/Preload.unity"); }
            set { EditorPrefs.SetString(EDITOR_PREFERENCE_PRELOAD_SCENE, value); }
        }

        private static string PreviousActiveScenePath
        {
            get { return SessionState.GetString("PreviousActiveScenePath", PreloadScenePath); }
            set { SessionState.SetString("PreviousActiveScenePath", value); }
        }
    }
}