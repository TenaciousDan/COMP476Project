using UnityEditor;

using TenaciousEditor.Utilities;

namespace TenaciousEditor.UI
{
    public class ScrollViewMenuItems
    {
        public static readonly string SCROLLVIEW_PREFAB_PATH = AssetPaths.UI_DIR + "ScrollView/ScrollView.prefab";

        [MenuItem("GameObject/Tenacious/UI/ScrollViews/ScrollView")]
        static void MenuItem()
        {
            EditorUtil.CreatePrefab("ScrollView", SCROLLVIEW_PREFAB_PATH, true);
        }
    }
}
