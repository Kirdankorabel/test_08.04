using Game2048.Gameplay.UI;
using UnityEngine;
using Zenject;

namespace Game2048.Installers
{
    public class UIControllerInstaller : MonoInstaller
    {
        [SerializeField] private MainMenuUI _mainMenuUI;
        [SerializeField] private GameUI _gameUI;
        [SerializeField] private GameOverUI _gameOverUI;

        public override void InstallBindings()
        {
            Container.BindInstance(_mainMenuUI).AsSingle();
            Container.BindInstance(_gameUI).AsSingle();
            Container.BindInstance(_gameOverUI).AsSingle();
            Container.Bind<UIController>().AsSingle();
            Container.BindInterfacesTo<UIMediator>().AsSingle();
        }
    }
}
