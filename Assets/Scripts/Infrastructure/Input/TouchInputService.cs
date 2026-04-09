using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Game2048.Infrastructure.Input
{
    public enum InputPhase
    {
        None,
        Began,
        Moved,
        Ended
    }

    public class TouchInputService : ITickable
    {
        public InputPhase CurrentPhase { get; private set; }
        public Vector2 TouchPosition { get; private set; }
        public Vector2 TouchDelta { get; private set; }

        private bool _isOverUI;

        public void Tick()
        {
            UpdateInput();
        }

        public void ResetState()
        {
            CurrentPhase = InputPhase.None;
            TouchDelta = Vector2.zero;
            _isOverUI = false;
        }

        private void UpdateInput()
        {
#if UNITY_EDITOR
            UpdateMouseInput();
#else
            UpdateTouchInput();
#endif
        }

        private void UpdateMouseInput()
        {
            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                _isOverUI = EventSystem.current != null
                            && EventSystem.current.IsPointerOverGameObject();

                if (_isOverUI)
                {
                    CurrentPhase = InputPhase.None;
                    return;
                }

                CurrentPhase = InputPhase.Began;
                TouchPosition = UnityEngine.Input.mousePosition;
                TouchDelta = Vector2.zero;
            }
            else if (UnityEngine.Input.GetMouseButton(0))
            {
                if (_isOverUI)
                    return;

                Vector2 newPos = UnityEngine.Input.mousePosition;
                TouchDelta = newPos - TouchPosition;
                TouchPosition = newPos;
                CurrentPhase = InputPhase.Moved;
            }
            else if (UnityEngine.Input.GetMouseButtonUp(0))
            {
                if (_isOverUI)
                {
                    _isOverUI = false;
                    CurrentPhase = InputPhase.None;
                    return;
                }

                CurrentPhase = InputPhase.Ended;
            }
            else
            {
                CurrentPhase = InputPhase.None;
                TouchDelta = Vector2.zero;
            }
        }

        private void UpdateTouchInput()
        {
            if (UnityEngine.Input.touchCount == 0)
            {
                CurrentPhase = InputPhase.None;
                TouchDelta = Vector2.zero;
                return;
            }

            var touch = UnityEngine.Input.GetTouch(0);

            if (touch.phase == UnityEngine.TouchPhase.Began)
            {
                _isOverUI = EventSystem.current != null
                            && EventSystem.current.IsPointerOverGameObject(touch.fingerId);

                if (_isOverUI)
                {
                    CurrentPhase = InputPhase.None;
                    return;
                }
            }

            if (_isOverUI)
            {
                if (touch.phase == UnityEngine.TouchPhase.Ended ||
                    touch.phase == UnityEngine.TouchPhase.Canceled)
                {
                    _isOverUI = false;
                }

                CurrentPhase = InputPhase.None;
                return;
            }

            TouchPosition = touch.position;
            TouchDelta = touch.deltaPosition;

            switch (touch.phase)
            {
                case UnityEngine.TouchPhase.Began:
                    CurrentPhase = InputPhase.Began;
                    break;
                case UnityEngine.TouchPhase.Moved:
                case UnityEngine.TouchPhase.Stationary:
                    CurrentPhase = InputPhase.Moved;
                    break;
                case UnityEngine.TouchPhase.Ended:
                case UnityEngine.TouchPhase.Canceled:
                    CurrentPhase = InputPhase.Ended;
                    break;
            }
        }
    }
}
