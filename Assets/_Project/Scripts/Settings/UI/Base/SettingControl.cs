using UnityEngine;

namespace Game.Scripts.Settings.UI.Base
{
    public abstract class SettingControl : MonoBehaviour
    {
        protected virtual void OnEnable()
        {
            if (!GameSettings.IsInitialized)
            {
                GameSettings.Initialize();
            }
            
            Subscribe();
            RefreshFromSettings();
        }
        
        protected virtual void OnDisable()
        {
            Unsubscribe();
        }
        
        protected abstract void Subscribe();
        protected abstract void Unsubscribe();
        protected abstract void RefreshFromSettings();
    }
}