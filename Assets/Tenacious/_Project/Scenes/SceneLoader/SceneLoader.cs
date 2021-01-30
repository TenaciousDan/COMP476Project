using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

using Tenacious.Shaders;

namespace Tenacious.Scenes
{
    public sealed class SceneLoader : MBSingleton<SceneLoader>
    {
        [SerializeField] private string loadingSceneName = "Loading";
        [SerializeField] private GameObject transitionsObject;
        [SerializeField] private GameObject imageAlphaMaskObject;
        [SerializeField] private GameObject fadeObject;
        [SerializeField] private List<Texture> transitionTextures;

        public const string NO_TRANSITION = null;
        public const string RANDOM_TRANSITION = "";
        public const string FADE_TRANSITION = "___FADE___";

        public enum ETransitionPhase { None, Out, In }

        public delegate void OnBeforeSceneLoadCallback();
        public event Action<float> OnLoadProgressUpdate;

        private Dictionary<string, Texture2D> textureMasks;
        private string sceneToLoad;
        private OnBeforeSceneLoadCallback beforeSceneLoadCallback;

        private string transition;
        private ETransitionPhase transitionPhase;

        private SceneLoader() { }

        protected override void Awake()
        {
            base.Awake();

            textureMasks = new Dictionary<string, Texture2D>();
            foreach (Texture texture in transitionTextures)
                textureMasks.Add(texture.name, (Texture2D) texture);

            transitionsObject.SetActive(false);
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (scene.name.Equals(loadingSceneName))
                transitionsObject.SetActive(false);
        }

        public void LoadScene(string sceneName, string transition = NO_TRANSITION, OnBeforeSceneLoadCallback onBeforeSceneLoadCallback = null)
        {
            sceneToLoad = sceneName;
            beforeSceneLoadCallback = onBeforeSceneLoadCallback;

            transitionsObject.SetActive(true);
            Transition = transition;
            TransitionPhase = ETransitionPhase.Out;

            if (Transition == NO_TRANSITION)
            {
                OnOutComplete();
                OnInComplete();
            }
        }

        public void OnOutComplete()
        {
            if (!string.IsNullOrWhiteSpace(sceneToLoad))
            {
                beforeSceneLoadCallback?.Invoke();

                if (SceneUtility.GetBuildIndexByScenePath(loadingSceneName) >= 0)
                    SceneManager.LoadScene(loadingSceneName);

                StartCoroutine(CRLoadSceneInBackground(sceneToLoad));
            }
            else
            {
                beforeSceneLoadCallback?.Invoke();
                TransitionPhase = ETransitionPhase.In;
            }

            sceneToLoad = null;
            beforeSceneLoadCallback = null;
        }

        public void OnInComplete()
        {
            transitionsObject.SetActive(false);
            Transition = NO_TRANSITION;
            TransitionPhase = ETransitionPhase.None;
        }

        public string Transition
        {
            get { return transition; }
            set
            {
                string val = value;
                if (val != null && val.Trim().Equals(RANDOM_TRANSITION) && textureMasks.Count > 0)
                {
                    string[] extra_transitions = { FADE_TRANSITION };
                    int rand = UnityEngine.Random.Range(0, textureMasks.Count + extra_transitions.Length);
                    if (rand >= textureMasks.Count)
                        val = extra_transitions[textureMasks.Count - rand];
                    else
                        val = new List<string>(textureMasks.Keys)[rand];
                }

                if (transition != val)
                    transition = val;
            }
        }

        public ETransitionPhase TransitionPhase
        {
            get { return transitionPhase; }
            set
            {
                if (transitionPhase != value)
                {
                    transitionPhase = value;

                    StopCoroutine(CRTransitionOut());
                    StopCoroutine(CRTransitionIn());

                    if (value == ETransitionPhase.Out)
                        StartCoroutine(CRTransitionOut());
                    else if (value == ETransitionPhase.In)
                        StartCoroutine(CRTransitionIn());
                }
            }
        }

