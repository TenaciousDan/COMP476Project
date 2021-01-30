using UnityEngine;

using System;

namespace Tenacious.UI
{
    public class TButton : UnityEngine.UI.Button
    {
        public enum State {
            Normal = 0,
            Highlighted = 1,
            Pressed = 2,
            Selected = 3,
            Disabled = 4
        }

        public Action<State, bool> OnStateChange;

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            OnStateChange?.Invoke((State)state, instant);

            base.DoStateTransition(state, instant);
        }
    }
}
