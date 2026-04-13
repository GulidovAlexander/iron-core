using UnityEngine;

namespace Game.Scripts.Core
{
    public class CursorManager : MonoBehaviour
    {
        public static CursorManager Instance { get; private set; }

        private bool isLocked;

        private void Awake()
        {
            Instance = this;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
                Unlock();
            else
                SetCursor(isLocked);
        }

        public void Lock()
        {
            isLocked = true;
            SetCursor(true);
        }

        public void Unlock()
        {
            isLocked = false;
            SetCursor(false);
        }

        private void SetCursor(bool locked)
        {
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !locked;
        }
    }
}