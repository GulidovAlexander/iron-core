using System;
using Game.Scripts.Settings.Storage;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Scripts.Settings.Modules
{
    public class GraphicsSettings
    {
        private const string KeyWindowMode = "Graphics.WindowMode";
        private const string KeyHDR = "Graphics.HDR";
        private const string KeyDisplay = "Graphics.Display";
        private const string KeyResolutionWidth = "Graphics.ResolutionWidth";
        private const string KeyResolutionHeight = "Graphics.ResolutionHeight";
        private const string KeyResolutionRefresh = "Graphics.ResolutionRefresh";
        private const string KeyFpsLimit = "Graphics.FpsLimit";
        private const string KeyVSync = "Graphics.VSync";
        private const string KeyBrightness = "Graphics.Brightness";
        private const string KeyGraphicsApi = "Graphics.Api";
        
        private readonly ISettingsStorage _storage;
    
        public event Action<FullScreenMode> OnWindowModeChanged;
        public event Action<bool> OnHDRChanged;
        public event Action<int> OnDisplayChanged;
        public event Action<Resolution> OnResolutionChanged;
        public event Action<int> OnFpsLimitChanged;
        public event Action<bool> OnVSyncChanged;
        public event Action<float> OnBrightnessChanged;
        public event Action<GraphicsApi> OnGraphicsApiChanged;
    
        public FullScreenMode WindowMode { get; private set; }
        public bool HDREnabled { get; private set; }
        public int DisplayIndex { get; private set; }
        public Resolution CurrentResolution { get; private set; }
        public int FpsLimit { get; private set; }     // -1 = без лимита
        public bool VSyncEnabled { get; private set; }
        public float Brightness { get; private set; } // 0.5 - 1.5 (1.0 = норм)
        public GraphicsApi PreferredGraphicsApi { get; private set; }
        
        public GraphicsSettings(ISettingsStorage storage)
        {
            _storage = storage;
        }
        
        public bool GraphicsApiRequiresRestart => 
            PreferredGraphicsApi != GraphicsApi.Auto && 
            !IsCurrentApi(PreferredGraphicsApi);
        
        // === WindowMode ===
        public void SetWindowMode(FullScreenMode mode)
        {
            if (WindowMode == mode) return;
            
            WindowMode = mode;
            Screen.fullScreenMode = mode;
            _storage.SetInt(KeyWindowMode, (int)mode);
            OnWindowModeChanged?.Invoke(mode);
        }
        
        // === HDR ===
        public void SetHDR(bool enabled)
        {
            if (HDREnabled == enabled) return;
            
            HDREnabled = enabled;
            ApplyHDR(enabled);
            _storage.SetInt(KeyHDR, enabled ? 1 : 0);
            OnHDRChanged?.Invoke(enabled);
        }
        
        // === Display ===
        public void SetDisplay(int index)
        {
            if (DisplayIndex == index) return;
            
            DisplayIndex = index;
            _storage.SetInt(KeyDisplay, index);
            OnDisplayChanged?.Invoke(index);
        }
        
        // === Resolution ===
        public void SetResolution(Resolution resolution)
        {
            if (ResolutionsEqual(CurrentResolution, resolution)) return;
            
            CurrentResolution = resolution;
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode, resolution.refreshRateRatio);
            
            _storage.SetInt(KeyResolutionWidth, resolution.width);
            _storage.SetInt(KeyResolutionHeight, resolution.height);
            _storage.SetInt(KeyResolutionRefresh, (int)resolution.refreshRateRatio.value);
            
            OnResolutionChanged?.Invoke(resolution);
        }
        
        // === FPS Limit ===
        public void SetFpsLimit(int fps)
        {
            if (FpsLimit == fps) return;
            
            FpsLimit = fps;
            Application.targetFrameRate = fps;
            _storage.SetInt(KeyFpsLimit, fps);
            OnFpsLimitChanged?.Invoke(fps);
        }
        
        // === VSync ===
        public void SetVSync(bool enabled)
        {
            if (VSyncEnabled == enabled) return;
            
            VSyncEnabled = enabled;
            QualitySettings.vSyncCount = enabled ? 1 : 0;
            _storage.SetInt(KeyVSync, enabled ? 1 : 0);
            OnVSyncChanged?.Invoke(enabled);
        }
        
        // === Brightness ===
        public void SetBrightness(float value)
        {
            value = Mathf.Clamp(value, 0.5f, 1.5f);
            if (Mathf.Approximately(Brightness, value)) return;
            
            Brightness = value;
            _storage.SetInt(KeyBrightness, Mathf.RoundToInt(value * 100));
            OnBrightnessChanged?.Invoke(value);
        }
        
        // === Graphics API ===
        public void SetGraphicsApi(GraphicsApi api)
        {
            if (PreferredGraphicsApi == api) return;
            
            PreferredGraphicsApi = api;
            _storage.SetInt(KeyGraphicsApi, (int)api);
            OnGraphicsApiChanged?.Invoke(api);
            // Применение требует перезапуска — обрабатывается отдельно
        }
        
        // === Загрузка ===
        public void Load()
        {
            WindowMode = (FullScreenMode)_storage.GetInt(KeyWindowMode, (int)FullScreenMode.ExclusiveFullScreen);
            HDREnabled = _storage.GetInt(KeyHDR, 0) == 1;
            DisplayIndex = _storage.GetInt(KeyDisplay, 0);
            FpsLimit = _storage.GetInt(KeyFpsLimit, -1);
            VSyncEnabled = _storage.GetInt(KeyVSync, 1) == 1;
            Brightness = _storage.GetInt(KeyBrightness, 100) / 100f;
            PreferredGraphicsApi = (GraphicsApi)_storage.GetInt(KeyGraphicsApi, (int)GraphicsApi.Auto);
            
            // Загрузка разрешения
            int width = _storage.GetInt(KeyResolutionWidth, Screen.currentResolution.width);
            int height = _storage.GetInt(KeyResolutionHeight, Screen.currentResolution.height);
            int refresh = _storage.GetInt(KeyResolutionRefresh, (int)Screen.currentResolution.refreshRateRatio.value);
            
            CurrentResolution = new Resolution
            {
                width = width,
                height = height,
                refreshRateRatio = new RefreshRate { numerator = (uint)refresh, denominator = 1 }
            };
            
            // Применяем
            Screen.fullScreenMode = WindowMode;
            Screen.SetResolution(width, height, WindowMode, CurrentResolution.refreshRateRatio);
            Application.targetFrameRate = FpsLimit;
            QualitySettings.vSyncCount = VSyncEnabled ? 1 : 0;
            ApplyHDR(HDREnabled);
        }
        
        // === Хелперы ===
        public Resolution[] GetAvailableResolutions()
        {
            return Screen.resolutions;
        }
        
        private void ApplyHDR(bool enabled)
        {
            if (Camera.main != null)
                Camera.main.allowHDR = enabled;
        }
        
        private bool IsCurrentApi(GraphicsApi api)
        {
            var current = SystemInfo.graphicsDeviceType;
            return api switch
            {
                GraphicsApi.Direct3D11 => current == GraphicsDeviceType.Direct3D11,
                GraphicsApi.Direct3D12 => current == GraphicsDeviceType.Direct3D12,
                GraphicsApi.Vulkan => current == GraphicsDeviceType.Vulkan,
                GraphicsApi.OpenGLCore => current == GraphicsDeviceType.OpenGLCore,
                GraphicsApi.Metal => current == GraphicsDeviceType.Metal,
                _ => true
            };
        }
        
        private static bool ResolutionsEqual(Resolution a, Resolution b)
        {
            return a.width == b.width 
                && a.height == b.height 
                && Mathf.Approximately((float)a.refreshRateRatio.value, (float)b.refreshRateRatio.value);
        }
    }
}