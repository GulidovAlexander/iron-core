using Game.Components.Health;
using UnityEngine;

namespace Game.Scripts.Components.Pickups
{
    public class ArmorPickup: Pickup
    {
        [SerializeField] private float armorAmount = 25f;

        protected override bool CanPickup(GameObject collector)
        {
            if(!collector.TryGetComponent(out ArmorComponent armor)) return false;
            return !armor.IsArmorFull;
        }
        
        protected override void OnPickup(GameObject collector)
        {
            if(collector.TryGetComponent(out ArmorComponent armor))
                armor.AddArmor(armorAmount);
        }
    }
}