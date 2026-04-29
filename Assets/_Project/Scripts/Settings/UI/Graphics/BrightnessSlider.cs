using Game.Scripts.Settings;
using Game.Scripts.Settings.UI.Base;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Settings.UI.Graphics
{
    [RequireComponent(typeof(Slider))]
    public class BrightnessSlider : SettingControl
    {
        private Slider _slider;
        
        private void Awake()
        {
            _slider = GetComponent<Slider>();
            _slider.minValue = 0.5f;
            _slider.maxValue = 1.5f;
            _slider.wholeNumbers = false;
        }
        
        protected override void Subscribe()
        {
            GameSettings.Graphics.OnBrightnessChanged += OnExternalChange;
            _slider.onValueChanged.AddListener(OnUserChange);
        }
        
        protected override void Unsubscribe()
        {
            GameSettings.Graphics.OnBrightnessChanged -= OnExternalChange;
            _slider.onValueChanged.RemoveListener(OnUserChange);
        }
        
        protected override void RefreshFromSettings()
        {
            _slider.SetValueWithoutNotify(GameSettings.Graphics.Brightness);
        }
        
        private void OnUserChange(float value)
        {
            GameSettings.Graphics.SetBrightness(value);
        }
        
        private void OnExternalChange(float value)
        {
            _slider.SetValueWithoutNotify(value);
        }
    }
}