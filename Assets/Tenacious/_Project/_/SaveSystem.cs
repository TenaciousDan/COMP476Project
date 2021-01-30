using UnityEngine;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using Tenacious.Data;
using Tenacious.Serialization;

namespace Tenacious
{
    public static class SaveSystem
    {
        public static readonly string SAVE_DIR = Application.persistentDataPath + "/save/";

        private const string SAVE_FILE_EXT = ".sav";
        private const string SAVEMETADATA_FILE_EXT = ".meta";

        public static void Save<T>(T saveData, BinaryFormatter formatter = null) where T : SaveData
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                Debug.LogError("Platform not supported: " + Application.platform.ToString());
                return;
            }
            else
            {
                if (!Directory.Exists(SAVE_DIR))
                    Directory.CreateDirectory(SAVE_DIR);

                string savePath = SAVE_DIR + saveData.fileName + SAVE_FILE_EXT;

                if (formatter == null) formatter = BinaryFormat.Formatter;

                FileStream stream = new FileStream(savePath, FileMode.Create);
                formatter.Serialize(stream, saveData);
                stream.Close();

                string saveMetadataPath = SAVE_DIR + "save_metadata" + SAVEMETADATA_FILE_EXT;

                SaveMetadata saveMetadata;
                if (File.Exists(saveMetadataPath))
                {
                    FileStream stream2 = new FileStream(saveMetadataPath, FileMode.Open);
                    saveMetadata = formatter.Deserialize(stream2) as SaveMetadata;
                    stream2.Close();
                }
                else
                {
                    saveMetadata = new SaveMetadata();
                    FileStream stream3 = new FileStream(saveMetadataPath, FileMode.Create);
                    formatter.Serialize(stream3, saveMetadata);
                    stream3.Close();
                }

                if (!saveMetadata.fileNames.Contains(saveData.fileName))
                    saveMetadata.fileNames.Add(saveData.fileName);

                saveMetadata.lastSave = saveData.fileName;

                FileStream stream4 = new FileStream(saveMetadataPath, FileMode.Create);
                formatter.Serialize(stream4, saveMetadata);
                stream4.Close();
            }
        }

        public static T Load<T>(string fileName, BinaryFormatter formatter = null) where T : SaveData
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                Debug.LogError("Platform not supported: " + Application.platform.ToString());
                return null;
            }
            else
            {
                if (!Directory.Exists(SAVE_DIR))
                    Directory.CreateDirectory(SAVE_DIR);

                T data = null;

                string path = SAVE_DIR + fileName + SAVE_FILE_EXT;
                if (fileName != null && File.Exists(path))
                {
                    if (formatter == null) formatter = BinaryFormat.Formatter;
                    FileStream stream = new FileStream(path, FileMode.Open);
                    data = formatter.Deserialize(stream) as T;
                    stream.Close();
                }

                return data;
            }
        }

        public static void Delete(string name, BinaryFormatter formatter = null)
        {
            string saveDataPath = SAVE_DIR + name + SAVE_FILE_EXT;

            if (File.Exists(saveDataPath))
            {
                string saveMetadataPath = SAVE_DIR + "save_metadata" + SAVEMETADATA_FILE_EXT;
                if (File.Exists(saveMetadataPath))
                {
                    if (formatter == null) formatter = BinaryFormat.Formatter;
                    FileStream stream = new FileStream(saveMetadataPath, FileMode.Open);
                    SaveMetadata saveMetadata = formatter.Deserialize(stream) as SaveMetadata;
                    stream.Close();

                    if (saveMetadata.fileNames.Contains(name))
                        saveMetadata.fileNames.Remove(name);

                    if (saveMetadata.lastSave.Equals(name))
                        saveMetadata.lastSave = null;

                    FileStream stream2 = new FileStream(saveMetadataPath, FileMode.Create);
                    formatter.Serialize(stream2, saveMetadata);
                    stream2.Close();
                }

                File.Delete(saveDataPath);
            }
        }

        public static SaveMetadata GetSaveMetadata(BinaryFormatter formatter = null)
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                Debug.LogError("Platform not supported: " + Application.platform.ToString());
                return null;
            }
            else
            {
                if (!Directory.Exists(SAVE_DIR))
                    Directory.CreateDirectory(SAVE_DIR);

                SaveMetadata saveMetadata = new SaveMetadata();

                string path = SAVE_DIR + "save_metadata" + SAVEMETADATA_FILE_EXT;
                if (File.Exists(path))
                {
                    if (formatter == null) formatter = BinaryFormat.Formatter;

                    FileStream stream = new FileStream(path, FileMode.Open);
                    saveMetadata = formatter.Deserialize(stream) as SaveMetadata;
                    stream.Close();
                }

                return saveMetadata;
            }
        }
    }
}
