using System.Collections.Generic;
using Game.Scripts.Common.Enums;
using UnityEngine;

namespace Game.Scripts.Core
{
    public class CursorManager : MonoBehaviour
    {
        public static CursorManager Instance { get; private set; }
        
        private HashSet<CursorUnlockRequester> unlockRequests = new HashSet<CursorUnlockRequester>();
        
        public bool HasRequest(CursorUnlockRequester requester) => unlockRequests.Contains(requester);

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if(!hasFocus) 
                ApplyState();
            else
                ApplyState();
        }

        public void RequestLock(CursorUnlockRequester requester)
        {
            unlockRequests.Remove(requester);
            ApplyState();
        }
        
        public void RequestUnlock(CursorUnlockRequester requester)
        {
            unlockRequests.Add(requester);
            ApplyState();
        }

        private void ApplyState()
        {
            var shouldUnlock = unlockRequests.Count > 0;
            Cursor.lockState = shouldUnlock ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = shouldUnlock;
        }
    }
}