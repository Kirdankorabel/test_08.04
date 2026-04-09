using Game2048.Gameplay.Board;
using Game2048.Gameplay.Cubes;
using Game2048.Infrastructure.Audio;
using Game2048.Infrastructure.Launch;
using UnityEngine;
using Zenject;

namespace Game2048.Installers
{
    public class InfrastructureInstaller : MonoInstaller
    {
        [SerializeField] private CubeView _cubePrefab;
        [SerializeField] private ParticleSystem _autoMergeVFXPrefab;

        public override void InstallBindings()
        {
            Container.Bind<AudioService>().AsSingle();
            Container.Bind<LaunchService>().AsSingle();
            Container.Bind<BoardService>().AsSingle();

            Container.Bind<ParticleSystem>().WithId("AutoMergeVFX").FromInstance(_autoMergeVFXPrefab);

            Container.BindMemoryPool<CubeView, CubePool>()
                .WithInitialSize(5)
                .FromComponentInNewPrefab(_cubePrefab)
                .UnderTransformGroup("Cubes");
        }
    }
}
