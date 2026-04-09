using Game.Scripts.Core.Movement;
using UnityEngine;

namespace Game.Scripts.Components.Enemies
{
    [RequireComponent(typeof(CharacterController))]
    public class EnemyMovement : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 3f;

        private Mover mover;

        private void Awake()
        {
            mover = new Mover(GetComponent<CharacterController>());
        }

        public void Move(Vector3 direction)
        {
            mover.UpdateGrounded(true);
            mover.Update(direction, moveSpeed, Time.deltaTime);
        }
    }
}