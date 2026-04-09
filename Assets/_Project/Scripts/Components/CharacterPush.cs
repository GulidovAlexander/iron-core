using Game.Scripts.Core.Extensions;
using UnityEngine;

namespace Game.Scripts.Components
{
    public class CharacterPush: MonoBehaviour
    {
        [SerializeField] private float pushForce = 3f;
        [SerializeField] private LayerMask pushableLayers;

        private const float VerticalPushThreshold = -0.3f;

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (!pushableLayers.HasLayer(hit.collider.gameObject.layer)) return;
            
            var rb = hit.collider.attachedRigidbody;
            if(rb == null || rb.isKinematic) return;
            if(hit.moveDirection.y < VerticalPushThreshold) return;
            
            var pushDirection = new  Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
            rb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
        }
    }
}