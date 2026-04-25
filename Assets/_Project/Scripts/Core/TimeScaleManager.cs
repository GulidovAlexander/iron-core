using System.Collections.Generic;
using Game.Scripts.Common.Enums;
using UnityEngine;

namespace Game.Scripts.Core
{
    public class TimeScaleManager: MonoBehaviour
    {
        public static TimeScaleManager Instance { get; private set; }
        
        private HashSet<TimeScaleRequester> freezeRequests = new HashSet<TimeScaleRequester>();
        private float originalTimeScale = 1f;
        public bool IsFrozen => freezeRequests.Count > 0;

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

        public void RequestFreeze(TimeScaleRequester requester)
        {
            freezeRequests.Add(requester);
            ApplyTimeScale();
        }

        public void RequestUnfreeze(TimeScaleRequester requester)
        {
            freezeRequests.Remove(requester);
            ApplyTimeScale();
        }
        
        private void ApplyTimeScale()
        {
            Time.timeScale = freezeRequests.Count > 0 ? 0f : originalTimeScale;
        }
    }
}