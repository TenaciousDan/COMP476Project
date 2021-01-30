using UnityEngine;

namespace Tenacious.Utilities
{
    public static class MathUtil
    {
        public static float Round(float value, int decimalPlaces = 0)
        {
            decimalPlaces = decimalPlaces >= 0 ? decimalPlaces : 0;
            float mult = Mathf.Pow(10f, (float)decimalPlaces);
            return Mathf.Round(value * mult) / mult;
        }
    }
}
