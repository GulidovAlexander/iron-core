using System;
using Game.Scripts.Components.Weapons;
using UnityEngine;

namespace Game.Scripts.Components.Player
{
    public class WeaponUser: MonoBehaviour
    {
        [SerializeField] private Animator _armsAnimator;
        [SerializeField] private WeaponSlot[] _slots;

        private PlayerInputHandler _input;
        private WeaponSlot  _currentSlot;
        private int _currentIndex = 0;

        public BaseWeapon CurrentWeapon => _currentSlot?.Weapon;
        
        public event Action<BaseWeapon> OnWeaponChanged;

        private void Awake()
        {
            _input = GetComponent<PlayerInputHandler>();
        }

        private void OnEnable()
        {
            _input.OnFireStarted += HandleShoot;
            _input.OnFireStopped += HandleFireStopped;
            _input.OnReloadPressed += HandleReload;
            _input.OnWeaponSwitch += EquipSlot;
        }

        private void OnDisable()
        {
            _input.OnFireStarted -= HandleShoot;
            _input.OnFireStopped -= HandleFireStopped;
            _input.OnReloadPressed -= HandleReload;
            _input.OnWeaponSwitch -= EquipSlot;
        }

        private void Start()
        {
            EquipSlot(0);
        }

        private void EquipSlot(int index)
        {
            if(index >= _slots.Length || _slots[index].Weapon == null) return;
            
            _currentSlot?.Weapon.gameObject.SetActive(false);

            _currentIndex = index;
            _currentSlot = _slots[index];
            _currentSlot.Weapon.gameObject.SetActive(true);
            
            OnWeaponChanged?.Invoke(_currentSlot.Weapon);
        }

        private void HandleReload()
        {
            if(_currentSlot == null) return;
            
            _currentSlot.Weapon.Reload();
            
            var data = _currentSlot.Weapon.GetData();
            _armsAnimator.SetTrigger(data.ReloadTrigger);
            _currentSlot.WeaponAnimator.SetTrigger(data.ReloadTrigger);
        }

        private void HandleFireStopped()
        {
            _currentSlot?.Weapon.SetFireInput(false);
        }

        private void HandleShoot()
        {
            if(_currentSlot == null || !_currentSlot.Weapon.CanAttack) return;
            _currentSlot.Weapon.Attack();

            var data = _currentSlot.Weapon.GetData();
            _armsAnimator.SetTrigger(data.ShootTrigger);
            _currentSlot.WeaponAnimator.SetTrigger(data.ShootTrigger);
        }
    }
}