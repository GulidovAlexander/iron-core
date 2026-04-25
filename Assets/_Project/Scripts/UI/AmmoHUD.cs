using Game.Scripts.Components.Player;
using Game.Scripts.Components.Weapons;
using TMPro;
using UnityEngine;

namespace Game.Scripts.UI
{
    public class AmmoHUD : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _ammoText;      // "12 / 30"

        [SerializeField] private WeaponUser _weaponUser;
        [SerializeField] private BaseWeapon _currentWeapon;

        private void Start()
        {
            if (_weaponUser != null)
            {
                // Подписываемся на смену оружия
                _weaponUser.OnWeaponChanged += OnWeaponChanged;
                
                // Если оружие уже есть
                if (_weaponUser.CurrentWeapon != null)
                    OnWeaponChanged(_weaponUser.CurrentWeapon);
            }
        }

        private void OnDestroy()
        {
            if (_weaponUser != null)
                _weaponUser.OnWeaponChanged -= OnWeaponChanged;
            
            UnsubscribeFromWeapon();
        }

        private void OnWeaponChanged(BaseWeapon newWeapon)
        {
            UnsubscribeFromWeapon();
            
            _currentWeapon = newWeapon;
            if (_currentWeapon != null)
            {
                _currentWeapon.OnAmmoChanged += UpdateAmmoDisplay;
                
                UpdateAmmoDisplay(_currentWeapon.CurrentAmmo, _currentWeapon.MaxAmmo);
            }
        }

        private void UnsubscribeFromWeapon()
        {
            if (_currentWeapon == null) return;
            
            _currentWeapon.OnAmmoChanged -= UpdateAmmoDisplay;
        }

        private void UpdateAmmoDisplay(int current, int max)
        {
            if (_ammoText != null)
                _ammoText.text = $"{current} / {max}";

            if (_ammoText != null)
                _ammoText.color = current <= max * 0.3f ? Color.red : Color.white;
        }
    }
}
