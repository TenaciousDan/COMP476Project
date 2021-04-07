using UnityEditor;

namespace TenaciousEditor
{
    public static class AssetPaths
    {
        private static string tenaciousRootDir;
        public static string ASSET_ROOT_DIR
        {
            get
            {
                if (tenaciousRootDir != null && !string.IsNullOrEmpty(tenaciousRootDir.Trim()))
                    return tenaciousRootDir;
                else
                {
                    tenaciousRootDir = EditorPrefs.GetString(
                        PlayerSettings.productGUID.ToString() + "TenaciousEditor.AssetPaths.AssetRootDirectory",
                        "Assets/Tenacious/"
                    );
                    
                    return tenaciousRootDir;
                }
            }
            set
            {
                if (value != null && !string.IsNullOrEmpty(value.Trim()))
                {
                    EditorPrefs.SetString(PlayerSettings.productGUID.ToString() + "TenaciousEditor.AssetPaths.AssetRootDirectory", value);
                    tenaciousRootDir = value;
                }
            }
        }

        public static readonly string EDITOR_DIR = ASSET_ROOT_DIR + "Editor/";
        public static readonly string EDITOR_CONFIG_DIR = EDITOR_DIR + "Configuration/";
        public static readonly string EDITOR_GRAPHVIEW_DIR = EDITOR_DIR + "Collections/GraphView/";

        public static readonly string PROJECT_DIR = ASSET_ROOT_DIR + "_Project/";
        public static readonly string COMMON_DIR = PROJECT_DIR + "Common/";
        public static readonly string CONFIG_DIR = PROJECT_DIR + "Configuration/";
        public static readonly string UI_DIR = PROJECT_DIR + "UI/";
        public static readonly string AUDIO_DIR = PROJECT_DIR + "Audio/";
        public static readonly string SCENES_DIR = PROJECT_DIR + "Scenes/";
        public static readonly string I18N_DIR = PROJECT_DIR + "I18n/";
    }
}
