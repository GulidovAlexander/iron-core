using System.Collections.Generic;
using Game.Scripts.Settings;
using Game.Scripts.Settings.Modules;
using Game.Scripts.Settings.UI.Base;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace Game.Settings.UI.Graphics
{
    [RequireComponent(typeof(TMP_Dropdown))]
    public class GraphicsApiDropdown : SettingControl
    {
        private const string TableName = "Settings Labels";
        private const string AutoKey = "settingsMenu.displayTab.graphicsApi.auto";
        
        private TMP_Dropdown _dropdown;
        private List<GraphicsApi> _availableApis;
        
        private void Awake()
        {
            _dropdown = GetComponent<TMP_Dropdown>();
            BuildAvailableApiList();
        }
        
        protected override void Subscribe()
        {
            GameSettings.Graphics.OnGraphicsApiChanged += OnExternalChange;
            LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
            _dropdown.onValueChanged.AddListener(OnUserChange);
        }
        
        protected override void Unsubscribe()
        {
            GameSettings.Graphics.OnGraphicsApiChanged -= OnExternalChange;
            LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
            _dropdown.onValueChanged.RemoveListener(OnUserChange);
        }
        
        protected override void RefreshFromSettings()
        {
            RefreshOptions();
            RefreshSelection();
        }
        
        private void BuildAvailableApiList()
        {
            _availableApis = new List<GraphicsApi> { GraphicsApi.Auto };
            
#if UNITY_STANDALONE_WIN
            _availableApis.Add(GraphicsApi.Direct3D11);
            _availableApis.Add(GraphicsApi.Direct3D12);
            _availableApis.Add(GraphicsApi.Vulkan);
#elif UNITY_STANDALONE_LINUX
            _availableApis.Add(GraphicsApi.Vulkan);
            _availableApis.Add(GraphicsApi.OpenGLCore);
#elif UNITY_STANDALONE_OSX
            _availableApis.Add(GraphicsApi.Metal);
#endif
        }
        
        private void RefreshOptions()
        {
            string autoText = LocalizationSettings.StringDatabase
                .GetLocalizedString(TableName, AutoKey);
            
            var options = new List<TMP_Dropdown.OptionData>();
            foreach (var api in _availableApis)
            {
                string text = api == GraphicsApi.Auto ? autoText : api.ToString();
                options.Add(new TMP_Dropdown.OptionData(text));
            }
            
            _dropdown.ClearOptions();
            _dropdown.AddOptions(options);
        }
        
        private void RefreshSelection()
        {
            int index = _availableApis.IndexOf(GameSettings.Graphics.PreferredGraphicsApi);
            if (index < 0) index = 0;
            _dropdown.SetValueWithoutNotify(index);
        }
        
        private void OnUserChange(int index)
        {
            if (index < 0 || index >= _availableApis.Count) return;
            GameSettings.Graphics.SetGraphicsApi(_availableApis[index]);
        }
        
        private void OnExternalChange(GraphicsApi api)
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