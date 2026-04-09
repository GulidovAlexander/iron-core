using Game.Components.Health;
using UnityEngine;

namespace Game.Scripts.Components.Explosion
{
    public class ExplosiveBarrel : MonoBehaviour
    {
        [SerializeField] private Ignitable ignitable;

        private HealthComponent health;

        private void Awake()
        {
            if(ignitable == null)
                Debug.LogError("[ExplosiveBarrel] Ignitable is null");
            
            health = GetComponent<HealthComponent>();
            health.OnDeath += HandleDeath;
        }

        private void OnDestroy()
        {
            health.OnDeath -= HandleDeath;
        }

        private void HandleDeath()
        {
            ignitable?.Ignite();
        }
    }
}