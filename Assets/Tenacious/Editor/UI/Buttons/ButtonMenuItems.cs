using UnityEditor;

using TenaciousEditor.Utilities;

namespace TenaciousEditor.UI
{
    public class ButtonMenuItems
    {
        public static readonly string BASIC_BUTTON_PREFAB_PATH = AssetPaths.UI_DIR + "Buttons/Button.prefab";
        public static readonly string OUTLINE_BUTTON_PREFAB_PATH = AssetPaths.UI_DIR + "Buttons/OutlineButton.prefab";

        [MenuItem("GameObject/Tenacious/UI/Buttons/Button")]
        static void MenuItem()
        {
            EditorUtil.CreatePrefab("Button", BASIC_BUTTON_PREFAB_PATH, true);
        }

        [MenuItem("GameObject/Tenacious/UI/Buttons/OutlineButton")]
        static void MenuItem2()
        {
            EditorUtil.CreatePrefab("Button", OUTLINE_BUTTON_PREFAB_PATH, true);
        }
    }
}