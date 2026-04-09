using Game.Components.Health;
using UnityEngine;

namespace Game.Scripts.Components
{
    public class DeathHandler: MonoBehaviour
    {
        [SerializeField] private float destroyDelay = 0f;

        private HealthComponent health;

        private void Awake()
        {
            health = GetComponent<HealthComponent>();
            health.OnDeath += HandleDeath;
        }

        private void OnDestroy()
        {
            health.OnDeath -= HandleDeath;
        }

        private void HandleDeath()
        {
            Destroy(gameObject);
        }
    }
}