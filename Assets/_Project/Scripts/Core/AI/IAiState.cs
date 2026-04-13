namespace Game.Scripts.Core.AI
{
    public interface IAiState
    {
        AiStateId GetId();
        void Enter(IAiAgentContext agent);
        void Update(IAiAgentContext agent, float dt);
        void Exit(IAiAgentContext agent);
    }
}