using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance {get; private set;}
    
    [Header("Panel Settings Tabs")]
    [SerializeField] private GameObject _graphicsTabPanel;
    [SerializeField] private GameObject _audioTabPanel;
    [SerializeField] private GameObject _controlsTabPanel;
    [SerializeField] private GameObject _gameplayTabPanel;
    
    [Header("Settings Item Display Tab")]
    [SerializeField] private TMP_Dropdown _languageDropdown;
    [SerializeField] private TMP_Dropdown _displayDropdown;
    [SerializeField] private TMP_Dropdown _windowModeDropdown;
    [SerializeField] private Toggle _hdrToggle;

    [Header("Settings Item Quality Tab")]
    
    [Header("Settings Item Audio Tab")]
    
    [Header("Settings Item Gameplay Tab")]
    
    [Header("Settings Item Controls Tab")]
    
    
    
    private List<DisplayInfo> _displays = new();
    
    private Camera _mainCamera;
    
    private readonly string[] _modeKeys =
    {
        "settingsMenu.displayTab.windowMode.fullScreen",
        "settingsMenu.displayTab.windowMode.borderless",
        "settingsMenu.displayTab.windowMode.windowed"
    };
    
    private List<(int, GameObject)> indexSettingsTab = new List<(int, GameObject)>();


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        _mainCamera = Camera.main;
        
        if (LocalizationSettings.InitializationOperation.IsDone)
        {
            SetupLanguageDropdown();
            SetupHDRToggle();
            SetupDisplayDropdown();
            SetupWindowModeDropdown();
        }
        else
        {
            LocalizationSettings.InitializationOperation.Completed += OnInitializationCompleted;
        }
    }
    
    private void OnInitializationCompleted(AsyncOperationHandle<LocalizationSettings> obj)
    {
        SetupLanguageDropdown();
        SetupHDRToggle();
        SetupDisplayDropdown();
        SetupWindowModeDropdown();

    }

    private void SetupWindowModeDropdown()
    {
        if (_windowModeDropdown.options.Count != 3)
        {
            Debug.LogError("WindowModeDropdown.options count != 3");
            return;
        }

        ApplyLocalizationWindowModeDropdown();
        
        _windowModeDropdown.onValueChanged.AddListener(OnWindowModeChanged);
    }

    private void OnWindowModeChanged(int index)
    {
    }

    private void ApplyLocalizationWindowModeDropdown()
    {
        for (int i = 0; i < _modeKeys.Length && i < _windowModeDropdown.options.Count; i++)
        {
            string localizedText = LocalizationSettings.StringDatabase.GetLocalizedString("Settings Labels", _modeKeys[i]);
        
            _windowModeDropdown.options[i].text = localizedText;
        }
    
        _windowModeDropdown.captionText.text = _windowModeDropdown.options[_windowModeDropdown.value].text;
    }

    private void SetupDisplayDropdown()
    {
        Screen.GetDisplayLayout(_displays);
        
        var options = _displays.Select(d => d.name).ToList();
        
        _displayDropdown.ClearOptions();
        _displayDropdown.AddOptions(options);
        _displayDropdown.onValueChanged.AddListener(OnMonitorSelected);
    }

    private void OnMonitorSelected(int index)
    {
        if(index < 0 || index >= _displays.Count) return;
        
        #if UNITY_STANDALONE
        Screen.MoveMainWindowTo(_displays[index], Vector2Int.zero);
        #else
        Debug.LogWarning("MoveMainWindowTo works only is Standalone builds");       
        #endif
    }

    

    private void SetupHDRToggle()
    {
        _hdrToggle.onValueChanged.AddListener(OnToggleChanged);
    }

    private void OnToggleChanged(bool isOn)
    {
        ApplyHDR(isOn);
        PlayerPrefs.SetInt("HDR_Enabled", isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void ApplyHDR(bool enable)
    {
        if (_mainCamera == null) return;
        
        _mainCamera.allowHDR = enable;
    }

    private void SetupLanguageDropdown()
    {
        var locales = LocalizationSettings.AvailableLocales.Locales;
        var options = locales.Select(locale => locale.Identifier.CultureInfo?.NativeName ?? locale.Identifier.Code).ToList();

        _languageDropdown.ClearOptions();
        _languageDropdown.AddOptions(options);
        _languageDropdown.value = GetCurrentLanguageIndex();
        _languageDropdown.onValueChanged.AddListener(OnLanguageChanged);
    }

    private int GetCurrentLanguageIndex()
    {
        var current = LocalizationSettings.SelectedLocale;
        return LocalizationSettings.AvailableLocales.Locales.IndexOf(current);
    }

    private void OnLanguageChanged(int index)
    {
        var selectedLanguage = LocalizationSettings.AvailableLocales.Locales[index];
        LocalizationSettings.SelectedLocale = selectedLanguage;

        PlayerPrefs.SetString("SelectedLanguage", selectedLanguage.Identifier.Code);
        PlayerPrefs.Save();
    }

    private void OnDestroy()
    {
        LocalizationSettings.InitializationOperation.Completed -= OnInitializationCompleted;
        
        _hdrToggle.onValueChanged.RemoveListener(OnToggleChanged);
        _displayDropdown.onValueChanged.RemoveListener(OnMonitorSelected);
        _windowModeDropdown.onValueChanged.RemoveListener(OnWindowModeChanged);
    }

    // private void SetActiveSettingsTab(int index)
    // {
    //     SetActiveSettingsTab(index, true);
    // }

    // private void AllDeactiveSettingsTabs()
    // {
    //     for (var index = 0; index < settingsTabs.Count; index++)
    //     {
    //         SetActiveSettingsTab(index,false);   
    //     }
    // }
    //
    // private void SetActiveSettingsTab(int index, bool active)
    // {
    //     settingsTabs[index].gameObject.SetActive(active);
    // }
}
