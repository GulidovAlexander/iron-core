using Game.Scripts.Components.Pickups;
using UnityEngine;

namespace Game.Scripts.Components.Player
{
    public class PickupDetector : MonoBehaviour
    {
        [SerializeField] private float pickupRadius = 2f;
        private SphereCollider triggerZone;

        private void Awake()
        {
            triggerZone = gameObject.AddComponent<SphereCollider>();
            triggerZone.isTrigger = true;
            triggerZone.radius = pickupRadius;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Pickup pickup))
                pickup.TryPickup(transform.root.gameObject);
        }
    }
}