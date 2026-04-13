using Game.Scripts.Core.Brains;
using Game.Scripts.Core.Movement;
using UnityEngine;

namespace Game.Scripts.Components.Player
{
    public class PlayerCharacterBrain : Brain
    {
        private readonly Mover mover;
        private readonly PlayerInputHandler input;
        private readonly Transform cameraTransform;

        private readonly float walkSpeed;
        private readonly float sprintSpeed;
        private readonly float jumpForce;

        public PlayerCharacterBrain(
            Mover mover,
            PlayerInputHandler input,
            Transform cameraTransform,
            float walkSpeed = 5f,
            float sprintSpeed = 8f,
            float jumpForce = 4f)
        {
            this.mover = mover;
            this.input = input;
            this.cameraTransform = cameraTransform;
            this.walkSpeed = walkSpeed;
            this.sprintSpeed = sprintSpeed;
            this.jumpForce = jumpForce;
        }

        protected override void UpdateLogic(float dt)
        {
            HandleMovement(dt);
            HandleJump();
        }

        private void HandleMovement(float dt)
        {
            Vector3 direction = GetMoveDirection();
            float speed = input.SprintHeld ? sprintSpeed : walkSpeed;
            mover.Update(direction, speed, dt);
        }

        private void HandleJump()
        {
            if (input.GetJumpPressed())
                mover.RequestJump(jumpForce);
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
    }
}