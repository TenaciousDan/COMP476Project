using UnityEngine;
using UnityEngine.SceneManagement;

using Tenacious.Scenes;

namespace Game.Scenes
{
    public class PreloadScene : MonoBehaviour
    {
        [SerializeField] private SceneReference nextScene;

        private void Awake()
        {
            // initializations go here
        }

        private void Start()
        {
            // load first Scene
            if (!Application.isEditor)
                SceneManager.LoadScene(nextScene);
        }
    }
}
