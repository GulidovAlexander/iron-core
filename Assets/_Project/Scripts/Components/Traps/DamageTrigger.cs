using Game.Core.Interfaces;
using Game.Scripts.Core.DataStructs;
using UnityEngine;

namespace _Project.Scripts.Components.Traps
{
    public class DamageTrigger: MonoBehaviour
    {
        [SerializeField] private float damagePerTick = 5f;
        [SerializeField] private float tickInterval = 1f;
        [SerializeField] private DamageType damageType = DamageType.Fire;

        private float timer;

        private void OnTriggerEnter(Collider other)
        {
            if (!TryGetDamageable(other, out var damageable)) return;

            ApplyDamage(damageable);
            timer = 0f;
        }

        private void OnTriggerStay(Collider other)
        {
            if (!TryGetDamageable(other, out var damageable)) return;
            
            timer += Time.deltaTime;

            if (timer < tickInterval) return;
            
            ApplyDamage(damageable);
            timer = 0f;
        }

        private void OnTriggerExit(Collider other)
        {
            timer = 0f;
        }
        
        private static bool TryGetDamageable(Collider other, out IDamageable damageable)
        {
            return other.TryGetComponent(out damageable) && !damageable.IsDead;
        }
        
        private void ApplyDamage(IDamageable damageable)
        {
            damageable.TakeDamage(new DamageData(damagePerTick, damageType, gameObject));
        }
    }
}