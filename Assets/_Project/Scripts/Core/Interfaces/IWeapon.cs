using System;

namespace Game.Scripts.Core.Interfaces
{
    public interface IWeapon
    {
        bool CanAttack { get; }
        int CurrentAmmo { get; }
        bool IsReloading { get; }
        int MaxAmmo { get; }
        
        void Attack();
        void Reload();
        void SetFireInput(bool pressed);
        
        event Action OnAttacked;
        event Action OnReloadStarted;
        event Action OnReloadCompleted;
    }
}