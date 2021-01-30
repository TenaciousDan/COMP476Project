using UnityEditor;

namespace TenaciousEditor
{
    public static class MenuItems
    {
        [MenuItem("GameObject/Tenacious/", false, 15)]
        static void GameObjectMenu() { }

        [MenuItem("GameObject/Tenacious/General/", false)]
        static void GameObjectMenuGeneral() { }
    }
}
