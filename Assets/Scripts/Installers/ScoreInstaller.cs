using Game2048.Gameplay.Scores;
using Zenject;

namespace Game2048.Installers
{
    public class ScoreInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<ScoreService>().AsSingle();
        }
    }
}
