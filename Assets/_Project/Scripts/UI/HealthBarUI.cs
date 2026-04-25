using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Components.Health
{
    public class HealthBarUI : MonoBehaviour
    {
        [Header("Images")]
        [SerializeField] private Image currentFill;
        [SerializeField] private Image delayedFill;
        
        [Header("Settings")]
        [SerializeField] private HealthComponent health;
        [SerializeField] private float _currentDuration = 0.2f;
        [SerializeField] private float _delayDuration = 0.5f;
        [SerializeField] private AnimationCurve _easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        private Coroutine _currentRoutine;
        private Coroutine _delayRoutine;
        
        private void Awake()
        {
            health.OnHealthChanged += UpdateBar;
        }

        private void Start()
        {
            currentFill.fillAmount = health.HealthPercentage;
            delayedFill.fillAmount = health.HealthPercentage;
        }

        private void OnDestroy()
        {
            health.OnHealthChanged -= UpdateBar;
        }

        private void UpdateBar(float current, float max)
        {
            var newRatio = current / max;
            
            if(_currentRoutine != null)
                StopCoroutine(_currentRoutine);
            _currentRoutine = StartCoroutine(AnimateFill(currentFill, _currentDuration, newRatio));
            
            if(_delayRoutine != null)
                StopCoroutine(_delayRoutine);
            _delayRoutine = StartCoroutine(AnimateFill(delayedFill, _delayDuration, newRatio));
        }

        private IEnumerator AnimateFill(Image fill, float duration, float targetRatio)
        {
            float startRatio = fill.fillAmount;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float curvedT = _easeCurve.Evaluate(t);
                fill.fillAmount = Mathf.Lerp(startRatio, targetRatio, curvedT);
                yield return null;
            }
            
            fill.fillAmount = targetRatio;
        }
    }
}
