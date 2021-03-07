using UnityEditor;

namespace Editor
{
    public static class AssetPaths
    {
        public static string ASSET_ROOT_DIR = "Assets/";

        public static readonly string EDITOR_DIR = ASSET_ROOT_DIR + "Editor/";

        public static readonly string PROJECT_DIR = ASSET_ROOT_DIR + "_Project/";
        public static readonly string PREFAB_DIR = PROJECT_DIR + "Prefabs/";
    }
}
