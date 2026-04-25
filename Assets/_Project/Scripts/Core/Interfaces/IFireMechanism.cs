using System;

namespace Game.Scripts.Core.Interfaces
{
    public interface IFireMechanism
    {
        void TryFire(float fireRate, ref float nextShotTime, ref int ammo, Action performFire, out bool didFire);
    }
}