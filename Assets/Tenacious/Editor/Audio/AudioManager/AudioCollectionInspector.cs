using UnityEditor;

using UnityEngine;

using System.IO;

namespace Tenacious.Audio
{
    [CustomEditor(typeof(AudioCollection))]
    public class AudioCollectionInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Add Music (click or drag&drop files)"))
            {
                string musicDirectoryPath = EditorUtility.OpenFolderPanel("Select Music Directory", Application.dataPath, "");
                if (!string.IsNullOrWhiteSpace(musicDirectoryPath))
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(musicDirectoryPath);
                    foreach (FileInfo fileInfo in dirInfo.GetFiles("*.*"))
                    {
                        string relativePath = fileInfo.FullName.Replace("\\", "/").Replace(Application.dataPath, "Assets");
                        AudioClip audioClip = AssetDatabase.LoadAssetAtPath<AudioClip>(relativePath);
                        if (audioClip != null)
                            AddMusic(audioClip);
                    }
                }
            }

            Rect musicButtonRect = GUILayoutUtility.GetLastRect();

            if (GUILayout.Button("Add Sounds (click or drag&drop files)"))
            {
                string soundDirectoryPath = EditorUtility.OpenFolderPanel("Select Music Directory", Application.dataPath, "");
                DirectoryInfo dirInfo = new DirectoryInfo(soundDirectoryPath);
                foreach (FileInfo fileInfo in dirInfo.GetFiles("*.*"))
                {
                    string relativePath = fileInfo.FullName.Replace("\\", "/").Replace(Application.dataPath, "Assets");
                    AudioClip audioClip = AssetDatabase.LoadAssetAtPath<AudioClip>(relativePath);
                    if (audioClip != null)
                        AddSound(audioClip);
                }
            }

            Rect soundsButtonRect = GUILayoutUtility.GetLastRect();

            Event currentEvent = Event.current;
            if (musicButtonRect.Contains(currentEvent.mousePosition) || soundsButtonRect.Contains(currentEvent.mousePosition))
            {
                if (Event.current.type == EventType.DragUpdated)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    Event.current.Use();
                }
                else if (Event.current.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    foreach (Object draggedObject in DragAndDrop.objectReferences)
                    {
                        if (draggedObject is AudioClip)
                        {
                            if (musicButtonRect.Contains(currentEvent.mousePosition))
                            {
                                AddMusic((AudioClip) draggedObject);
                            }
                            else if (soundsButtonRect.Contains(currentEvent.mousePosition))
                            {
                                AddSound((AudioClip)draggedObject);
                            }
                        }
                    }
                }
            }

            base.OnInspectorGUI();
        }

        private void AddMusic(AudioClip audioClip)
        {
            AudioCollection audioCollection = (AudioCollection) target;
            audioCollection.AddMusic(audioClip);
        }

        private void AddSound(AudioClip audioClip)
        {
            AudioCollection audioCollection = (AudioCollection) target;
            audioCollection.AddSound(audioClip);
        }
    }
}
