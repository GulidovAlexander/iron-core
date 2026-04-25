using UnityEngine;

namespace Game.Scripts.Components.Player
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
        
        // Старт пустой — курсором управляет другой компонент
        private void Start() { }
        
        private void Update()
        {
            if (!input) return;
            HandleCameraRotation();
        }
        
        private void HandleCameraRotation()
        {
            
            // Простейшее определение геймпада (можно улучшить через InputSystem.devices)
            isUsingGamepad = input.LookInput.magnitude < 0.5f && Mathf.Abs(input.LookInput.x) < 0.3f;
            float currentSensitivity = isUsingGamepad ? gamepadSensitivity : mouseSensitivity;
            Vector2 targetDelta = input.LookInput * currentSensitivity;
            
            // Горизонтальный поворот тела
            if (playerBody)
                playerBody.Rotate(Vector3.up * (targetDelta.x * Time.deltaTime));
            
            // Вертикальный поворот камеры
            float vertical = invertY ? targetDelta.y : -targetDelta.y;
            xRotation += vertical * Time.deltaTime;
            xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
        
        public void SetSensitivity(float newSensitivity) => mouseSensitivity = Mathf.Clamp(newSensitivity, 1f, 1000f);
        public void SetGamepadSensitivity(float newSensitivity) => gamepadSensitivity = Mathf.Clamp(newSensitivity, 1f, 500f);
        public void SetInvertY(bool invert) => invertY = invert;
        public void ResetCameraAngle() { xRotation = 0f; transform.localRotation = Quaternion.identity; }
    }
}