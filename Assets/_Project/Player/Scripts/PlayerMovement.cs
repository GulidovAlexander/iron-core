using UnityEngine;

namespace Player.Scripts
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float walkSpeed = 5f;
        [SerializeField] private float sprintSpeed = 8f;
        [SerializeField] private float jumpForce = 6.5f;
        [SerializeField] private float gravity = -15f;  // Увеличил гравитацию
        
        [Header("Gravity Settings")]
        [SerializeField] private float fallMultiplier = 2.5f;
        
        [Header("Ground Check")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundDistance = 0.4f;
        [SerializeField] private LayerMask groundMask;
        
        [Header("References")]
        [SerializeField] private Transform cameraTransform;
        
        private CharacterController controller;
        private PlayerInputHandler input;
        private Vector3 velocity;
        private bool isGrounded;
        
        // Для предотвращения двойного прыжка
        private bool canJump = true;
        
        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            input = GetComponent<PlayerInputHandler>();
            
            if (cameraTransform == null)
            {
                Debug.LogWarning("PlayerMovement: cameraTransform not assigned!");
            }
        }
    
        private void Update()
        {
            HandleGroundCheck();
            HandleMovement();
            HandleGravity();
            HandleJump();
        }
    
        private void HandleGroundCheck()
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        
            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
                canJump = true;  // Только когда на земле - можно прыгать
            }
            else
            {
                canJump = false;  // В воздухе - прыгать нельзя
            }
        }
    
        private void HandleMovement()
        {
            if (cameraTransform == null) return;
            
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();
        
            Vector3 moveDirection = (forward * input.MoveInput.y) + (right * input.MoveInput.x);
            float currentSpeed = input.SprintHeld ? sprintSpeed : walkSpeed;
            float moveDistance = currentSpeed * Time.deltaTime;
    
            controller.Move(moveDirection * moveDistance); 
        }
    
        private void HandleJump()
        {
            // Прыгаем ТОЛЬКО если на земле И можем прыгать
            if (input.GetJumpPressed() && isGrounded && canJump)
            {
                velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
                canJump = false;  // Сразу запрещаем повторный прыжок
            }
        }
    
        private void HandleGravity()
        {
            // Ускоренное падение для реализма
            if (velocity.y < 0)
            {
                velocity.y += gravity * fallMultiplier * Time.deltaTime;
            }
            else
            {
                velocity.y += gravity * Time.deltaTime;
            }
            
            // Ограничиваем максимальную скорость падения
            if (velocity.y < -20f)
                velocity.y = -20f;
            
            controller.Move(velocity * Time.deltaTime);
        }
        
        private void OnDrawGizmosSelected()
        {
            if (groundCheck != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
            }
        }
    }
}