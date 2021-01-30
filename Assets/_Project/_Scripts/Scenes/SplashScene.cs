using UnityEngine;
using UnityEngine.UI;

using Tenacious.Scenes;

namespace Game.Scenes
{
    public class SplashScene : MonoBehaviour
    {
        [SerializeField] private Image logo;

        private void Awake()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = false;
        }

        public void GoToNextScene()
        {
            SceneLoader.Instance.LoadScene("Welcome", "Box");
        }
    }
}