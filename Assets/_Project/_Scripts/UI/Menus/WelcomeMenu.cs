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
    public class WelcomeMenu : MonoBehaviour
    {
        [SerializeField] private GameObject firstSelected;

        private void Awake()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (firstSelected != null)
                EventSystem.current.SetSelectedGameObject(firstSelected);
        }

        public void PlayBtnClick()
        {
            // TODO
        }

        public void CreditsBtnClick()
        {
            SceneLoader.Instance.LoadScene("Credits", SceneLoader.FADE_TRANSITION);
        }

        public void QuitBtnClick()
        {
            ConfirmWindow cw = (ConfirmWindow)SystemUI.Instance.WindowManager.OpenWindow("ConfirmWindow");
            cw.SetData("Are you sure you want to quit ?", (ConfirmWindow dialog, bool result) =>
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