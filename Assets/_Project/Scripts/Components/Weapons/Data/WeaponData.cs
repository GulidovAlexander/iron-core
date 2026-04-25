using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Scripts.Components.Weapons
{
    [CreateAssetMenu(menuName = "Iron Core/Weapon Data", fileName = "NewWeaponData")]
    public class WeaponData : ScriptableObject
    {
        [Header("Animation")]
        public string ShootTrigger = "Shoot";
        public string ReloadTrigger = "Reload";
        public string FireBool = "IsFiring";

        [Header("Combat")]
        public float Damage = 10f;
        [Tooltip("Выстрелов в секунду. 2 = пистолет, 10 = автомат.")]
        public float FireRate = 2f;
        public float Range = 100f;
        public int MagazineSize = 30;

        [Header("Sounds")]
        public AudioClip ShootSound;
        public AudioClip MagazineOutSound;
        public AudioClip MagazineInSound;
        public AudioClip ReloadCompleteSound;
        public AudioClip EmptyClickSound;     // звук при пустом магазине
    }
}