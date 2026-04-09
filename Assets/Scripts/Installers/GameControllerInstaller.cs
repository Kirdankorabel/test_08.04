using Game2048.Gameplay.GameFlow;
using Zenject;

namespace Game2048.Installers
{
    public class GameControllerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GameController>().AsSingle();
        }
    }
}
