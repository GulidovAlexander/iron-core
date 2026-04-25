using System;
using Game.Scripts.Core.Interfaces;
using UnityEngine;

namespace Game.Scripts.Components.Weapons
{
    public abstract class BaseWeapon: MonoBehaviour, IWeapon
    {
        [SerializeField] protected WeaponData _data;
        [SerializeField] protected AudioSource _audioSource;
        
        protected int _currentAmmo;
        protected float _nextShotTime;
        protected bool _isReloading;
        
        public event Action OnAttacked;
        public event Action OnReloadStarted;
        public event Action OnReloadCompleted;
        public event Action<int, int> OnAmmoChanged;

        public bool CanAttack => 
            !_isReloading &&
            _currentAmmo > 0 &&
            Time.time > _nextShotTime;

        public int CurrentAmmo => _currentAmmo;
        public int MaxAmmo => _data.MagazineSize;
        public bool IsReloading => _isReloading;
        

        protected virtual void Awake() => _currentAmmo = _data.MagazineSize;

        public abstract void Attack();
        
        public virtual void Reload()
        {
            if(_isReloading || _currentAmmo == _data.MagazineSize) return;
            _currentAmmo = _data.MagazineSize;
        }
        
        public virtual void SetFireInput(bool pressed) { }

        protected void PlaySound(AudioClip clip)
        {
            if(_audioSource && clip)
                _audioSource.PlayOneShot(clip);
        }
        
        protected void NotifyAmmoChanged() => OnAmmoChanged?.Invoke(_currentAmmo, _data.MagazineSize);
        
        public WeaponData GetData() => _data;
        
        public virtual void EjectMagazine()
        {
            if (_audioSource != null && _data.MagazineOutSound != null)
                _audioSource.PlayOneShot(_data.MagazineOutSound);
        }

        public virtual void InsertMagazine()
        {
            if (_audioSource != null && _data.MagazineInSound != null)
                _audioSource.PlayOneShot(_data.MagazineInSound);
        }

        public virtual void CompleteReload()
        {
            _currentAmmo = _data.MagazineSize;
            _isReloading = false;
    
            if (_audioSource != null && _data.ReloadCompleteSound != null)
                _audioSource.PlayOneShot(_data.ReloadCompleteSound);
    
            OnAmmoChanged?.Invoke(_currentAmmo, _data.MagazineSize);
            OnReloadCompleted?.Invoke();
        }
        
        protected void NotifyAttacked() => OnAttacked?.Invoke();
    }
}