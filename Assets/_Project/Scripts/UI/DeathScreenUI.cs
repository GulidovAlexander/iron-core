using System;
using Game.Scripts.Common.Enums;
using Game.Scripts.Components.Player;
using Game.Scripts.Core;
using UnityEngine;

namespace Game.Scripts.UI
{
    public class DeathScreenUI: MonoBehaviour
    {
        [SerializeField] private GameObject deathScreen;
        [SerializeField] private PlayerRespawn playerRespawn;

        private void Awake()
        {
            Hide();
        }

        public void Show()
        {
            deathScreen.SetActive(true);
            CursorManager.Instance.RequestUnlock(CursorUnlockRequester.UI);
            Time.timeScale = 0f;
        }
        
        public void OnRespawnClicked()
        {
            Time.timeScale = 1f;
            CursorManager.Instance.RequestLock(CursorUnlockRequester.UI);
            Hide();
            playerRespawn.Respawn();
        }
        
        private void Hide()
        {
            deathScreen.SetActive(false);
        }
    }
}