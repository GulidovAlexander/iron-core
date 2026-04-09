using System;
using UnityEngine;

namespace Game.Components.Health
{
    public class HealthComponent : MonoBehaviour
    {
        private const float MinHealth = 1f;
        
        [SerializeField] private float maxHealth = 100f;
        private float currentHealth;

        public event Action<float, float> OnHealthChanged;
        public event Action OnDeath;
        
        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;
        public float HealthPercentage => currentHealth / maxHealth;
        public bool IsDead => currentHealth <= 0f;
        public bool IsFullHealth => currentHealth >= maxHealth;

        private void Awake() => Initialize(maxHealth);
        
        public void Initialize(float max)
        {
            if (max <= 0f) 
                Debug.LogError($"[HealthComponent] maxHealth must be > 0", this);
            
            maxHealth = max > 0f ? max : MinHealth;
            currentHealth = maxHealth;
        }

        public void TakeDamage(DamageData damage)
        {
            if (IsDead) return;

            currentHealth = Mathf.Max(currentHealth - damage.amount, 0f);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);

            if (IsDead) OnDeath?.Invoke();
        }

        public void Heal(float amount)
        {
            if (IsDead || amount <= 0f) return;
            
            currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }

        public void Revive()
        {
            currentHealth = maxHealth;
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }
    }
}