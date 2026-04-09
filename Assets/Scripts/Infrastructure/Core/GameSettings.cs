using UnityEngine;

namespace Game2048.Infrastructure.Core
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "2048/Game Settings")]
    public class GameSettings : ScriptableObject
    {
        [Header("Board")]
        [SerializeField] private float _boardWidth = 6f;

        [Header("Spawn")]
        [SerializeField] private Vector3 _spawnPosition = new Vector3(0f, 1f, -8f);
        [SerializeField] private float _spawnChanceOf2 = 0.75f;

        [Header("Launch")]
        [SerializeField] private float _launchForce = 15f;
        [SerializeField] private float _dragSensitivity = 0.01f;

        [Header("Merge")]
        [SerializeField] private float _minMergeImpulse = 2f;
        [SerializeField] private float _mergePopForce = 3f;
        [SerializeField] private float _collisionBounceForce = 1.5f;
        [SerializeField] private float _settleVelocityThreshold = 0.05f;
        [SerializeField] private float _settleAngularThreshold = 0.05f;

        [Header("Auto-Merge")]
        [SerializeField] private float _autoMergeRiseHeight = 4f;
        [SerializeField] private float _autoMergeRiseDuration = 0.5f;
        [SerializeField] private float _autoMergeFlyDuration = 0.4f;

        [Header("Game Over")]
        [SerializeField] private float _overflowLineZ = -7f;

        [Header("Cube Visuals")]
        [SerializeField] private Color[] _cubeColors;

        public float BoardWidth => _boardWidth;
        public Vector3 SpawnPosition => _spawnPosition;
        public float SpawnChanceOf2 => _spawnChanceOf2;
        public float LaunchForce => _launchForce;
        public float DragSensitivity => _dragSensitivity;
        public float MinMergeImpulse => _minMergeImpulse;
        public float MergePopForce => _mergePopForce;
        public float CollisionBounceForce => _collisionBounceForce;
        public float SettleVelocityThreshold => _settleVelocityThreshold;
        public float SettleAngularThreshold => _settleAngularThreshold;
        public float AutoMergeRiseHeight => _autoMergeRiseHeight;
        public float AutoMergeRiseDuration => _autoMergeRiseDuration;
        public float AutoMergeFlyDuration => _autoMergeFlyDuration;
        public float OverflowLineZ => _overflowLineZ;

        public Color[] CubeColors => _cubeColors;

        public Color GetColorForValue(int po2Value)
        {
            int index = Mathf.RoundToInt(Mathf.Log(po2Value, 2)) - 1;
            index = Mathf.Clamp(index, 0, _cubeColors.Length - 1);
            return _cubeColors[index];
        }
    }
}
