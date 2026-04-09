using Game2048.Infrastructure.Core;
using UnityEngine;
using Zenject;

namespace Game2048.Gameplay.Cubes.Merge
{
    [RequireComponent(typeof(CubeView))]
    public class CubeCollisionHandler : MonoBehaviour
    {
        private CubeView _cubeView;
        private GameSettings _settings;
        private SignalBus _signalBus;

        [Inject]
        public void Construct(GameSettings settings, SignalBus signalBus)
        {
            _settings = settings;
            _signalBus = signalBus;
        }

        private void Awake()
        {
            _cubeView = GetComponent<CubeView>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_cubeView.Data.IsPendingMerge || !_cubeView.Data.IsLaunched)
                return;

            var other = collision.gameObject.GetComponent<CubeView>();
            if (other == null || other.Data.IsPendingMerge || !other.Data.IsLaunched)
                return;

            var impulse = collision.impulse.magnitude;
            if (impulse < _settings.MinMergeImpulse)
                return;

            if (_cubeView.Data.Po2Value == other.Data.Po2Value
                && GetInstanceID() < other.GetInstanceID())
            {
                _signalBus.Fire(new MergeRequestSignal
                {
                    CubeA = _cubeView,
                    CubeB = other
                });
                return;
            }

            var bounceDir = (_cubeView.transform.position - other.transform.position).normalized;
            _cubeView.Rigidbody.AddForce(
                (Vector3.up + bounceDir * 0.3f) * _settings.CollisionBounceForce,
                ForceMode.Impulse);
        }
    }
}
