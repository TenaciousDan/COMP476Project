using UnityEngine;
using UnityEngine.UI;

namespace Tenacious.UI
{
    [RequireComponent(typeof(TButton))]
    public class UIButton : MonoBehaviour
    {
        [SerializeField] protected string label = "";

        public AudioClip hoverSound;
        public AudioClip clickSound;

        public bool enableHoverSound = false;
        public bool enableClickSound = false;

        protected TButton button;
        protected AudioSource audioSource;

        protected virtual void Awake()
        {
            if (audioSource == null) audioSource = this.GetComponent<AudioSource>();

            if (button == null) button = this.GetComponent<TButton>();
            button.OnStateChange += this.OnStateChange;
        }

        private void OnStateChange(TButton.State state, bool instant)
        {
            if (enableHoverSound && (state == TButton.State.Highlighted || state == TButton.State.Selected))
                audioSource?.PlayOneShot(hoverSound);
            else if (enableClickSound && state == TButton.State.Pressed)
                audioSource?.PlayOneShot(clickSound);
        }

        public string Label
        {
            get => label;
            set
            {
                label = value;

                this.GetComponentInChildren<Text>().text = label;
            }
        }
    }
}
