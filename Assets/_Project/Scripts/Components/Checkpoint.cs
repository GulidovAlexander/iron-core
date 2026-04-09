using Game.Scripts.Components.Player;
using UnityEngine;

namespace Game.Scripts.Components
{
    public class Checkpoint: MonoBehaviour
    {
        private bool isActivated;
        
        private void OnTriggerEnter(Collider other)
        {
            if(isActivated) return;
            if (!other.TryGetComponent(out PlayerRespawn respawn)) return ;

            isActivated = true;
            respawn.SetSpawnPoint(transform);
        }
    }
}