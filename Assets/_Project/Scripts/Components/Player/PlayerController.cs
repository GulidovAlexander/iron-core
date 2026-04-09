using Game.Scripts.Core.Movement;
using Player.Scripts;
using UnityEngine;

namespace Game.Scripts.Components.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float walkSpeed = 5f;
        [SerializeField] private float sprintSpeed = 8f;
        [SerializeField] private float jumpForce = 4f;

        [Header("Ground Check")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundDistance = 0.4f;
        [SerializeField] private LayerMask groundMask;

        [Header("References")]
        [SerializeField] private Transform cameraTransform;

        private PlayerCharacterBrain brain;
        private Mover mover;

        private void Awake()
        {
            var controller = GetComponent<CharacterController>();
            var input = GetComponent<PlayerInputHandler>();

            mover = new Mover(controller);
            brain = new PlayerCharacterBrain(
                mover, input, cameraTransform,
                walkSpeed, sprintSpeed, jumpForce);

            brain.Initialization();
            brain.Enable();
        }

        private void Update()
        {
            bool isGrounded = Physics.CheckSphere(
                groundCheck.position, groundDistance, groundMask);

            mover.UpdateGrounded(isGrounded);
            mover.Tick(Time.deltaTime);
            brain.Tick(Time.deltaTime);
        }

        private void OnDrawGizmosSelected()
        {
            if (groundCheck == null) return;
            Gizmos.color = mover?.IsGrounded ?? false ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }
}