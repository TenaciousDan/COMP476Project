using UnityEditor;

using UnityEngine;

using System.Collections.Generic;

namespace TenaciousEditor.Utilities
{
    public static class EditorUtil
    {
        public static List<GameObject> CreatePrefab(string name, string assetPath, bool unpack = false)
        {
            List<GameObject> addedGameObjects = new List<GameObject>();
            GameObject[] selectedGameObjects = Selection.gameObjects;
            if (selectedGameObjects.Length == 0)
            {
                GameObject prefab = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<Object>(assetPath));
                prefab.name = name;

                Undo.RegisterCreatedObjectUndo(prefab, "Create " + prefab.name);

                addedGameObjects.Add(prefab);

                if (unpack)
                    PrefabUtility.UnpackPrefabInstance(prefab, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            }
            else
            {
                foreach (GameObject selectedGameObject in selectedGameObjects)
                {
                    GameObject prefab = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<Object>(assetPath));
                    prefab.name = name;
                    prefab.transform.SetParent(selectedGameObject.transform, false);

                    Undo.RegisterCreatedObjectUndo(prefab, "Create " + prefab.name);

                    addedGameObjects.Add(prefab);

                    if (unpack)
                        PrefabUtility.UnpackPrefabInstance(prefab, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
                }
                Selection.objects = selectedGameObjects = null;
            }

            return addedGameObjects;
        }

        public static List<GameObject> CreateGameObject(string name, params System.Type[] componentTypes)
        {
            List<GameObject> addedGameObjects = new List<GameObject>();
            GameObject[] selectedGameObjects = Selection.gameObjects;
            if (selectedGameObjects.Length == 0)
            {
                GameObject gameObject = new GameObject(name);
                foreach (System.Type type in componentTypes)
                    gameObject.AddComponent(type);

                Undo.RegisterCreatedObjectUndo(gameObject, "Create " + gameObject.name);

                addedGameObjects.Add(gameObject);
            }
            else
            {
                foreach (GameObject selectedGameObject in selectedGameObjects)
                {
                    GameObject gameObject = new GameObject(name);
                    foreach (System.Type type in componentTypes)
                        gameObject.AddComponent(type);

                    gameObject.transform.SetParent(selectedGameObject.transform, false);

                    Undo.RegisterCreatedObjectUndo(gameObject, "Create " + gameObject.name);

                    addedGameObjects.Add(gameObject);
                }
                Selection.objects = selectedGameObjects = null;
            }

            return addedGameObjects;
        }
    }
}
