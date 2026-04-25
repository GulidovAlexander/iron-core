using Game.Scripts.Common.Enums;
using Game.Scripts.Core;
using UnityEngine;

namespace Game.Scripts.UI
{
    public class MenuManager: MonoBehaviour
    {
        [SerializeField] private GameObject pauseMenuUI;
        [SerializeField] private PlayerInputHandler input;
        
        private bool isPaused =false;

        private void Start()
        {
            input.OnMenuPressed += TogglePause;
            if(pauseMenuUI)
                pauseMenuUI.SetActive(false);
        }

        public void ResumeGame()
        {
            if(isPaused)
                TogglePause();
        }
        
        public void ConfirmQuit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
        }
        

        private void TogglePause()
        {
            isPaused = !isPaused;
            if(pauseMenuUI)
                pauseMenuUI.SetActive(isPaused);
    
            if (isPaused)
            {
                CursorManager.Instance.RequestUnlock(CursorUnlockRequester.UI);
                TimeScaleManager.Instance.RequestFreeze(TimeScaleRequester.PauseMenu);
            }
            else
            {
                CursorManager.Instance.RequestLock(CursorUnlockRequester.UI);
                TimeScaleManager.Instance.RequestUnfreeze(TimeScaleRequester.PauseMenu);
            }
        }

        private void OnDestroy()
        {
            if(input != null)
                input.OnMenuPressed -= TogglePause;
        }
    }
}