using UnityEngine;
using UnityEngine.EventSystems;

using System;
using UnityEngine.UI;

namespace Tenacious.UI.Windows
{
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(Image))]
    public class Window : MonoBehaviour
    {
        [Tooltip("The game object to focus on when this window is focused")]
        [SerializeField] public GameObject focusTarget;

        [Tooltip("WindowManager Properties")]
        [SerializeField] private WindowManager.Properties windowManagerProperties;
        public WindowManager.Properties WMProperties { get => windowManagerProperties; set => windowManagerProperties = value; }

        public Action<Window> OnShow { get; set; }
        public Action<Window> OnHide { get; set; }
        public Action<Window> OnFocus { get; set; }

        private Image overlay;

        protected virtual void Awake()
        {
            overlay = this.GetComponent<Image>();
        }

        protected virtual void Reset()
        {
            Overlay.enabled = false;
            Overlay.color = new Color(0, 0, 0, 0);
        }

        public virtual bool Interactable
        {
            get { return this.GetComponent<CanvasGroup>().interactable; }
            set { this.GetComponent<CanvasGroup>().interactable = value; }
        }

        public virtual void Show()
        {
            this.gameObject.SetActive(true);
            OnShow?.Invoke(this);
        }

        public virtual void Hide()
        {
            this.gameObject.SetActive(false);
            OnHide?.Invoke(this);
        }

        public virtual void Focus(EventSystem eventSystem = null)
        {
            if (eventSystem == null) eventSystem = EventSystem.current;

            eventSystem.SetSelectedGameObject(focusTarget);

            OnFocus?.Invoke(this);
        }

        public Image Overlay
        {
            get => overlay != null ? overlay : overlay = this.GetComponent<Image>();
        }

        public float OverlayOpacity
        {
            get => overlay.color.a;
            set => overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, value);
        }
    }
}
