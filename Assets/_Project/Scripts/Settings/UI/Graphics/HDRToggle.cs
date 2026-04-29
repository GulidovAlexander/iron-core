using Game.Scripts.Settings;
using Game.Scripts.Settings.UI.Base;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Settings.UI.Graphics
{
    [RequireComponent(typeof(Toggle))]
    public class HDRToggle : SettingControl
    {
        private Toggle _toggle;
        
        private void Awake()
        {
            _toggle = GetComponent<Toggle>();
        }
        
        protected override void Subscribe()
        {
            GameSettings.Graphics.OnHDRChanged += OnExternalChange;
            _toggle.onValueChanged.AddListener(OnUserChange);
        }
        
        protected override void Unsubscribe()
        {
            GameSettings.Graphics.OnHDRChanged -= OnExternalChange;
            _toggle.onValueChanged.RemoveListener(OnUserChange);
        }
        
        protected override void RefreshFromSettings()
        {
            _toggle.SetIsOnWithoutNotify(GameSettings.Graphics.HDREnabled);
        }
        
        private void OnUserChange(bool isOn)
        {
            GameSettings.Graphics.SetHDR(isOn);
        }
        
        private void OnExternalChange(bool isOn)
        {
            _toggle.SetIsOnWithoutNotify(isOn);
        }
    }
}