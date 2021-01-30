using UnityEngine;

namespace Tenacious.Utilities
{
    public static class CommonUtil
    {
        public static string[] GetConnectedJoysticks()
        {
            string[] jsNames = Input.GetJoystickNames();

            for (int i = 0; i < jsNames.Length; ++i)
            {
                if (string.IsNullOrEmpty(jsNames[i]))
                    jsNames[i] = null;
            }

            return jsNames;
        }

        public static int GetNumberOfConnectedJoysticks()
        {
            string[] jsNames = GetConnectedJoysticks();
            int connectedJoysticks = 0;

            for (int i = 0; i < jsNames.Length; ++i)
            {
                if (jsNames[i] != null)
                    connectedJoysticks++;
            }

            return connectedJoysticks;
        }
    }
}
