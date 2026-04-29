using UnityEngine;

namespace Game.Scripts.Settings.Storage
{
    public class PlayerPrefsStorage : ISettingsStorage
    {
        public void SetInt(string key, int value) => PlayerPrefs.SetInt(key, value);
        public int GetInt(string key, int defaultValue) => PlayerPrefs.GetInt(key, defaultValue);
        public void SetFloat(string key, float value) => PlayerPrefs.SetFloat(key, value);
        public float GetFloat(string key, float defaultValue) => PlayerPrefs.GetFloat(key, defaultValue);
        public void SetString(string key, string value) => PlayerPrefs.SetString(key, value);
        public string GetString(string key, string defaultValue) => PlayerPrefs.GetString(key, defaultValue);
        public bool HasKey(string key) => PlayerPrefs.HasKey(key);
        public void Save() => PlayerPrefs.Save();
    }
}