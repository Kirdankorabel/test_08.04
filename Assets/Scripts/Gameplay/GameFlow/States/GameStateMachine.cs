using System.Collections.Generic;

namespace Game2048.Gameplay.GameFlow
{
    public enum GameState
    {
        WaitingForInput,
        Dragging,
        Launched,
        AutoMerging,
        GameOver
    }

    public interface IGameStateListener
    {
        public void OnGameStateChanged(GameState state);
    }
}

namespace Game2048.Gameplay.GameFlow.States
{
    public class GameStateMachine
    {
        private readonly Dictionary<GameState, IGameState> _states = new Dictionary<GameState, IGameState>();
        private readonly List<IGameStateListener> _listeners = new List<IGameStateListener>();
        private IGameState _currentState;

        public GameState CurrentState { get; private set; }

        public void AddState(GameState key, IGameState state)
        {
            state.OnStateChangeRequested += ChangeState;
            _states[key] = state;
        }

        public void AddListener(IGameStateListener listener)
        {
            _listeners.Add(listener);
        }

        public void ChangeState(GameState newState)
        {
            _currentState?.Exit();
            CurrentState = newState;
            _currentState = _states[newState];
            _currentState.Enter();

            for (var i = 0; i < _listeners.Count; i++)
            {
                _listeners[i].OnGameStateChanged(newState);
            }
        }

        public void Tick()
        {
            _currentState?.Tick();
        }
    }
}
