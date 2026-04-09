using System;
using Game2048.Gameplay.Board;
using Game2048.Gameplay.Cubes;
using Game2048.Infrastructure.Core;
using Game2048.Infrastructure.Launch;
using UnityEngine;

namespace Game2048.Gameplay.GameFlow.States
{
    public class DraggingState : IGameState
    {
        private readonly CubeController _cubeController;
        private readonly LaunchService _launchService;
        private readonly BoardService _boardService;
        private readonly GameSettings _settings;

        private Vector2 _lastPosition;

        public event Action<GameState> OnStateChangeRequested;

        public DraggingState(
            CubeController cubeController,
            LaunchService launchService,
            BoardService boardService,
            GameSettings settings)
        {
            _cubeController = cubeController;
            _launchService = launchService;
            _boardService = boardService;
            _settings = settings;
        }

        public void Enter()
        {
            _lastPosition = GetPointerPosition();
        }

        public void Tick()
        {
            var cube = _cubeController.CurrentCube;
            if (cube == null)
            {
                OnStateChangeRequested?.Invoke(GameState.WaitingForInput);
                return;
            }

            if (IsPointerHeld())
            {
                var current = GetPointerPosition();
                var deltaX = (current.x - _lastPosition.x) * _settings.DragSensitivity;
                _lastPosition = current;

                var pos = cube.transform.position;
                var halfWidth = _boardService.GetBoardHalfWidth();
                pos.x = Mathf.Clamp(pos.x + deltaX, -halfWidth, halfWidth);
                cube.transform.position = pos;
            }

            if (IsPointerUp())
            {
                _cubeController.LaunchCurrent();
                _launchService.Launch(_cubeController.LastLaunchedCube);
                OnStateChangeRequested?.Invoke(GameState.Launched);
            }
        }

        public void Exit() { }

        private static Vector2 GetPointerPosition()
        {
#if UNITY_EDITOR
            return Input.mousePosition;
#else
            return Input.touchCount > 0 ? Input.GetTouch(0).position : Vector2.zero;
#endif
        }

        private static bool IsPointerHeld()
        {
#if UNITY_EDITOR
            return Input.GetMouseButton(0);
#else
            return Input.touchCount > 0
                   && (Input.GetTouch(0).phase == TouchPhase.Moved
                       || Input.GetTouch(0).phase == TouchPhase.Stationary);
#endif
        }

        private static bool IsPointerUp()
        {
#if UNITY_EDITOR
            return Input.GetMouseButtonUp(0);
#else
            return Input.touchCount > 0
                   && (Input.GetTouch(0).phase == TouchPhase.Ended
                       || Input.GetTouch(0).phase == TouchPhase.Canceled);
#endif
        }
    }
}
