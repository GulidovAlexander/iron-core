using Game.Components.Health;
using Game.Scripts.UI;
using UnityEngine;

namespace Game.Scripts.Components.Player
{
    public class PlayerRespawn: MonoBehaviour
    {
        [SerializeField] private float respawnDelay = 3f;
        [SerializeField] private Transform defaultSpawnPoint;
        [SerializeField] private DeathScreenUI deathScreen;
        
        private Transform currentSpawnPoint;
        private HealthComponent health;

        private void Awake()
        {
            health = GetComponent<HealthComponent>();
            health.OnDeath += HandleDeath;
            currentSpawnPoint = defaultSpawnPoint;
            
            transform.position = defaultSpawnPoint.position;
        }

        private void OnDestroy()
        {
            health.OnDeath -= HandleDeath;
        }

        public void SetSpawnPoint(Transform spawnPoint)
        {
            currentSpawnPoint = spawnPoint;
        }

        private void HandleDeath()
        {
            deathScreen.Show();
        }

        public void Respawn()
        {
            var cc = GetComponent<CharacterController>();
            if (cc != null)
            {
                cc.enabled = false;
                transform.position = currentSpawnPoint.position;
                cc.enabled = true;
            }
            else
            {
                transform.position = currentSpawnPoint.position;
            }
    
            health.Revive();
        }
    }
}