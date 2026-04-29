using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace Game.Scripts.Settings.UI.Graphics
{
    [RequireComponent(typeof(Volume))]
    public class BrightnessApplier : MonoBehaviour
    {
        private Volume _volume;
        private ColorAdjustments _colorAdjustments;
        
        private void Awake()
        {
            _volume = GetComponent<Volume>();
            
            if (_volume.profile.TryGet(out _colorAdjustments) == false)
            {
                _colorAdjustments = _volume.profile.Add<ColorAdjustments>(true);
            }
        }
        
        private void OnEnable()
        {
            GameSettings.Graphics.OnBrightnessChanged += ApplyBrightness;
            ApplyBrightness(GameSettings.Graphics.Brightness);
        }
        
        private void OnDisable()
        {
            GameSettings.Graphics.OnBrightnessChanged -= ApplyBrightness;
        }
        
        private void ApplyBrightness(float value)
        {
            // value: 0.5..1.5 → postExposure: -1..+1 EV
            float exposure = (value - 1.0f) * 2.0f;
            _colorAdjustments.postExposure.value = exposure;
        }
    }
}