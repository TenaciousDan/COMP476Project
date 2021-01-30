using UnityEditor;
using UnityEditor.SceneTemplate;

using TenaciousEditor.Utilities;

namespace TenaciousEditor.Scenes
{
    public class ScenesMenuItems
    {
        public static readonly string SCENELOADER_PREFAB_PATH = AssetPaths.SCENES_DIR + "SceneLoader/SceneLoader.prefab";
        public static readonly string BASIC_SCENE_TEMPLATE_PATH = AssetPaths.SCENES_DIR + "Templates/BasicScene.scenetemplate";

        [MenuItem("GameObject/Tenacious/Scenes/SceneLoader")]
        static void MenuItem()
        {
            EditorUtil.CreatePrefab("SceneLoader", SCENELOADER_PREFAB_PATH, true);
        }

        [MenuItem("Tenacious/Scenes/Basic Scene")]
        static void MenuItem2()
        {
            SceneTemplateService.Instantiate(AssetDatabase.LoadAssetAtPath<SceneTemplateAsset>(BASIC_SCENE_TEMPLATE_PATH), false);
        }
    }
}
