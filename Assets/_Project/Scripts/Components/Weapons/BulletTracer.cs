using UnityEngine;

namespace Game.Scripts.Components.Weapons
{
    public class BulletTracer : MonoBehaviour
    {
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private float _lifetime = 0.05f; // длительность видимости
        
        private float _timer;
        private bool _isActive;

        private void Awake()
        {
            if (_lineRenderer == null)
                _lineRenderer = GetComponent<LineRenderer>();
            
            _lineRenderer.positionCount = 2;
            _lineRenderer.enabled = false;
        }

        /// <summary>
        /// Показать трассер от точки start до точки end
        /// </summary>
        public void Show(Vector3 start, Vector3 end)
        {
            _lineRenderer.SetPosition(0, start);
            _lineRenderer.SetPosition(1, end);
            _lineRenderer.enabled = true;
            _timer = _lifetime;
            _isActive = true;
        }

        private void Update()
        {
            if (!_isActive) return;
            
            _timer -= Time.deltaTime;
            
            // Затухание
            float alpha = Mathf.Clamp01(_timer / _lifetime);
            Color color = _lineRenderer.startColor;
            color.a = alpha;
            _lineRenderer.startColor = color;
            _lineRenderer.endColor = color;
            
            if (_timer <= 0f)
            {
                _lineRenderer.enabled = false;
                _isActive = false;
            }
        }
    }
}