using Tenacious.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Tenacious.UI
{
    [ExecuteInEditMode]
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField] private Image filledImage;
        [SerializeField] private Text progressText;
        [SerializeField] private string progressUnits;
        [SerializeField, Min(0)] private int progressPrecision = 0;

        public float minimum = 0;
        public float maximum = 100;
        public float current = 0;

        protected virtual void Update()
        {
            updateFillAmount();
        }

        private void updateFillAmount()
        {
            float currentOffset = current - minimum;
            float maximumOffeset = maximum - minimum;

            if (filledImage != null)
                filledImage.fillAmount = Mathf.Clamp(currentOffset / maximumOffeset, 0, 1);

            if (progressText != null)
                progressText.text = MathUtil.Round(current, progressPrecision) + " " + progressUnits;
        }
    }
}