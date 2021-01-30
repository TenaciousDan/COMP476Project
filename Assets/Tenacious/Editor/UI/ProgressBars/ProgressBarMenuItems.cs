using UnityEditor;

using TenaciousEditor.Utilities;

namespace TenaciousEditor.UI
{
    public class ProgressBarMenuItems
    {
        public static readonly string PROGRESSBAR_PREFAB_PATH = AssetPaths.UI_DIR + "ProgressBars/ProgressBar.prefab";
        public static readonly string PROGRESSBAR_OUTLINE_PREFAB_PATH = AssetPaths.UI_DIR + "ProgressBars/ProgressBarOutline.prefab";
        public static readonly string PROGRESSPIE_PREFAB_PATH = AssetPaths.UI_DIR + "ProgressBars/ProgressPie.prefab";
        public static readonly string PROGRESSKNOB_PREFAB_PATH = AssetPaths.UI_DIR + "ProgressBars/ProgressKnob.prefab";
        public static readonly string PROGRESSKNOB_OUTLINE_PREFAB_PATH = AssetPaths.UI_DIR + "ProgressBars/ProgressKnobOutline.prefab";

        [MenuItem("GameObject/Tenacious/UI/ProgressBars/ProgressBar")]
        static void MenuItem()
        {
            EditorUtil.CreatePrefab("ProgressBar", PROGRESSBAR_PREFAB_PATH, true);
        }

        [MenuItem("GameObject/Tenacious/UI/ProgressBars/ProgressBar (Outline)")]
        static void MenuItem2()
        {
            EditorUtil.CreatePrefab("ProgressBar", PROGRESSBAR_OUTLINE_PREFAB_PATH, true);
        }

        [MenuItem("GameObject/Tenacious/UI/ProgressBars/ProgressPie")]
        static void MenuItem3()
        {
            EditorUtil.CreatePrefab("ProgressPie", PROGRESSPIE_PREFAB_PATH, true);
        }

        [MenuItem("GameObject/Tenacious/UI/ProgressBars/ProgressKnob")]
        static void MenuItem4()
        {
            EditorUtil.CreatePrefab("ProgressKnob", PROGRESSKNOB_PREFAB_PATH, true);
        }

        [MenuItem("GameObject/Tenacious/UI/ProgressBars/ProgressKnob (Outline)")]
        static void MenuItem5()
        {
            EditorUtil.CreatePrefab("ProgressKnob", PROGRESSKNOB_OUTLINE_PREFAB_PATH, true);
        }
    }
}
