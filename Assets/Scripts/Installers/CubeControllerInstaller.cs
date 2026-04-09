using Game2048.Gameplay.Cubes;
using Zenject;

namespace Game2048.Installers
{
    public class CubeControllerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<CubeController>().AsSingle();
        }
    }
}
