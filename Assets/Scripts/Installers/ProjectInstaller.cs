using Game2048.Infrastructure.Core;
using UnityEngine;
using Zenject;

namespace Game2048.Installers
{
    public class ProjectInstaller : MonoInstaller
    {
        [SerializeField] private GameSettings _gameSettings;

        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);
            Container.BindInstance(_gameSettings).AsSingle();
        }
    }
}
