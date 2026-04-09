using System.Collections;
using UnityEngine;

namespace Game.Scripts.Components.Explosion
{
    public class Ignitable: MonoBehaviour
    {
        [SerializeField] private float fuseTime = 3f;
        [SerializeField] private Explosion explosion;
        [SerializeField] private ParticleSystem fireEffect;
        [SerializeField] private ParticleSystem explosionEffect;
        [SerializeField] private ParticleSystem smokeEffect;
        
        private bool isIgnited;
        private float timer;
        
        public bool IsIgnited => isIgnited;

        public void Ignite()
        {
            if(isIgnited) return;
            isIgnited = true;

            if (fireEffect)
                fireEffect.Play();

            StartCoroutine(FuseRoutine());
        }

        private IEnumerator FuseRoutine()
        {
            yield return new WaitForSeconds(fuseTime);
            Explode();
        }

        private void Explode()
        {
            explosion.Explode(gameObject);

            if (explosionEffect)
            {
                explosionEffect.transform.SetParent(null);
                explosionEffect.Play();
            }

            if (smokeEffect)
            {
                smokeEffect.transform.SetParent(null);
                smokeEffect.Play();
            }
            
            Destroy(gameObject);
        }
    }
}