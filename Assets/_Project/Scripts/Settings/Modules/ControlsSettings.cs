using Game.Scripts.Settings.Storage;

namespace Game.Scripts.Settings.Modules
{
    public class ControlsSettings
    {
        private readonly PlayerPrefsStorage storage;

        public ControlsSettings(PlayerPrefsStorage storage)
        {
            this.storage = storage;
        }

        public void Load()
        {
            throw new System.NotImplementedException();
        }
    }
}