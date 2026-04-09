using UnityEngine;

namespace Game.Scripts.Core
{
    public class CursorManager: MonoBehaviour
    {
        public static CursorManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public void Lock()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void Unlock()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}