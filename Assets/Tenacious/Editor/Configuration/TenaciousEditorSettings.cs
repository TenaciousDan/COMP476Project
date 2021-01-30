using UnityEditor;

using UnityEngine;

namespace TenaciousEditor.Configuration
{
    public class TenaciousEditorSettings : ScriptableObject
    {
        public static TenaciousEditorSettings GetOrCreateSettings()
        {
            string settingsPath = AssetPaths.EDITOR_CONFIG_DIR + "EditorSettings.asset";
            TenaciousEditorSettings settings = AssetDatabase.LoadAssetAtPath<TenaciousEditorSettings>(settingsPath);
            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<TenaciousEditorSettings>();
                AssetDatabase.CreateAsset(settings, settingsPath);
                AssetDatabase.SaveAssets();
            }
            return settings;
        }

        public static void SaveSettings(TenaciousEditorSettings settings)
        {
            string settingsPath = AssetPaths.EDITOR_CONFIG_DIR + "EditorSettings.asset";
            AssetDatabase.CreateAsset(settings, settingsPath);
            AssetDatabase.SaveAssets();
        }
    }
}
