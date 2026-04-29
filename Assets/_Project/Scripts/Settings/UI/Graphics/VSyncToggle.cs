using Game.Scripts.Settings.UI.Base;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.Settings.UI.Graphics
{
    [RequireComponent(typeof(Toggle))]
    public class VSyncToggle : SettingControl
    {
        private Toggle _toggle;
        
        private void Awake()
        {
            _toggle = GetComponent<Toggle>();
        }
        
        protected override void Subscribe()
        {
            GameSettings.Graphics.OnVSyncChanged += OnExternalChange;
            _toggle.onValueChanged.AddListener(OnUserChange);
        }
        
        protected override void Unsubscribe()
        {
            GameSettings.Graphics.OnVSyncChanged -= OnExternalChange;
            _toggle.onValueChanged.RemoveListener(OnUserChange);
        }
        
        protected override void RefreshFromSettings()
        {
            _toggle.SetIsOnWithoutNotify(GameSettings.Graphics.VSyncEnabled);
        }
        
        private void OnUserChange(bool isOn)
        {
            GameSettings.Graphics.SetVSync(isOn);
        }
        
        private void OnExternalChange(bool isOn)
        {
            _toggle.SetIsOnWithoutNotify(isOn);
        }
    }
}