using Game2048.Infrastructure.Audio;
using Game2048.Infrastructure.Core;
using UnityEngine;
using Zenject;

namespace Game2048.Gameplay.Cubes.Merge
{
    public class CubeMergeService
    {
        private readonly GameSettings _settings;
        private readonly SignalBus _signalBus;
        private readonly AudioService _audioService;

        public CubeMergeService(
            GameSettings settings,
            SignalBus signalBus,
            AudioService audioService)
        {
            _settings = settings;
            _signalBus = signalBus;
            _audioService = audioService;
        }

        public void Initialize()
        {
            _signalBus.Subscribe<MergeRequestSignal>(OnMergeRequest);
        }

        public void Dispose()
        {
            _signalBus.TryUnsubscribe<MergeRequestSignal>(OnMergeRequest);
        }

        private void OnMergeRequest(MergeRequestSignal signal)
        {
            TryMerge(signal.CubeA, signal.CubeB);
        }

        public void TryMerge(CubeView a, CubeView b)
        {
            if (a == null || b == null)
                return;

            if (a.Data.IsPendingMerge || b.Data.IsPendingMerge)
                return;

            if (a.Data.Po2Value != b.Data.Po2Value)
                return;

            a.Data.IsPendingMerge = true;
            b.Data.IsPendingMerge = true;

            ExecuteMerge(a, b);
        }

        private void ExecuteMerge(CubeView a, CubeView b)
        {
            var newValue = a.Data.Po2Value + b.Data.Po2Value;
            var midpoint = (a.transform.position + b.transform.position) * 0.5f;
            var combinedVelocity = a.Rigidbody.velocity + b.Rigidbody.velocity;
            var horizontalDir = new Vector3(combinedVelocity.x, 0f, combinedVelocity.z).normalized;

            b.Despawn();

            a.SetValue(newValue);
            a.Data.IsPendingMerge = false;
            a.transform.position = midpoint;
            a.transform.rotation = Quaternion.identity;
            a.Rigidbody.isKinematic = false;
            a.Rigidbody.velocity = Vector3.zero;
            a.Rigidbody.AddForce(
                (Vector3.up + horizontalDir * 0.3f) * _settings.MergePopForce,
                ForceMode.Impulse);

            _audioService.PlayMerge();

            _signalBus.Fire(new MergeCompletedSignal
            {
                ResultCube = a,
                ScoreReward = newValue / 2
            });
        }
    }
}
