using UnityEngine;
using UnityEngine.UI;

namespace Game.Components.Health
{
    public class ArmorBarUI : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private ArmorComponent armor;

        private void Awake()
        {
            armor.OnArmorChanged += UpdateBar;
        }

        private void Start()
        {
            slider.value = armor.ArmorPercentage;
        }

        private void OnDestroy()
        {
            armor.OnArmorChanged -= UpdateBar;
        }

        private void UpdateBar(float current, float max)
        {
            slider.value = current / max;
        }
    }
}
