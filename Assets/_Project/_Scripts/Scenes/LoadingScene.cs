using UnityEngine;

using Tenacious.Scenes;
using Tenacious.UI;

namespace Game.Scenes
{
    public class LoadingScene : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private ProgressBar progressBar;

        [Tooltip("Time to wait until Canvas is shown")]
        [SerializeField] private float canvasShowTime = 1;
        private float canvasTimer;

        private void Awake()
        {
            if (progressBar != null)
                progressBar.current = 0;

            SceneLoader.Instance.OnLoadProgressUpdate += OnLoadProgressUpdate;

            canvas.gameObject.SetActive(false);
            canvasTimer = 0;
        }

        private void Update()
        {
            canvasTimer += Time.deltaTime;
            if (canvasTimer >= canvasShowTime)
                canvas.gameObject.SetActive(true);
        }

        private void OnLoadProgressUpdate(float progress)
        {
            if (progressBar != null)
                progressBar.current = progress * 100;
        }
    }
}