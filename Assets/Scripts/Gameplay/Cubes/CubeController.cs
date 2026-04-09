using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game2048.Gameplay.Board;
using Game2048.Gameplay.Cubes.AutoMerge;
using Game2048.Gameplay.Cubes.Merge;
using Game2048.Infrastructure.Audio;
using Game2048.Infrastructure.Core;
using UnityEngine;
using Zenject;

namespace Game2048.Gameplay.Cubes
{
    public class CubeController : IDisposable
    {
        private readonly CubePool _pool;
        private readonly GameSettings _settings;
        private readonly SignalBus _signalBus;
        private readonly CubeRegistry _registry;
        private readonly CubeSpawner _spawner;
        private readonly CubeMergeService _mergeService;
        private readonly AutoMergeService _autoMergeService;

        public event Action<CubeView> OnMergeCompleted;

        public IReadOnlyList<CubeView> ActiveCubes => _registry.ActiveCubes;
        public CubeView CurrentCube { get; private set; }
        public CubeView LastLaunchedCube { get; private set; }
        public bool IsAutoMergeRunning => _autoMergeService.IsRunning;

        [Inject]
        public CubeController(
            CubePool pool,
            BoardService boardService,
            GameSettings settings,
            SignalBus signalBus,
            [Inject(Id = "AutoMergeVFX")] ParticleSystem autoMergeVFXPrefab,
            AudioService audioService)
        {
            _pool = pool;
            _settings = settings;
            _signalBus = signalBus;

            _registry = new CubeRegistry();
            _spawner = new CubeSpawner(_pool, _settings, boardService, _registry);
            _mergeService = new CubeMergeService(_settings, _signalBus, audioService);
            _mergeService.Initialize();

            var vfxPlayer = new MergeVFXPlayer(autoMergeVFXPrefab, _settings);
            _autoMergeService = new AutoMergeService(vfxPlayer, new AutoMergeAnimator(), _settings, _signalBus, audioService);

            _signalBus.Subscribe<MergeCompletedSignal>(HandleMergeCompleted);
        }

        public void Dispose()
        {
            _signalBus.TryUnsubscribe<MergeCompletedSignal>(HandleMergeCompleted);
            _mergeService.Dispose();
        }

        public void Restart()
        {
            _spawner.DespawnAll();
            CurrentCube = null;
            LastLaunchedCube = null;
        }

        public CubeView Spawn()
        {
            CurrentCube = _spawner.Spawn();
            return CurrentCube;
        }

        public void LaunchCurrent()
        {
            if (CurrentCube == null)
                return;

            CurrentCube.Data.IsLaunched = true;
            LastLaunchedCube = CurrentCube;
            CurrentCube = null;
        }

        public bool TryFindMergeablePair(out CubeView a, out CubeView b)
        {
            return _registry.TryFindMergeablePair(out a, out b);
        }

        public bool IsCubeSettled(CubeView cube)
        {
            if (cube == null)
                return true;

            return cube.Rigidbody.velocity.sqrMagnitude <= _settings.SettleVelocityThreshold
                   && cube.Rigidbody.angularVelocity.sqrMagnitude <= _settings.SettleAngularThreshold;
        }

        public async UniTask ExecuteAutoMergeAsync(CancellationToken ct)
        {
            if (!_registry.TryFindMergeablePair(out var a, out var b))
                return;

            await _autoMergeService.ExecuteAutoMergeAsync(a, b, ct);
        }

        private void HandleMergeCompleted(MergeCompletedSignal signal)
        {
            OnMergeCompleted?.Invoke(signal.ResultCube);
        }
    }
}
