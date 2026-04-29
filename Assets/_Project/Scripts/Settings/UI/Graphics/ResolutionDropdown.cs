using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Settings.UI.Base;
using TMPro;
using UnityEngine;

namespace Game.Scripts.Settings.UI.Graphics
{
    [RequireComponent(typeof(TMP_Dropdown))]
    public class ResolutionDropdown : SettingControl
    {
        private TMP_Dropdown _dropdown;
        private Resolution[] _resolutions;
        
        private void Awake()
        {
            _dropdown = GetComponent<TMP_Dropdown>();
        }
        
        protected override void Subscribe()
        {
            GameSettings.Graphics.OnResolutionChanged += OnExternalChange;
            _dropdown.onValueChanged.AddListener(OnUserChange);
        }
        
        protected override void Unsubscribe()
        {
            GameSettings.Graphics.OnResolutionChanged -= OnExternalChange;
            _dropdown.onValueChanged.RemoveListener(OnUserChange);
        }
        
        protected override void RefreshFromSettings()
        {
            _resolutions = GameSettings.Graphics.GetAvailableResolutions()
                .Distinct(new ResolutionComparer())
                .ToArray();
            
            var options = _resolutions
                .Select(r => $"{r.width} × {r.height} @ {(int)r.refreshRateRatio.value}Hz")
                .ToList();
            
            _dropdown.ClearOptions();
            _dropdown.AddOptions(options);
            
            int index = FindCurrentIndex();
            if (index < 0) index = 0;
            _dropdown.SetValueWithoutNotify(index);
        }
        
        private int FindCurrentIndex()
        {
            var current = GameSettings.Graphics.CurrentResolution;
            for (int i = 0; i < _resolutions.Length; i++)
            {
                if (_resolutions[i].width == current.width && 
                    _resolutions[i].height == current.height)
                    return i;
            }
            return -1;
        }
        
        private void OnUserChange(int index)
        {
            if (index < 0 || index >= _resolutions.Length) return;
            GameSettings.Graphics.SetResolution(_resolutions[index]);
        }
        
        private void OnExternalChange(Resolution resolution)
        {
            int index = FindCurrentIndex();
            if (index >= 0)
                _dropdown.SetValueWithoutNotify(index);
        }
        
        // Сравнивает разрешения без учёта частоты обновления (для DISTINCT)
        private class ResolutionComparer : IEqualityComparer<Resolution>
        {
            public bool Equals(Resolution a, Resolution b)
            {
                return a.width == b.width && a.height == b.height;
            }
            
            public int GetHashCode(Resolution r)
            {
                return r.width.GetHashCode() ^ r.height.GetHashCode();
            }
        }
    }
}