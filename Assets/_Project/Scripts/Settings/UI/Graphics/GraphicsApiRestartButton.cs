using System.Diagnostics;
using Game.Scripts.Settings;
using Game.Scripts.Settings.Modules;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Settings.UI.Graphics
{
    [RequireComponent(typeof(Button))]
    public class GraphicsApiRestartButton : MonoBehaviour
    {
        private Button _button;
        
        private void Awake()
        {
            _button = GetComponent<Button>();
        }
        
        private void OnEnable()
        {
            _button.onClick.AddListener(OnClick);
            UpdateInteractable();
            
            GameSettings.Graphics.OnGraphicsApiChanged += _ => UpdateInteractable();
        }
        
        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnClick);
        }
        
        private void UpdateInteractable()
        {
            _button.interactable = GameSettings.Graphics.GraphicsApiRequiresRestart;
        }
        
        private void OnClick()
        {
            string arg = GetCommandLineArgument(GameSettings.Graphics.PreferredGraphicsApi);
            if (string.IsNullOrEmpty(arg))
            {
                UnityEngine.Debug.LogWarning("No command-line arg for selected API");
                return;
            }
            
#if UNITY_STANDALONE
            string exePath = System.IO.Path.Combine(
                Application.dataPath, 
                "..", 
                Application.productName + GetExecutableExtension()
            );
            
            Process.Start(exePath, arg);
            Application.Quit();
#endif
        }
        
        private static string GetCommandLineArgument(GraphicsApi api)
        {
            return api switch
            {
                GraphicsApi.Direct3D11 => "-force-d3d11",
                GraphicsApi.Direct3D12 => "-force-d3d12",
                GraphicsApi.Vulkan => "-force-vulkan",
                GraphicsApi.OpenGLCore => "-force-glcore",
                _ => null
            };
        }
        
        private static string GetExecutableExtension()
        {
#if UNITY_STANDALONE_WIN
            return ".exe";
#elif UNITY_STANDALONE_OSX
            return ".app";
#else
            return "";
#endif
        }
    }
}