using UnityEngine;
using UnityEngine.UI;

namespace Tenacious.UI.Windows
{
    public class ConfirmWindow : Window
    {
        [SerializeField] public RectTransform rootPanelTransform;
        [SerializeField] public Text txtMessage;
        [SerializeField] public Text txtConfirm;
        [SerializeField] public Text txtCancel;

        public const float DEFAULT_WIDTH = 600f;
        public const string DEFAULT_CONFIRM_STRING = "CONFIRM";
        public const string DEFAULT_CANCEL_STRING = "CANCEL";

        public delegate void Callback(ConfirmWindow self, bool result);
        private Callback callback;

        public void SetData(string message, Callback callback, float width = DEFAULT_WIDTH, 
            string strConfirm = DEFAULT_CONFIRM_STRING, string strCancel = DEFAULT_CANCEL_STRING)
        {
            txtMessage.text = message;
            this.callback = callback != null ? callback : (ConfirmWindow s, bool r) => { };
            txtConfirm.text = strConfirm;
            txtCancel.text = strCancel;

            rootPanelTransform.sizeDelta = new Vector2(width, rootPanelTransform.sizeDelta.y);
        }

        public void ConfirmClick()
        {
            callback(this, true);
        }

        public void CancelClick()
        {
            callback(this, false);
        }
    }
}
