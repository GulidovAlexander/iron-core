using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game.Scripts.UI.Menus
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button exitButton;

        private void Start()
        {
            newGameButton.onClick.AddListener(NewGameButtonClicked);
            settingsButton.onClick.AddListener(SettingsButtonClicked);
            exitButton.onClick.AddListener(ExitButtonClicked);
        }
      
        private void OnDestroy()
        {
            newGameButton.onClick.RemoveListener(NewGameButtonClicked);
            settingsButton.onClick.RemoveListener(SettingsButtonClicked);
            exitButton.onClick.RemoveListener(ExitButtonClicked);
        }

        private void NewGameButtonClicked()
        {
            SceneLoader.Instance.LoadScene("_Test");
            
        }
        
        private void ExitButtonClicked()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }

        private void SettingsButtonClicked()
        {
            SceneLoader.Instance.LoadScene("Settings");
        }
    }
}