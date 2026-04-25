using Game.Core.Interfaces;
using UnityEngine;

namespace Game.Scripts.Components.Weapons
{
    public class PistolWeapon : BaseFirearm
    {
        public override void Attack()
        {
            if (!CanAttack) return;

            _currentAmmo--;
            _nextShotTime = Time.time + 1f / _data.FireRate;

            if (PerformRaycast(out var hit))
            {
                hit.collider.GetComponent<IDamageable>()
                    ?.TakeDamage(new DamageData { amount = _data.Damage });
            }

            PlaySound(_data.ShootSound);
            NotifyAmmoChanged();
            NotifyAttacked();
        }
    }
}