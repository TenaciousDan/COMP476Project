using UnityEditor;

using TenaciousEditor.Utilities;

namespace TenaciousEditor.UI.Windows
{
    public class WindowsMenuItems
    {
        public static readonly string WINDOWMANAGER_PREFAB_PATH = AssetPaths.UI_DIR + "Windows/WindowManager.prefab";
        public static readonly string WINDOW_PREFAB_PATH = AssetPaths.UI_DIR + "Windows/Window.prefab";

        [MenuItem("GameObject/Tenacious/UI/Windows/WindowManager")]
        static void MenuItem()
        {
            EditorUtil.CreatePrefab("WindowManager", WINDOWMANAGER_PREFAB_PATH, true);
        }

        [MenuItem("GameObject/Tenacious/UI/Windows/Window")]
        static void MenuItem2()
        {
            EditorUtil.CreatePrefab("Window", WINDOW_PREFAB_PATH, true);
        }
    }
}