using UnityEngine;

namespace Game.Scripts.Components.Pickups
{
    public abstract class Pickup: MonoBehaviour
    {
        public void TryPickup(GameObject collector)
        {
            if(!CanPickup(collector)) return;
            
            OnPickup(collector);
            Destroy(gameObject);
        }
        
        protected virtual bool CanPickup(GameObject collector) => true;
        protected abstract void OnPickup(GameObject collector);
    }
}