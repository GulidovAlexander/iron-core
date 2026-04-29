namespace Game.Scripts.Settings.Storage
{
    public interface ISettingsStorage
    {
        void SetInt(string key, int value);
        int GetInt(string key, int defaultValue);
    
        void SetFloat(string key, float value);
        float GetFloat(string key, float defaultValue);
    
        void SetString(string key, string value);
        string GetString(string key, string defaultValue);
    
        bool HasKey(string key);
        void Save();
    }
}