namespace Game.Scripts.Core.Interfaces
{
    public interface IWeaponUserAnimator
    {
        void PlayShoot();
        void PlayReload();
        void SetShootBool(bool value);
    }
}