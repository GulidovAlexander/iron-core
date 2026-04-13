using UnityEngine;

namespace Game.Scripts.Core.Extensions
{
    public static class LayerMaskExtensions
    {
        public static bool HasLayer(this LayerMask mask, int layer)
        {
            return (mask & (1 << layer)) != 0;
        }
    }
}