using System;
using Game2048.Gameplay.Board;
using Game2048.Gameplay.Cubes;
using Game2048.Infrastructure.Core;
using Game2048.Infrastructure.Input;
using Game2048.Infrastructure.Launch;
using UnityEngine;

namespace Game2048.Gameplay.GameFlow.States
{
    public class DraggingState : IGameState
    {
        private readonly ICubeController _cubeController;
        private readonly TouchInputService _inputService;
        private readonly LaunchService _launchService;
        private readonly BoardService _boardService;
        private readonly GameSettings _settings;

        public event Action<GameState> OnStateChangeRequested;

        public DraggingState(
            ICubeController cubeController,
            TouchInputService inputService,
            LaunchService launchService,
            BoardService boardService,
            GameSettings settings)
        {
            _cubeController = cubeController;
            _inputService = inputService;
            _launchService = launchService;
            _boardService = boardService;
            _settings = settings;
        }

        public void Enter() { }

        public void Tick()
        {
            var cube = _cubeController.CurrentCube;
            if (cube == null)
            {
                OnStateChangeRequested?.Invoke(GameState.WaitingForInput);
                return;
            }

            if (_inputService.CurrentPhase == InputPhase.Moved ||
                _inputService.CurrentPhase == InputPhase.Began)
            {
                float deltaX = _inputService.TouchDelta.x * _settings.DragSensitivity;
                var pos = cube.transform.position;
                float halfWidth = _boardService.GetBoardHalfWidth();
                pos.x = Mathf.Clamp(pos.x + deltaX, -halfWidth, halfWidth);
                cube.transform.position = pos;
            }

            if (_inputService.CurrentPhase == InputPhase.Ended)
            {
                _cubeController.LaunchCurrent();
                _launchService.Launch(_cubeController.LastLaunchedCube);
                OnStateChangeRequested?.Invoke(GameState.Launched);
            }
        }

        public void Exit() { }
    }
}
