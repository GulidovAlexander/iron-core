using Game.Scripts.Settings.Storage;

namespace Game.Scripts.Settings.Modules
{
    public class AudioSettings
    {
        private readonly ISettingsStorage storage;

        public AudioSettings(ISettingsStorage storage)
        {
            this.storage = storage;
        }

        public void Load()
        {
            throw new System.NotImplementedException();
        }
    }
}