using Game.Components.Health;
using Game.Scripts.Core.Movement;
using UnityEngine;

namespace Game.Scripts.Core.AI
{
    public interface IAiAgentContext
    {
        Transform Transform { get; }
        Mover Mover { get; }
        HealthComponent Health { get; }
        Transform PlayerTransform { get; }
        float DetectionRange { get; }
        float AttackRange { get; }
    }
}