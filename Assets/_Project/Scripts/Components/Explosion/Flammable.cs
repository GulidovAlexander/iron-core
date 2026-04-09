using UnityEngine;

namespace Game.Scripts.Components.Explosion
{
    public class Flammable: MonoBehaviour
    {
        private Ignitable ignitable;

        private void Awake()
        {
            ignitable = GetComponent<Ignitable>();
        }

        public void CatchFire()
        {
            ignitable?.Ignite();
        }
    }
}