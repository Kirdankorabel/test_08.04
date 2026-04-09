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
        private const float PulseDuration = 0.3f;

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
                PrepareCubes(cubeA, cubeB);
                _audioService.PlayAutoMerge();

                await PlaySpiralAnimation(cubeA, cubeB, ct);

                var resultCube = MergeCubes(cubeA, cubeB);

                await _animator.ScalePulseAsync(resultCube, PulseDuration, ct);
                await WaitForSettle(resultCube, ct);
            }
            finally
            {
                IsRunning = false;
            }
        }

        private static void PrepareCubes(CubeView cubeA, CubeView cubeB)
        {
            cubeA.Data.IsPendingMerge = true;
            cubeB.Data.IsPendingMerge = true;

            cubeA.Rigidbody.isKinematic = true;
            cubeB.Rigidbody.isKinematic = true;
            cubeA.Collider.enabled = false;
            cubeB.Collider.enabled = false;
        }

        private async UniTask PlaySpiralAnimation(CubeView cubeA, CubeView cubeB, CancellationToken ct)
        {
            var midpoint = (cubeA.transform.position + cubeB.transform.position) * 0.5f;
            var risePoint = midpoint + Vector3.up * _settings.AutoMergeRiseHeight;
            var duration = _settings.AutoMergeRiseDuration + _settings.AutoMergeFlyDuration;

            await _animator.SpiralMergeAsync(
                cubeA, cubeB, risePoint, SpreadOffset,
                duration, _vfxPlayer, cubeA.Data.Po2Value, ct);
        }

        private CubeView MergeCubes(CubeView cubeA, CubeView cubeB)
        {
            var newValue = cubeA.Data.Po2Value + cubeB.Data.Po2Value;
            var mergePosition = (cubeA.transform.position + cubeB.transform.position) * 0.5f;

            cubeB.Despawn();

            cubeA.SetValue(newValue);
            cubeA.Data.IsPendingMerge = false;
            cubeA.transform.position = mergePosition;
            cubeA.transform.rotation = Quaternion.identity;
            cubeA.Collider.enabled = true;
            cubeA.Rigidbody.isKinematic = false;

            _signalBus.Fire(new MergeCompletedSignal
            {
                ResultCube = cubeA,
                ScoreReward = newValue / 2
            });

            return cubeA;
        }

        private static async UniTask WaitForSettle(CubeView cube, CancellationToken ct)
        {
            await UniTask.WaitUntil(
                () => cube.Rigidbody.velocity.sqrMagnitude < 0.05f
                      && cube.Rigidbody.angularVelocity.sqrMagnitude < 0.05f,
                cancellationToken: ct);
        }
    }
}
