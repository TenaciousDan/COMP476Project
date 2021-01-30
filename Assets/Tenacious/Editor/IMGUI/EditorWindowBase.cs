using UnityEditor;

using UnityEngine;

namespace TenaciousEditor.IMGUI
{
    public class EditorWindowBase : EditorWindow
    {
        public EditorWindowBase() : base()
        {
            minSize = new Vector2(50, 50);
        }

        public virtual void OnGUI()
        {
            //
        }
    }
}
