using Game.Core.Interfaces;
using UnityEngine;

namespace Game.Scripts.Components.Weapons
{
    public class ShotgunWeapon : BaseFirearm
    {
        [SerializeField] private int _pelletsCount = 8;    // кол-во дробин
        [SerializeField] private float _spreadAngle = 10f; // разброс в градусах

        public override void Attack()
        {
            if (!CanAttack) return;

            _currentAmmo--;
            _nextShotTime = Time.time + 1f / _data.FireRate;

            for (int i = 0; i < _pelletsCount; i++)
                FirePellet();

            PlaySound(_data.ShootSound);
            NotifyAmmoChanged();
            NotifyAttacked();
        }

        private void FirePellet()
        {
            var cameraRay = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            var targetPoint = cameraRay.GetPoint(_data.Range);

            if (Physics.Raycast(cameraRay, out var cameraHit, _data.Range))
                targetPoint = cameraHit.point;

            // разброс для каждой дробины
            var spread = Random.insideUnitSphere * Mathf.Tan(_spreadAngle * Mathf.Deg2Rad);
            var direction = ((targetPoint - _muzzle.position).normalized + spread).normalized;

            if (Physics.Raycast(_muzzle.position, direction, out var hit, _data.Range))
            {
                hit.collider.GetComponent<IDamageable>()
                    ?.TakeDamage(new DamageData { amount = _data.Damage });

                SpawnImpact(hit);
            }
        }
    }
}