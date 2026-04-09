using System;
using Game2048.Gameplay.Cubes;
using Game2048.Infrastructure.Core;
using UnityEngine;

namespace Game2048.Gameplay.GameFlow.States
{
    public class LaunchedState : IGameState
    {
        private readonly ICubeController _cubeController;
        private readonly GameOverDetector _gameOverDetector;

        private CubeView _trackedCube;
        private float _settleTimer;
        private const float SettleDelay = 0.5f;

        public event Action<GameState> OnStateChangeRequested;

        public LaunchedState(
            ICubeController cubeController,
            GameOverDetector gameOverDetector)
        {
            _cubeController = cubeController;
            _gameOverDetector = gameOverDetector;
        }

        public void Enter()
        {
            _trackedCube = _cubeController.LastLaunchedCube;
            _settleTimer = 0f;
            _cubeController.OnMergeCompleted += HandleMergeCompleted;
        }

        public void Tick()
        {
            if (!_cubeController.IsCubeSettled(_trackedCube))
            {
                _settleTimer = 0f;
                return;
            }

            _settleTimer += Time.deltaTime;
            if (_settleTimer < SettleDelay)
                return;

            if (_gameOverDetector.CheckGameOver())
            {
                OnStateChangeRequested?.Invoke(GameState.GameOver);
                return;
            }

            _cubeController.Spawn();
            OnStateChangeRequested?.Invoke(GameState.WaitingForInput);
        }

        public void Exit()
        {
            _cubeController.OnMergeCompleted -= HandleMergeCompleted;
            _trackedCube = null;
        }

        private void HandleMergeCompleted(CubeView resultCube)
        {
            _trackedCube = resultCube;
            _settleTimer = 0f;
        }
    }
}
