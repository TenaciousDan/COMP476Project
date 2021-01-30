using UnityEngine;
using UnityEngine.EventSystems;

using System;
using System.Collections.Generic;

namespace Tenacious.UI.Windows
{
    [RequireComponent(typeof(CanvasGroup))]
    public class WindowManager : MonoBehaviour
    {
        [Tooltip("The event system to use or EventSystem.current if null.")]
        [SerializeField] private EventSystem eventSystem;

        [Tooltip("When the last window is closed, return focus back to whatever had focus before any window was opened.")]
        public bool returnNativeSelection = true;

        [Tooltip("List of Window prefabs")]
        [SerializeField] private List<Window> windowPrefabList;

        private Dictionary<string, Window> windows;

        private Transform windowStackTransform;
        private Transform windowCacheTransform;

        private GameObject nativeSelectedGameObject;

        protected virtual void Awake()
        {
            if (eventSystem == null) eventSystem = EventSystem.current;
            if (windowPrefabList == null) windowPrefabList = new List<Window>();
            if (windows == null) windows = new Dictionary<string, Window>();
            
            windowStackTransform = this.transform.Find("Stack");
            windowStackTransform.gameObject.SetActive(true);
            windowCacheTransform = this.transform.Find("Cache");

            foreach (Window window in windowStackTransform.GetComponentsInChildren<Window>())
            {
                OpenWindow(window);
            }
        }

        protected virtual void Reset()
        {
            //
        }

        private T CreateWindowFromPrefab<T>(string name) where T : Window
        {
            T windowPrefab = GetWindowPrefab<T>(name);

            T window = null;
            if (windowPrefab != null)
            {
                window = Instantiate(windowPrefab, this.transform);
                window.name = windowPrefab.name;
            }
            else
                Debug.LogError("[" + nameof(WindowManager) + "] Cannot find prefab script for window {name: " + name + ", type: " + typeof(T) + "}");

            return window;
        }

        private T GetWindowPrefab<T>(string name) where T : Window
        {
            foreach (T windowPrefab in windowPrefabList)
            {
                if (windowPrefab != null && windowPrefab.name.Equals(name))
                    return windowPrefab.GetComponent<T>();
            }

            return null;
        }

        public Window this[string id]
        {
            get => windows.ContainsKey(id) ? windows[id] : null;
        }

        public Window this[int index]
        {
            get => index > 0 && index < windows.Count ? windowStackTransform.GetChild(index).GetComponent<Window>() : null;
        }

        public Window Front
        {
            get => windows.Count > 0 ? windowStackTransform.GetChild(windows.Count - 1).GetComponent<Window>() : null;
        }

        public Window Back
        {
            get => windows.Count > 0 ? windowStackTransform.GetChild(0).GetComponent<Window>() : null;
        }

        public int Count
        {
            get => windows.Count;
        }

        public Window OpenWindow(string name, Properties properties = null)
        {
            Window window = CreateWindowFromPrefab<Window>(name);
            return OpenWindow(window, properties);
        }
        public T OpenWindow<T>(T window, Properties properties = null) where T : Window
        {
            if (window == null) return null;

            if (windows.Count == 0)
                nativeSelectedGameObject = eventSystem.currentSelectedGameObject;

            if (properties != null)
                window.WMProperties = new Properties(window.name, properties);
            else
                window.WMProperties = new Properties(window.name, window.WMProperties);

            if (windows.Count > 0)
            {
                for (int i = windows.Count - 1; i >= 0; i--)
                {
                    Window win = windowStackTransform.GetChild(i).GetComponent<Window>();

                    if (window.WMProperties.hideWindowsUnderneath)
                        win.Hide();

                    if (win.WMProperties.hideWindowsUnderneath) break;
                }
            }

            window.transform.SetParent(windowStackTransform);
            windows.Add(window.WMProperties.WindowId, window);

            // make sure new window is drawn on top of previous one in the stack
            window.transform.SetAsLastSibling();

            window.gameObject.SetActive(true);
            window.WMProperties.OnOpen?.Invoke(window);
            window.Show();
            window.Focus();

            return window;
        }

