using UnityEditor;

namespace Editor
{
    public static class MenuItems
    {
        public const string GAMEOBJECT_MENUITEM_PATH = "GameObject/TornadoSunshine/";

        public const string GAMEOBJECT_PREFAB_MENUITEM_PATH = GAMEOBJECT_MENUITEM_PATH + "Prefabs/";
        public const string GAMEOBJECT_PREFAB_UI_MENUITEM_PATH = GAMEOBJECT_PREFAB_MENUITEM_PATH + "UI/";

        // set order of context menu items

        // also set priority for GameObject context menu
        [MenuItem(GAMEOBJECT_MENUITEM_PATH, false, 14)]
        static void GameObjectMenu() { }

        [MenuItem(GAMEOBJECT_PREFAB_MENUITEM_PATH)]
        static void GameObjectPrefabMenu() { }
    }
}
