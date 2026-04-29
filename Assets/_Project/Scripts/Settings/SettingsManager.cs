using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance {get; private set;}
    
    [SerializeField] private TMP_Dropdown _languageDropdown;

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
        if (LocalizationSettings.InitializationOperation.IsDone)
        {
            SetupLanguageDropdown();
        }
        else
        {
            LocalizationSettings.InitializationOperation.Completed += OnInitializationCompleted;
        }
    }

    private void OnInitializationCompleted(AsyncOperationHandle<LocalizationSettings> obj)
    {
        SetupLanguageDropdown();
    }

    private void SetupLanguageDropdown()
    {
        var locales = LocalizationSettings.AvailableLocales.Locales;
        var options = new List<string>();

        foreach (var locale in locales)
        {
            var displayName = locale.Identifier.CultureInfo?.NativeName ?? locale.Identifier.Code;
            
            options.Add(displayName);
        }
        
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
    }
}
