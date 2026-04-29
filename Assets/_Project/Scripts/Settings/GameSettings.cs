using System;
using Game.Scripts.Settings.Modules;
using Game.Scripts.Settings.Storage;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace Game.Scripts.Settings
{
    public static class GameSettings
    {
        public static GraphicsSettings Graphics { get; private set; }
        public static LaguageSettings Language { get; private set; }
        public static Modules.AudioSettings Audio { get; private set; }
        public static GameplaySettings Gameplay { get; private set; }
        public static ControlsSettings Controls { get; private set; }
        
        public static bool IsInitialized { get; private set; }

        public static void Initialize()
        {
            if(IsInitialized) return;

            var storage = new PlayerPrefsStorage();
            
            Graphics = new GraphicsSettings(storage);
            Language = new LaguageSettings(storage);
            Audio = new Modules.AudioSettings(storage);
            Gameplay = new GameplaySettings(storage);
            Controls = new ControlsSettings(storage);

            Graphics.Load();
            Language.Load();
            // Audio.Load();
            // Gameplay.Load();
            // Controls.Load();
            
            IsInitialized = true;
        }

        public static void Save()
        {
            PlayerPrefs.Save();
        }
    }
}