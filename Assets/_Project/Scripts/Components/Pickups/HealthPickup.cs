using Game.Components.Health;
using UnityEngine;

namespace Game.Scripts.Components.Pickups
{
    public class HealthPickup: Pickup
    {
        [SerializeField] private float healAmount = 25f;

        protected override bool CanPickup(GameObject collector)
        {
            if(!collector.TryGetComponent(out HealthComponent health)) return false;
            return !health.IsDead && !health.IsFullHealth;
        }
        
        protected override void OnPickup(GameObject collector)
        {
            if(collector.TryGetComponent(out HealthComponent health))
                health.Heal(healAmount);
        }
    }
}