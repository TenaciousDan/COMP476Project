using UnityEditor;

using TenaciousEditor.Utilities;

namespace TenaciousEditor.Audio
{
    public class AudioMenuItems
    {
        public static readonly string AUDIOMANAGER_PREFAB_PATH = AssetPaths.AUDIO_DIR + "AudioManager/AudioManager.prefab";

        //[MenuItem("GameObject/Tenacious/Audio/AudioManager")]
        static void MenuItem()
        {
            EditorUtil.CreatePrefab("AudioManager", AUDIOMANAGER_PREFAB_PATH, true);
        }
    }
}
