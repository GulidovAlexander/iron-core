using UnityEngine;

namespace Game.Scripts.Components
{
    public class FaceCamera : MonoBehaviour
    {
        private Camera mainCamera;

        private void Awake()
        {
            mainCamera = Camera.main;
        }

        private void LateUpdate()
        {
            transform.LookAt(transform.position + mainCamera.transform.forward);
        }
    }
}