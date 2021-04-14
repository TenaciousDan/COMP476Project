#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.EventSystems;

using Tenacious.Scenes;
using Tenacious.UI.Windows;
using Tenacious;

namespace Game.UI.Menus
{
    public class GameplayOptionsMenu : MonoBehaviour
    {
        [SerializeField] private GameObject firstSelected;

        private void Awake()
        {
            if (firstSelected != null)
                EventSystem.current.SetSelectedGameObject(firstSelected);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!transform.GetChild(0).gameObject.activeSelf)
                    transform.GetChild(0).gameObject.SetActive(true);
                else
                    transform.GetChild(0).gameObject.SetActive(false);
            }
        }

        public void MainMenuClick()
        {
            SceneLoader.Instance.LoadScene("Welcome", "Box");
        }

        public void QuitBtnClick()
        {
            ConfirmWindow cw = (ConfirmWindow)SystemUI.Instance.WindowManager.OpenWindow("ConfirmWindow");
            cw.SetData("Are you sure you want to quit the game?", (ConfirmWindow self, bool result) =>
            {
                if (result)
                {
                    Application.Quit();
#if UNITY_EDITOR
                    EditorApplication.isPlaying = false;
#endif
                }
                else
                    SystemUI.Instance.WindowManager.CloseTopWindow();
            });
        }
    }
}