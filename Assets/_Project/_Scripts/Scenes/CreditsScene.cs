using UnityEngine;

using Tenacious.Scenes;
using Tenacious.Audio;

namespace Game.Scenes
{
    public class CreditsScene : MonoBehaviour
    {
        [SerializeField] private RectTransform scrollingContent;
        [SerializeField] [Min(0)] private float movementSpeed = 50f;

        private float scrollSpeed;

        private float thankYouTimer;

        private void Awake()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = false;
            scrollSpeed = movementSpeed;
            thankYouTimer = 3;
        }

        private void Start()
        {
            scrollingContent.anchoredPosition = new Vector2(scrollingContent.anchoredPosition.x, -scrollingContent.parent.GetComponent<RectTransform>().rect.height);
        }

        private void alterScrollSpeed(float speedFactor = 1)
        {
            scrollSpeed = movementSpeed * speedFactor;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)) alterScrollSpeed(3);
            if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.Return)) alterScrollSpeed();

            if (scrollingContent.anchoredPosition.y >= 4150)//scrollingContent.rect.height)
            {
                if (thankYouTimer > 0)
                    thankYouTimer -= Time.deltaTime;
                else
                    SceneLoader.Instance.LoadScene("Welcome", SceneLoader.FADE_TRANSITION);
            }
            else
                scrollingContent.transform.Translate(0, scrollSpeed * Time.deltaTime, 0);
        }
    }
}
