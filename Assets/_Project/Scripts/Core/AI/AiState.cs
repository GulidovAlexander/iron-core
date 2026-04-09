using UnityEngine;

namespace Game.Scripts.Core.AI
{
    public abstract class AiState : IAiState
    {
        protected float EnterTime { get; private set; }
        protected float Elapsed => Time.time - EnterTime;

        public abstract AiStateId GetId();

        public virtual void Enter(IAiAgentContext agent)
        {
            EnterTime = Time.time;
        }

        public virtual void Update(IAiAgentContext agent, float dt) { }
        public virtual void Exit(IAiAgentContext agent) { }

        protected bool Timeout(float t) => Elapsed >= t;

        protected void FaceTowards(IAiAgentContext agent, Vector3 worldPoint, float turnSpeed = 720f)
        {
            var dir = worldPoint - agent.Transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude < 1e-6f) return;

            var targetRot = Quaternion.LookRotation(dir.normalized, Vector3.up);
            agent.Transform.rotation = Quaternion.RotateTowards(
                agent.Transform.rotation, targetRot, turnSpeed * Time.deltaTime);
        }
    }
}