using UnityEngine;

namespace Game.Scripts.Components.Player
{
    public class GroundChecker : MonoBehaviour
    {
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundDistance = 0.4f;
        [SerializeField] private LayerMask groundMask;

        public bool IsGrounded { get; private set; }
    }
}