        public void CloseAllWindows()
        {
            do CloseTopWindow();
            while (windows.Count > 0);
        }  
        public Window CloseTopWindow()
        {
            return Front != null ? CloseWindow(Front.WMProperties.WindowId) : null;
        }
        public Window CloseWindow(int index)
        {
            Window window = this[index];
            return window != null ? CloseWindow(window.WMProperties.WindowId) : null;
        }
        public Window CloseWindow(string id)
        {
            if (id == null || !windows.ContainsKey(id)) return null;

            if (windows.Count == 1 && returnNativeSelection)
                eventSystem.SetSelectedGameObject(nativeSelectedGameObject);

            Window window = windows[id];

            if (window.WMProperties.destroyWhenClosed)
                Destroy(window.gameObject);
            else
            {
                window.gameObject.SetActive(false);
                window.transform.SetParent(windowCacheTransform);
            }

            int index = window.transform.GetSiblingIndex() - 1;
            windows.Remove(window.WMProperties.WindowId);
            window.WMProperties.OnClose?.Invoke(window);

            if (window.gameObject.activeSelf && window.WMProperties.hideWindowsUnderneath)
            {
                for (int i = index; i >= 0; i--)
                {
                    Window win = windowStackTransform.GetChild(i).GetComponent<Window>();
                    if (!win.gameObject.activeSelf) win.Show();
                    if (win.WMProperties.hideWindowsUnderneath) break;
                }
            }

            if (windows.Count > 0)
                Front.Focus();

            return window;
        }

        public Window MoveForwards(string id)
        {
            if (id == null || !windows.ContainsKey(id)) return null;

            return MoveTo(id, windows[id].transform.GetSiblingIndex() + 1);
        }

        public Window MoveToFront(string id)
        {
            if (Front == null) return null;

            return MoveTo(id, Front.transform.GetSiblingIndex());
        }

        public Window MoveBackwards(string id)
        {
            if (id == null || !windows.ContainsKey(id)) return null;

            return MoveTo(id, windows[id].transform.GetSiblingIndex() - 1);
        }

        public Window MoveToBack(string id)
        {
            if (Back == null) return null;

            return MoveTo(id, Back.transform.GetSiblingIndex());
        }

        public Window MoveTo(string id, int newIndex)
        {
            if (id == null || !windows.ContainsKey(id)) return null;

            Window window = windows[id];
            Transform windowTransform = window.transform;

            if (newIndex > windowTransform.GetSiblingIndex())
            {
                // Moving forwards
                if (newIndex >= 0 && newIndex < windows.Count)
                {
                    Window surpassedWindow = windowStackTransform.GetChild(newIndex).GetComponent<Window>();
                    windowTransform.SetSiblingIndex(newIndex);

                    if (!window.gameObject.activeSelf)
                    {
                        if (surpassedWindow.gameObject.activeSelf)
                            window.Show();
                    }
                    
                    if (window.gameObject.activeSelf && window.WMProperties.hideWindowsUnderneath)
                    {
                        for (int i = newIndex - 1; i >= 0; i--)
                        {
                            Window win = windowStackTransform.GetChild(i).GetComponent<Window>();
                            if (win == window) continue;

                            win.Hide();

                            if (win.WMProperties.hideWindowsUnderneath) break;
                        }
                    }
                }
            }
            else if (newIndex < windowTransform.GetSiblingIndex())
            {
                // Moving backwards
                if (newIndex >= 0 && newIndex < windows.Count)
                {
                    Window surpassedWindow = windowStackTransform.GetChild(newIndex).GetComponent<Window>();
                    int oldIndex = windowTransform.GetSiblingIndex();
                    windowTransform.SetSiblingIndex(newIndex);

                    if (surpassedWindow.gameObject.activeSelf)
                    {
                        if (!surpassedWindow.WMProperties.hideWindowsUnderneath && !window.gameObject.activeSelf)
                            window.Show();
                        else if (surpassedWindow.WMProperties.hideWindowsUnderneath && window.gameObject.activeSelf)
                            window.Hide();
                    }
                    else if (!surpassedWindow.gameObject.activeSelf && window.gameObject.activeSelf)
                    {
                        window.Hide();
                    }

                    if (window.WMProperties.hideWindowsUnderneath)
                    {
                        for (int i = oldIndex; i >= 0; i--)
                        {
                            Window win = windowStackTransform.GetChild(i).GetComponent<Window>();

                            win.Show();

                            if (win.WMProperties.hideWindowsUnderneath) break;
                        }
                    }
                }
            }
            
            return window;
        }

        [Serializable]
        public class Properties
        {
            private string windowId;
            public string WindowId
            {
                get => windowId == null ? Guid.NewGuid().ToString() : windowId;
            }

            public Action<Window> OnOpen { get; set; }
            public Action<Window> OnClose { get; set; }

            [Tooltip("Destroy the GameObject when this window is closed")]
            [SerializeField] public bool destroyWhenClosed = true;

            [Tooltip("Hide windows that are under this one")]
            public bool hideWindowsUnderneath = false;

            private Properties() 
            {
                //
            }

            public Properties(string windowName) : this()
            {
                windowId = windowName + " (" + WindowId + ")";
            }

            public Properties(string windowName, Properties properties) : this(windowName)
            {
                OnOpen = properties.OnOpen;
                OnClose = properties.OnClose;
                destroyWhenClosed = properties.destroyWhenClosed;
                hideWindowsUnderneath = properties.hideWindowsUnderneath;
            }
        }
    }
}
