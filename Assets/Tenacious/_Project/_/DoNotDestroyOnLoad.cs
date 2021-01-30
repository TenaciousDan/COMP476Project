using UnityEngine;

using System;
using System.Collections.Generic;

namespace Tenacious
{
    public class DoNotDestroyOnLoad
    {
        public DoNotDestroyOnLoad(GameObject gameObject)
        {
            GameObject.DontDestroyOnLoad(gameObject);
        }

        private static GameObject ddolSpy;
        private static GameObject Spy
        {
            get
            {
                if (ddolSpy == null)
                {
                    ddolSpy = new GameObject("[DDOL " + Guid.NewGuid().ToString() + "]");
                    GameObject.DontDestroyOnLoad(ddolSpy);
                }

                return ddolSpy;
            }
        }

        public static List<GameObject> GetRootGameObjects()
        {
            Spy.transform.SetAsLastSibling();
            List<GameObject> gameObjects = new List<GameObject>(Spy.scene.GetRootGameObjects());
            gameObjects.RemoveAt(gameObjects.Count - 1);

            return gameObjects;
        }
    }
}