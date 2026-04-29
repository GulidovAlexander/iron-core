using System.Collections.Generic;
using Game.Scripts.Settings.UI.Base;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace Game.Scripts.Settings.UI.Graphics
{
  [RequireComponent(typeof(TMP_Dropdown))]
    public class FpsLimitDropdown : SettingControl
    {
        private const string TableName = "Settings Labels";
        private const string UnlimitedKey = "settingsMenu.displayTab.fpsLimit.unlimited";
        
        // -1 значит "без лимита"
        private static readonly int[] FpsValues = { -1, 30, 60, 75, 120, 144, 165, 240 };
        
        private TMP_Dropdown _dropdown;
        
        private void Awake()
        {
            _dropdown = GetComponent<TMP_Dropdown>();
        }
        
        protected override void Subscribe()
        {
            GameSettings.Graphics.OnFpsLimitChanged += OnExternalChange;
            LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
            _dropdown.onValueChanged.AddListener(OnUserChange);
        }
        
        protected override void Unsubscribe()
        {
            GameSettings.Graphics.OnFpsLimitChanged -= OnExternalChange;
            LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
            _dropdown.onValueChanged.RemoveListener(OnUserChange);
        }
        
        protected override void RefreshFromSettings()
        {
            RefreshOptions();
            RefreshSelection();
        }
        
        private void RefreshOptions()
        {
            string unlimitedText = LocalizationSettings.StringDatabase
                .GetLocalizedString(TableName, UnlimitedKey);
            
            var options = new List<TMP_Dropdown.OptionData>(FpsValues.Length);
            foreach (var fps in FpsValues)
            {
                string text = fps == -1 ? unlimitedText : fps.ToString();
                options.Add(new TMP_Dropdown.OptionData(text));
            }
            
            _dropdown.ClearOptions();
            _dropdown.AddOptions(options);
        }
        
        private void RefreshSelection()
        {
            int index = System.Array.IndexOf(FpsValues, GameSettings.Graphics.FpsLimit);
            if (index < 0) index = 0;
            _dropdown.SetValueWithoutNotify(index);
        }
        
        private void OnUserChange(int index)
        {
            if (index < 0 || index >= FpsValues.Length) return;
            GameSettings.Graphics.SetFpsLimit(FpsValues[index]);
        }
        
        private void OnExternalChange(int fps)
        {
            RefreshSelection();
        }
        
        private void OnLocaleChanged(Locale newLocale)
        {
            RefreshOptions();
            RefreshSelection();
        }
    }
}