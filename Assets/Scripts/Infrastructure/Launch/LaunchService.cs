using Game2048.Gameplay.Cubes;
using Game2048.Infrastructure.Audio;
using Game2048.Infrastructure.Core;
using UnityEngine;
using Zenject;

namespace Game2048.Infrastructure.Launch
{
    public class LaunchService
    {
        private readonly GameSettings _settings;
        private readonly AudioService _audioService;

        [Inject]
        public LaunchService(GameSettings settings, AudioService audioService)
        {
            _settings = settings;
            _audioService = audioService;
        }

        public void Launch(CubeView cube)
        {
            cube.Rigidbody.isKinematic = false;
            cube.Rigidbody.AddForce(Vector3.forward * _settings.LaunchForce, ForceMode.Impulse);
            _audioService.PlayLaunch();
        }
    }
}
