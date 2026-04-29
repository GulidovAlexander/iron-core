using Game.Scripts.Settings.Storage;

namespace Game.Scripts.Settings.Modules
{
    public class GameplaySettings
    {
        private readonly PlayerPrefsStorage storage;

        public GameplaySettings(PlayerPrefsStorage storage)
        {
            this.storage = storage;
        }

        public void Load()
        {
            throw new System.NotImplementedException();
        }
    }
}