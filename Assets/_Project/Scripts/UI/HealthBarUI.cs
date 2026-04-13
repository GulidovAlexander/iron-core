using UnityEngine;
using UnityEngine.UI;

namespace Game.Components.Health
{
    public class HealthBarUI : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private HealthComponent health;

        private void Awake()
        {
            health.OnHealthChanged += UpdateBar;
        }

        private void Start()
        {
            slider.value = health.HealthPercentage;
        }

        private void OnDestroy()
        {
            health.OnHealthChanged -= UpdateBar;
        }

        private void UpdateBar(float current, float max)
        {
            slider.value = current / max;
        }
    }
}
