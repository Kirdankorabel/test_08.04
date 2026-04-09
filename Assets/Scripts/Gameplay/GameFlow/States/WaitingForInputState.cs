using System;
using Game2048.Infrastructure.Input;

namespace Game2048.Gameplay.GameFlow.States
{
    public class WaitingForInputState : IGameState
    {
        private readonly TouchInputService _inputService;

        public event Action<GameState> OnStateChangeRequested;

        public WaitingForInputState(TouchInputService inputService)
        {
            _inputService = inputService;
        }

        public void Enter() { }

        public void Tick()
        {
            if (_inputService.CurrentPhase == InputPhase.Began)
            {
                OnStateChangeRequested?.Invoke(GameState.Dragging);
            }
        }

        public void Exit() { }
    }
}
