using System;
using Game.Scripts.Settings.Storage;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace Game.Scripts.Settings.Modules
{
    public class LaguageSettings
    {
        private const string TEXT_LAGUAGE_SETTINGS_KEY = "Laguage.Text";
        private const string VOICE_LANGUAGE_SETTINGS_KEY = "Laguage.Voice";

        private readonly ISettingsStorage storage;
        
        public static event Action<Locale> OnTextLanguageChanged;
        public static event Action<Locale> OnVoiceLanguageChanged;
        
        public static Locale TextLanguage { get; private set; }
        public static Locale VoiceLanguage { get; private set; }
     
        public LaguageSettings(ISettingsStorage storage)
        {
            this.storage = storage;
        }
        
        public void Load()
        {
            var text = LoadLocal(TEXT_LAGUAGE_SETTINGS_KEY);
            if (text != null)
                LocalizationSettings.SelectedLocale = text;
            
            VoiceLanguage = LoadLocal(VOICE_LANGUAGE_SETTINGS_KEY) ?? LocalizationSettings.SelectedLocale;
        }
        
        public void SetTextLanguage(Locale locale)
        {
            if(locale == null) return;
            if(LocalizationSettings.SelectedLocale == locale) return;
            
            LocalizationSettings.SelectedLocale = locale;
            storage.SetString(TEXT_LAGUAGE_SETTINGS_KEY, locale.Identifier.Code);
            OnTextLanguageChanged?.Invoke(locale);
        }

        public void SetVoiceLanguage(Locale locale)
        {
            if(locale == null) return;
            if(VoiceLanguage == locale) return;
            
            VoiceLanguage = locale;
            storage.SetString(VOICE_LANGUAGE_SETTINGS_KEY, locale.Identifier.Code);
            OnVoiceLanguageChanged?.Invoke(locale);
        }

        private Locale LoadLocal(string key)
        {
            if(!storage.HasKey(key)) return null;
            var code = storage.GetString(key, null);
            return LocalizationSettings.AvailableLocales.GetLocale(code);
        }

    }
}