using System.Collections.Generic;
using Game.Scripts.Settings.UI.Base;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace Game.Scripts.Settings.UI.Graphics
{
    [RequireComponent(typeof(TMP_Dropdown))]
    public class WindowModeDropdown: SettingControl
    {
       private const string TableName = "Settings Labels";
        
        private static readonly string[] LocalizationKeys =
        {
            "settingsMenu.displayTab.windowMode.fullScreen",
            "settingsMenu.displayTab.windowMode.borderless",
            "settingsMenu.displayTab.windowMode.windowed"
        };
        
        private static readonly FullScreenMode[] Modes =
        {
            FullScreenMode.ExclusiveFullScreen,
            FullScreenMode.FullScreenWindow,
            FullScreenMode.Windowed
        };
        
        private TMP_Dropdown _dropdown;
        
        private void Awake()
        {
            _dropdown = GetComponent<TMP_Dropdown>();
        }
        
        protected override void Subscribe()
        {
            GameSettings.Graphics.OnWindowModeChanged += OnExternalChange;
            LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
            _dropdown.onValueChanged.AddListener(OnUserChange);
        }
        
        protected override void Unsubscribe()
        {
            GameSettings.Graphics.OnWindowModeChanged -= OnExternalChange;
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
            var options = new List<TMP_Dropdown.OptionData>(LocalizationKeys.Length);
            foreach (var key in LocalizationKeys)
            {
                var text = LocalizationSettings.StringDatabase.GetLocalizedString(TableName, key);
                options.Add(new TMP_Dropdown.OptionData(text));
            }
            
            _dropdown.ClearOptions();
            _dropdown.AddOptions(options);
        }
        
        private void RefreshSelection()
        {
            int index = System.Array.IndexOf(Modes, GameSettings.Graphics.WindowMode);
            if (index < 0) index = 0;
            
            _dropdown.SetValueWithoutNotify(index);
            _dropdown.RefreshShownValue();
        }
        
        private void OnUserChange(int index)
        {
            if (index < 0 || index >= Modes.Length) return;
            GameSettings.Graphics.SetWindowMode(Modes[index]);
        }
        
        private void OnExternalChange(FullScreenMode mode)
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