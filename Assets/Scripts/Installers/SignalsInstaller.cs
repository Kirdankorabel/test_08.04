using Game2048.Infrastructure.Core;
using Zenject;

namespace Game2048.Installers
{
    public class SignalsInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.DeclareSignal<MergeRequestSignal>();
            Container.DeclareSignal<MergeCompletedSignal>();
        }
    }
}
