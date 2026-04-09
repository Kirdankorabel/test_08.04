using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game2048.Gameplay.GameFlow.States
{
    public class WaitingForInputState : IGameState
    {
        private static readonly List<RaycastResult> RaycastResults = new List<RaycastResult>();

        public event Action<GameState> OnStateChangeRequested;

        public void Enter() { }

        public void Tick()
        {
            if (IsWorldTouchBegan())
            {
                OnStateChangeRequested?.Invoke(GameState.Dragging);
            }
        }

        public void Exit() { }

        private static bool IsWorldTouchBegan()
        {
#if UNITY_EDITOR
            return Input.GetMouseButtonDown(0)
                   && !IsPointerOverUI(Input.mousePosition);
#else
            if (Input.touchCount == 0)
                return false;

            var touch = Input.GetTouch(0);
            return touch.phase == TouchPhase.Began
                   && !IsPointerOverUI(touch.position);
#endif
        }

        private static bool IsPointerOverUI(Vector2 screenPosition)
        {
            if (EventSystem.current == null)
                return false;

            var eventData = new PointerEventData(EventSystem.current)
            {
                position = screenPosition
            };

            RaycastResults.Clear();
            EventSystem.current.RaycastAll(eventData, RaycastResults);

            for (var i = 0; i < RaycastResults.Count; i++)
            {
                if (RaycastResults[i].gameObject.layer == 5)
                    return true;
            }

            return false;
        }
    }
}
