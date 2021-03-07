using UnityEditor;

using UnityEngine;
using UnityEngine.UI;

using TenaciousEditor.Utilities;

using System.Collections.Generic;

namespace Editor.UI
{
    public static class UIMenuItems
    {
        public static readonly string BUTTON_PREFAB_PATH = AssetPaths.PREFAB_DIR + "UI/Button.prefab";
        public static readonly string PANEL_PREFAB_PATH = AssetPaths.PREFAB_DIR + "UI/Panel.prefab";
        public static readonly string PANEL_WITH_TITLE_PREFAB_PATH = AssetPaths.PREFAB_DIR + "UI/PanelWithTitle.prefab";
        public static readonly string SCROLL_VIEW_PREFAB_PATH = AssetPaths.PREFAB_DIR + "UI/Scroll View.prefab";
        public static readonly string TOGGLE_PREFAB_PATH = AssetPaths.PREFAB_DIR + "UI/Toggle.prefab";
        public static readonly string TEXT_PREFAB_PATH = AssetPaths.PREFAB_DIR + "UI/Text.prefab";
        public static readonly string DROPDOWN_PREFAB_PATH = AssetPaths.PREFAB_DIR + "UI/Dropdown.prefab";
        public static readonly string HEADER_PREFAB_PATH = AssetPaths.PREFAB_DIR + "UI/Header.prefab";

        public static int[] textSizes = new int[6] { 256, 192, 128, 64, 42, 28 };

        [MenuItem(MenuItems.GAMEOBJECT_PREFAB_UI_MENUITEM_PATH + "Button")]
        static void ButtonPrefabMenuItem()
        {
            EditorUtil.CreatePrefab("Button", BUTTON_PREFAB_PATH);
        }

        [MenuItem(MenuItems.GAMEOBJECT_PREFAB_UI_MENUITEM_PATH + "Panel")]
        static void PanelPrefabMenuItem()
        {
            EditorUtil.CreatePrefab("Panel", PANEL_PREFAB_PATH);
        }

        [MenuItem(MenuItems.GAMEOBJECT_PREFAB_UI_MENUITEM_PATH + "Panel With Title")]
        static void PanelWithTitlePrefabMenuItem()
        {
            EditorUtil.CreatePrefab("Panel", PANEL_WITH_TITLE_PREFAB_PATH);
        }

        [MenuItem(MenuItems.GAMEOBJECT_PREFAB_UI_MENUITEM_PATH + "Scroll View")]
        static void ScrollViewPrefabMenuItem()
        {
            EditorUtil.CreatePrefab("Scroll View", SCROLL_VIEW_PREFAB_PATH);
        }

        [MenuItem(MenuItems.GAMEOBJECT_PREFAB_UI_MENUITEM_PATH + "Toggle")]
        static void TogglePrefabMenuItem()
        {
            EditorUtil.CreatePrefab("Toggle", TOGGLE_PREFAB_PATH);
        }

        [MenuItem(MenuItems.GAMEOBJECT_PREFAB_UI_MENUITEM_PATH + "Text")]
        static void TextPrefabMenuItem()
        {
            EditorUtil.CreatePrefab("Text", TEXT_PREFAB_PATH);
        }

        [MenuItem(MenuItems.GAMEOBJECT_PREFAB_UI_MENUITEM_PATH + "Dropdown")]
        static void DropdownPrefabMenuItem()
        {
            EditorUtil.CreatePrefab("Dropdown", DROPDOWN_PREFAB_PATH);
        }

        [MenuItem(MenuItems.GAMEOBJECT_PREFAB_UI_MENUITEM_PATH + "Headers/H1")]
        static void H1PrefabMenuItem() { HeaderPrefabMenuItem(textSizes[0]); }
        [MenuItem(MenuItems.GAMEOBJECT_PREFAB_UI_MENUITEM_PATH + "Headers/H2")]
        static void H2PrefabMenuItem() { HeaderPrefabMenuItem(textSizes[1]); }
        [MenuItem(MenuItems.GAMEOBJECT_PREFAB_UI_MENUITEM_PATH + "Headers/H3")]
        static void H3PrefabMenuItem() { HeaderPrefabMenuItem(textSizes[2]); }
        [MenuItem(MenuItems.GAMEOBJECT_PREFAB_UI_MENUITEM_PATH + "Headers/H4")]
        static void H4PrefabMenuItem() { HeaderPrefabMenuItem(textSizes[3]); }
        [MenuItem(MenuItems.GAMEOBJECT_PREFAB_UI_MENUITEM_PATH + "Headers/H5")]
        static void H5PrefabMenuItem() { HeaderPrefabMenuItem(textSizes[4]); }
        [MenuItem(MenuItems.GAMEOBJECT_PREFAB_UI_MENUITEM_PATH + "Headers/H6")]
        static void H6PrefabMenuItem() { HeaderPrefabMenuItem(textSizes[5], true); }
        static void HeaderPrefabMenuItem(int fontSize = 28, bool isBold = false)
        {
            List<GameObject> headers = EditorUtil.CreatePrefab("Header", HEADER_PREFAB_PATH);
            foreach (GameObject g in headers)
            {
                Text text = g.GetComponent<Text>();
                text.fontSize = fontSize;
                if (isBold) text.fontStyle = FontStyle.Bold;
                text.alignment = TextAnchor.LowerLeft;
                RectTransform rectTransform = (RectTransform)g.transform;
                rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, fontSize);
            }
        }
    }
}