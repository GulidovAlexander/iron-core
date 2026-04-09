using Game.Scripts.Core;
using UnityEngine;

namespace Player.Scripts
{
    public class PlayerCamera : MonoBehaviour
    {
        [Header("Camera Settings")]
        [SerializeField] private Transform playerBody;
        [SerializeField] private float mouseSensitivity = 5f;
        [SerializeField] private float gamepadSensitivity = 150f;
        [SerializeField] private bool invertY = false;
        [SerializeField] private float maxLookAngle = 80f;
        
        private PlayerInputHandler input;
        private float xRotation = 0f;
        private Vector2 currentLookDelta;
        private Vector2 lookDeltaVelocity;
        
        // Определяем устройство
        private bool isUsingGamepad = false;
        
        private void Awake()
        {
            input = GetComponentInParent<PlayerInputHandler>();
            
            if (playerBody == null)
            {
                playerBody = transform.parent?.parent;
                if (playerBody == null)
                    Debug.LogError("PlayerCamera: playerBody not assigned!");
            }
        }
        
        private void Start()
        {
            CursorManager.Instance.Lock();
        }
        
        private void Update()
        {
            if (!input) return;
            
            HandleCameraRotation();
        }
        
        private void HandleCameraRotation()
        {
            // Определяем устройство по величине инпута
            isUsingGamepad = input.LookInput.magnitude < 0.5f && Mathf.Abs(input.LookInput.x) < 0.3f;
            
            // Выбираем чувствительность
            float currentSensitivity = isUsingGamepad ? gamepadSensitivity : mouseSensitivity;
            
            // Получаем инпут
            Vector2 targetDelta = input.LookInput * currentSensitivity;

            // Горизонтальный поворот (тело)
            if (playerBody)
            {
                float horizontal = targetDelta.x * Time.deltaTime;
                playerBody.Rotate(Vector3.up * horizontal);
            }
            
            // Вертикальный поворот (камера)
            float vertical = invertY ? targetDelta.y : -targetDelta.y;
            xRotation += vertical * Time.deltaTime;
            xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);
            
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
        
        
        public void SetSensitivity(float newSensitivity)
        {
            mouseSensitivity = Mathf.Clamp(newSensitivity, 1f, 1000f);
        }
        
        public void SetGamepadSensitivity(float newSensitivity)
        {
            gamepadSensitivity = Mathf.Clamp(newSensitivity, 1f, 500f);
        }
        
        public void SetInvertY(bool invert)
        {
            invertY = invert;
        }
        
        public void ResetCameraAngle()
        {
            xRotation = 0f;
            transform.localRotation = Quaternion.identity;
        }
    }
}