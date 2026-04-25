using Game.Scripts.Common.Enums;
using Game.Scripts.Core;
using UnityEngine;

namespace Game.Scripts.Components.Player
{
    public class PlayerCursorController : MonoBehaviour
    {
        [SerializeField] PlayerInputHandler playerInputHandler;
        
        private void Start()=> CursorManager.Instance.RequestLock(CursorUnlockRequester.Player);
        private void OnDestroy() => CursorManager.Instance.RequestUnlock(CursorUnlockRequester.Player);
    }
}
