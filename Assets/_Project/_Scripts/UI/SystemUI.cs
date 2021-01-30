using UnityEngine;

using Tenacious;
using Tenacious.UI.Windows;

namespace Game.UI
{
    public class SystemUI : MBSingleton<SystemUI>
    {
        protected SystemUI() { }

        public WindowManager WindowManager
        {
            get { return this.GetComponentInChildren<WindowManager>(); }
        }
    }
}