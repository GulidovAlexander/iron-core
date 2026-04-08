using Game.Core.Interfaces;
using Game.Scripts.Core.DataStructs;
using UnityEngine;

namespace _Project.Scripts.Components.Effects
{
    public abstract class DamageEffect: ScriptableObject 
    {
        public DamageType damageType;
        public abstract void Apply(IDamageable target, GameObject source);
    }
}