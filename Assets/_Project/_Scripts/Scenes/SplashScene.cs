using UnityEngine;
using UnityEngine.UI;

using Tenacious.Scenes;
using Tenacious.Audio;

namespace Game.Scenes
{
    public class SplashScene : MonoBehaviour
    {
        [SerializeField] private Image logo;

        private void Awake()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = false;

            AudioManager.Instance.PlayMusic("Intro");
        }

        public void GoToNextScene()
        {
            SceneLoader.Instance.LoadScene("Welcome", "Box");
        }
    }
}