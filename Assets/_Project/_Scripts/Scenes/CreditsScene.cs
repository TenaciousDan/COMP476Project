using UnityEngine;

using Tenacious.Scenes;

namespace Game.Scenes
{
    public class CreditsScene : MonoBehaviour
    {
        [SerializeField] private RectTransform scrollingContent;
        [SerializeField] [Min(0)] private float movementSpeed = 50f;

        private float scrollSpeed;

        private void Awake()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = false;
            scrollSpeed = movementSpeed;
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

            scrollingContent.transform.Translate(0, scrollSpeed * Time.deltaTime, 0);

            if (scrollingContent.anchoredPosition.y > scrollingContent.rect.height)
                SceneLoader.Instance.LoadScene("Welcome", SceneLoader.FADE_TRANSITION);
        }
    }
}
