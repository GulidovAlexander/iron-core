using UnityEngine;

namespace Game.Scripts.Core.Movement
{
    public class Mover
    {
        private readonly CharacterController controller;

        private Vector3 velocity;
        private float coyoteTimer;
        private float jumpBufferTimer;

        private readonly float gravity;
        private readonly float fallMultiplier;
        private readonly float coyoteTime;
        private readonly float jumpBufferTime;
        private readonly float maxFallSpeed;

        public bool IsGrounded { get; private set; }
        public Vector3 Velocity => velocity;

        public Mover(
            CharacterController controller,
            float gravity = -20f,
            float fallMultiplier = 4f,
            float coyoteTime = 0.15f,
            float jumpBufferTime = 0.1f,
            float maxFallSpeed = 40f)
        {
            this.controller = controller;
            this.gravity = gravity;
            this.fallMultiplier = fallMultiplier;
            this.coyoteTime = coyoteTime;
            this.jumpBufferTime = jumpBufferTime;
            this.maxFallSpeed = maxFallSpeed;
        }

        public void Update(Vector3 moveDirection, float speed, float deltaTime)
        {
            HandleGravity(deltaTime);
            controller.Move(moveDirection * speed * deltaTime + velocity * deltaTime);
        }

        public void UpdateGrounded(bool isGrounded)
        {
            bool wasGrounded = IsGrounded;
            IsGrounded = isGrounded;

            if (IsGrounded && velocity.y < 0f)
                velocity.y = -2f;

            if (wasGrounded && !IsGrounded)
                coyoteTimer = coyoteTime;
            else if (IsGrounded)
                coyoteTimer = 0f;
            else
                coyoteTimer -= Time.deltaTime;
        }

        public void RequestJump(float jumpForce)
        {
            jumpBufferTimer = jumpBufferTime;
            TryJump(jumpForce);
        }

        public void Tick(float deltaTime)
        {
            if (jumpBufferTimer > 0f)
                jumpBufferTimer -= deltaTime;
        }

        private void TryJump(float jumpForce)
        {
            bool canJump = IsGrounded || coyoteTimer > 0f;
            if (jumpBufferTimer <= 0f || !canJump) return;

            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
        }

        private void HandleGravity(float deltaTime)
        {
            if (velocity.y < 0f)
                velocity.y += gravity * fallMultiplier * deltaTime;
            else
                velocity.y += gravity * deltaTime;

            velocity.y = Mathf.Max(velocity.y, -maxFallSpeed);
        }
    }
}