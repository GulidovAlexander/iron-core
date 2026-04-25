using System;
using Game.Core.Interfaces;
using UnityEngine;

namespace Game.Components.Health
{
    public class DamageReceiver: MonoBehaviour, IDamageable
    {
        private HealthComponent health;
        private ArmorComponent armor;
        
        public bool IsDead => health.IsDead;

        private void Awake()
        {
            health = GetComponent<HealthComponent>();
            armor = GetComponent<ArmorComponent>();
            
            if(health == null)
                Debug.LogError("[DamageReceiver] Health component not found", this);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public void TakeDamage(DamageData damage)
        {
            if(IsDead) return;

            var remaining = armor != null
                ? armor.Absorb(damage.amount)
                : damage.amount;
            
            if(remaining <= 0f) return;
            
            health.TakeDamage(new DamageData(remaining, damage.damageType, damage.source));
        }
    }
}