        private IEnumerator CRTransitionOut()
        {
            if (Transition != NO_TRANSITION && Transition.Equals(FADE_TRANSITION))
            {
                Image imgFade = fadeObject.GetComponent<Image>();
                imgFade.enabled = true;
                imgFade.color = new Color(imgFade.color.r, imgFade.color.g, imgFade.color.b, 0f);
                while (TransitionPhase == ETransitionPhase.Out && imgFade.color.a < 1f)
                {
                    imgFade.color = new Color(imgFade.color.r, imgFade.color.g, imgFade.color.b, imgFade.color.a + (Time.unscaledDeltaTime * 1.5f));
                    yield return null;
                }
            }
            else
            {
                imageAlphaMaskObject.GetComponent<Image>().enabled = true;
                ImageAlphaMaskShaderController imageAlphaMaskController = imageAlphaMaskObject.GetComponent<ImageAlphaMaskShaderController>();
                imageAlphaMaskController.MaskValue = 0f;

                if (Transition != NO_TRANSITION && textureMasks.ContainsKey(Transition))
                {
                    imageAlphaMaskController.MaskTexture = textureMasks[Transition];
                    while (TransitionPhase == ETransitionPhase.Out && imageAlphaMaskController.MaskValue < 1f)
                    {
                        imageAlphaMaskController.MaskValue = Mathf.Clamp(imageAlphaMaskController.MaskValue + (Time.unscaledDeltaTime * 1.5f), 0f, 1f);
                        yield return null;
                    }
                }
            }

            OnOutComplete();
        }

        private IEnumerator CRTransitionIn()
        {
            if (Transition != NO_TRANSITION && Transition.Equals(FADE_TRANSITION))
            {
                Image imgFade = fadeObject.GetComponent<Image>();
                imgFade.enabled = true;
                imgFade.color = new Color(imgFade.color.r, imgFade.color.g, imgFade.color.b, 1f);
                while (TransitionPhase == ETransitionPhase.In && imgFade.color.a > 0f)
                {
                    imgFade.color = new Color(imgFade.color.r, imgFade.color.g, imgFade.color.b, imgFade.color.a - (Time.unscaledDeltaTime * 1.5f));
                    yield return null;
                }

                imgFade.enabled = false;
            }
            else
            {
                imageAlphaMaskObject.GetComponent<Image>().enabled = true;
                ImageAlphaMaskShaderController imageAlphaMaskController = imageAlphaMaskObject.GetComponent<ImageAlphaMaskShaderController>();
                imageAlphaMaskController.MaskValue = 1f;

                if (Transition != NO_TRANSITION && textureMasks.ContainsKey(Transition))
                {
                    imageAlphaMaskController.MaskTexture = textureMasks[Transition];
                    while (TransitionPhase == ETransitionPhase.In && imageAlphaMaskController.MaskValue > 0f)
                    {
                        imageAlphaMaskController.MaskValue = Mathf.Clamp(imageAlphaMaskController.MaskValue - (Time.unscaledDeltaTime * 1.5f), 0f, 1f);
                        yield return null;
                    }
                }

                imageAlphaMaskObject.GetComponent<Image>().enabled = false;
            }

            OnInComplete();
        }

        private IEnumerator CRLoadSceneInBackground(string sceneName)
        {
            yield return null; // continue running on the next frame

            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
            asyncOperation.allowSceneActivation = false;

            while (!asyncOperation.isDone)
            {
                float progress = asyncOperation.progress + 0.1f;

                OnLoadProgressUpdate?.Invoke(progress);

                // activate the new scene when progress is complete 
                if (progress >= 1f)
                {
                    transitionsObject.SetActive(true);
                    asyncOperation.allowSceneActivation = true;
                }

                yield return null;
            }

            OnLoadProgressUpdate = null;
            TransitionPhase = ETransitionPhase.In;
        }
    }
}