using System;
using UnityEngine;

namespace Game.Scripts.Components.Weapons
{
    [Serializable]
    public class WeaponSlot
    {
        public BaseWeapon Weapon;
        public Animator WeaponAnimator;
    }
}