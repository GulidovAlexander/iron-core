using Game.Scripts.Components.Explosion;
using UnityEngine;

namespace Game.Scripts.Components.Traps
{
    public class FireZone:MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if(other.TryGetComponent(out Flammable flammable))
                flammable.CatchFire();
        }
    }
}