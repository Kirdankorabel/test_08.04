using System;

namespace Game2048.Gameplay.GameFlow.States
{
    public class GameOverState : IGameState
    {
        public event Action<GameState> OnStateChangeRequested;

        public void Enter() { }

        public void Tick() { }

        public void Exit() { }
    }
}
