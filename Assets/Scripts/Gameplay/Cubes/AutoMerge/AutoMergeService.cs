using System.Threading;
using Cysharp.Threading.Tasks;
using Game2048.Gameplay.Cubes.Merge;
using Game2048.Infrastructure.Audio;
using Game2048.Infrastructure.Core;
using UnityEngine;
using Zenject;

namespace Game2048.Gameplay.Cubes.AutoMerge
{
    public class AutoMergeService
    {
        private readonly MergeVFXPlayer _vfxPlayer;
        private readonly AutoMergeAnimator _animator;
        private readonly GameSettings _settings;
        private readonly SignalBus _signalBus;
        private readonly AudioService _audioService;

        private const float SpreadOffset = 0.5f;

        public bool IsRunning { get; private set; }

        public AutoMergeService(
            MergeVFXPlayer vfxPlayer,
            AutoMergeAnimator animator,
            GameSettings settings,
            SignalBus signalBus,
            AudioService audioService)
        {
            _vfxPlayer = vfxPlayer;
            _animator = animator;
            _settings = settings;
            _signalBus = signalBus;
            _audioService = audioService;
        }

        public async UniTask ExecuteAutoMergeAsync(CubeView cubeA, CubeView cubeB, CancellationToken ct)
        {
            IsRunning = true;

            try
            {
                cubeA.Data.IsPendingMerge = true;
                cubeB.Data.IsPendingMerge = true;

                cubeA.Rigidbody.isKinematic = true;
                cubeB.Rigidbody.isKinematic = true;
                cubeA.Collider.enabled = false;
                cubeB.Collider.enabled = false;

                _audioService.PlayAutoMerge();

                var midpoint = (cubeA.transform.position + cubeB.transform.position) * 0.5f;
                var risePoint = midpoint + Vector3.up * _settings.AutoMergeRiseHeight;

                var totalDuration = _settings.AutoMergeRiseDuration + _settings.AutoMergeFlyDuration;
                var po2Value = cubeA.Data.Po2Value;

                await _animator.SpiralMergeAsync(
                    cubeA, cubeB, risePoint, SpreadOffset,
                    totalDuration, _vfxPlayer, po2Value, ct);

                var newValue = po2Value + cubeB.Data.Po2Value;

                cubeB.Despawn();

                cubeA.SetValue(newValue);
                cubeA.Data.IsPendingMerge = false;
                cubeA.transform.position = risePoint;
                cubeA.transform.rotation = Quaternion.identity;
                cubeA.Collider.enabled = true;
                cubeA.Rigidbody.isKinematic = false;

                await _animator.ScalePulseAsync(cubeA, 0.3f, ct);

                _signalBus.Fire(new MergeCompletedSignal
                {
                    ResultCube = cubeA,
                    ScoreReward = newValue / 2
                });

                await UniTask.WaitUntil(
                    () => cubeA.Rigidbody.velocity.sqrMagnitude < 0.05f
                          && cubeA.Rigidbody.angularVelocity.sqrMagnitude < 0.05f,
                    cancellationToken: ct);
            }
            finally
            {
                IsRunning = false;
            }
        }
    }
}
