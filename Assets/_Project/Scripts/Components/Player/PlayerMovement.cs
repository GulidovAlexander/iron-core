using Game.Scripts.Core.Movement;
using UnityEngine;

namespace Game.Scripts.Components.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
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

        private Mover mover;
        private PlayerInputHandler input;

        private void Awake()
        {
            var controller = GetComponent<CharacterController>();
            input = GetComponent<PlayerInputHandler>();
            mover = new Mover(controller);

            if (cameraTransform == null)
                Debug.LogWarning("PlayerMovement: cameraTransform not assigned!", this);
        }

        private void Update()
        {
            bool isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
            mover.UpdateGrounded(isGrounded);
            mover.Tick(Time.deltaTime);

            if (input.GetJumpPressed())
                mover.RequestJump(jumpForce);

            Vector3 moveDirection = GetMoveDirection();
            float speed = input.SprintHeld ? sprintSpeed : walkSpeed;
            mover.Update(moveDirection, speed, Time.deltaTime);
        }

        private Vector3 GetMoveDirection()
        {
            if (cameraTransform == null) return Vector3.zero;

            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            return forward * input.MoveInput.y + right * input.MoveInput.x;
        }

        private void OnDrawGizmosSelected()
        {
            if (groundCheck == null) return;
            Gizmos.color = mover?.IsGrounded ?? false ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }
}