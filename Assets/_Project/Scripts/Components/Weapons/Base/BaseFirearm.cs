using Game.Core.Interfaces;
using Game.Scripts.Components.VFX;
using UnityEngine;

namespace Game.Scripts.Components.Weapons
{
    public abstract class BaseFirearm : BaseWeapon
    {
        [SerializeField] protected Transform _muzzle;
        [SerializeField] protected GameObject _impactPrefab;
        [SerializeField] protected GameObject _sparksVFX;  // для стен/металла
        [SerializeField] protected GameObject _bloodVFX;   // для живых

        protected Camera _camera;

        protected override void Awake()
        {
            base.Awake();
            _camera = Camera.main;
        }

        // Общий raycast для всего огнестрела
        // Возвращает true если попал, заполняет hit
        protected bool PerformRaycast(out RaycastHit hit)
        {
            var cameraRay = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            var targetPoint = cameraRay.GetPoint(_data.Range);

            if (Physics.Raycast(cameraRay, out var cameraHit, _data.Range))
                targetPoint = cameraHit.point;

            var shootDirection = (targetPoint - _muzzle.position).normalized;

            if (Physics.Raycast(_muzzle.position, shootDirection, out hit, _data.Range))
            {
                SpawnImpact(hit);
                return true;
            }

            return false;
        }

        protected void SpawnImpact(RaycastHit hit)
        {
            bool isAlive = hit.collider.GetComponent<IDamageable>() != null;
            bool isMovable = hit.rigidbody != null && !hit.rigidbody.isKinematic;

            // декаль — только на статике и движущихся неживых
            if (!isAlive)
                DecalPool.Instance?.Get(
                    hit.point,
                    Quaternion.LookRotation(hit.normal),
                    isMovable ? hit.transform : null);

            // VFX — искры или кровь
            var vfxPrefab = isAlive ? _bloodVFX : _sparksVFX;
            if (vfxPrefab == null) return;

            var vfx = Instantiate(vfxPrefab, hit.point,
                Quaternion.LookRotation(hit.normal));
            Destroy(vfx, 2f);
        }
    }
}