using System.Collections.Generic;

namespace Game.Scripts.Core.AI
{
    public class AiStateMachine
    {
        private readonly Dictionary<AiStateId, IAiState> states = new();
        private IAiState currentState;
        private IAiAgentContext context;

        public AiStateId CurrentStateId => currentState?.GetId() ?? AiStateId.Idle;

        public AiStateMachine(IAiAgentContext context)
        {
            this.context = context;
        }

        public void AddState(IAiState state)
        {
            states[state.GetId()] = state;
        }

        public void SetInitialState(AiStateId id)
        {
            if (!states.TryGetValue(id, out var state)) return;
            currentState = state;
            currentState.Enter(context);
        }

        public void TransitionTo(AiStateId id)
        {
            if (!states.TryGetValue(id, out var state)) return;
            if (currentState?.GetId() == id) return;

            currentState?.Exit(context);
            currentState = state;
            currentState.Enter(context);
        }

        public void Update(float dt)
        {
            currentState?.Update(context, dt);
        }
    }
}