using UnityEngine;

namespace Game.Scripts.Components.Weapons
{
    public class WeaponAnimationEvents : MonoBehaviour
    {
        [SerializeField] private BaseWeapon _weapon;

        // === Эти методы вызываются из Animation Events ===
        
        /// <summary>
        /// Вызывается когда магазин вынут (середина анимации перезарядки)
        /// </summary>
        public void AE_MagazineOut()
        {
            _weapon?.EjectMagazine();
        }

        /// <summary>
        /// Вызывается когда новый магазин вставлен
        /// </summary>
        public void AE_MagazineIn()
        {
            _weapon?.InsertMagazine();
        }

        /// <summary>
        /// Вызывается в конце анимации — перезарядка завершена
        /// </summary>
        public void AE_ReloadComplete()
        {
            _weapon?.CompleteReload();
        }
    }
}