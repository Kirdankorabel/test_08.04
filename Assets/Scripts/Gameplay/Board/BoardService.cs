using Game2048.Infrastructure.Core;
using UnityEngine;
using Zenject;

namespace Game2048.Gameplay.Board
{
    public class BoardService
    {
        private readonly GameSettings _settings;

        [Inject]
        public BoardService(GameSettings settings)
        {
            _settings = settings;
        }

        public Vector3 GetSpawnPosition()
        {
            return _settings.SpawnPosition;
        }

        public float GetBoardHalfWidth()
        {
            return _settings.BoardWidth * 0.5f - 0.5f;
        }

        public bool IsOverflowing(Vector3 position)
        {
            return position.z < _settings.OverflowLineZ;
        }
    }
}
