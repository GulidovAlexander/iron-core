using System;
using UnityEngine;

namespace Game.Components.Health
{
    public class ArmorComponent: MonoBehaviour
    {
        private const float MIN_ARMOR = 0f;
        
        [SerializeField] private float maxArmor = 100f;
        private float currentArmor;
        
        public event Action<float,float> OnArmorChanged;
        
        public float CurrentArmor => currentArmor;
        public float MaxArmor => maxArmor;
        public float ArmorPercentage => currentArmor / maxArmor;
        public bool IsArmorFull => currentArmor >= maxArmor;
        public bool HasArmor => currentArmor > MIN_ARMOR;

        private void Awake()
        {
            Initialize(maxArmor);
        }

        public void Initialize(float max)
        {
            if(max <= 0f)
                Debug.LogError("[Armor Component] maxArmor must be > 0", this);
            
            maxArmor = max > 0f ? max : 1f;
            currentArmor = MIN_ARMOR;
        }

        public float Absorb(float damage)
        {
            if (!HasArmor) return damage;
            
            var absorbed = Mathf.Min(currentArmor, damage);
            currentArmor -= absorbed;
            OnArmorChanged?.Invoke(currentArmor, maxArmor);
            
            return damage - absorbed;
        }

        public void AddArmor(float amount)
        {
            if(amount <= 0f) return;
            
            currentArmor = Mathf.Min(currentArmor + amount, maxArmor);
            OnArmorChanged?.Invoke(currentArmor, maxArmor);
        }
    }
}