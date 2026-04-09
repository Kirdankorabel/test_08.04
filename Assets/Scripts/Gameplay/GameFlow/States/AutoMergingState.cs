using System;

namespace Game2048.Gameplay.GameFlow.States
{
    public class AutoMergingState : IGameState
    {
        public event Action<GameState> OnStateChangeRequested;

        public void Enter() { }

        public void Tick() { }

        public void Exit() { }
    }
}
