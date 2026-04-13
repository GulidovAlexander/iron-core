using Game.Core.Interfaces;
using Game.Scripts.Core.DataStructs;
using UnityEngine;

namespace Game.Scripts.Components.Explosion
{
    public class Explosion: MonoBehaviour
    {
        [SerializeField] public float damage = 50f;
        [SerializeField] public float radius = 5f;
        [SerializeField] public float knockbackForce = 10f;
        [SerializeField] public LayerMask affectedLayers;
        
        private readonly Collider[] overlapBuffer = new Collider[32];

        public void Explode(GameObject source)
        {
            var size = Physics.OverlapSphereNonAlloc(transform.position, radius, overlapBuffer, affectedLayers);

            for (var i = 0; i < size; i++)
            {
                var hit = overlapBuffer[i];
                var falloff = CalculateFalloff(hit.transform.position);
        
                ApplyDamage(hit, source, falloff);
                ApplyKnockback(hit, falloff);
            }
        }

        private void ApplyDamage(Collider hit, GameObject source, float falloff)
        {
            if (!hit.TryGetComponent(out IDamageable damageable)) return;
            damageable.TakeDamage(new DamageData(damage * falloff, DamageType.Explosion, source));
        }

        private void ApplyKnockback(Collider hit, float falloff)
        {
            if (!hit.TryGetComponent(out Rigidbody rb)) return;
            if (rb.isKinematic) return;

            var direction = (hit.transform.position - transform.position).normalized;
            rb.AddForce(direction * knockbackForce * falloff, ForceMode.Impulse);
        }
        
        private float CalculateFalloff(Vector3 position)
        {
            var distance = Vector3.Distance(transform.position, position);
            return 1f - Mathf.Clamp01(distance / radius);
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}