using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game2048.Gameplay.Cubes
{
    public interface ICubeController
    {
        public event Action<CubeView> OnMergeCompleted;

        public IReadOnlyList<CubeView> ActiveCubes { get; }

        public CubeView CurrentCube { get; }

        public CubeView LastLaunchedCube { get; }

        public bool IsAutoMergeRunning { get; }

        public CubeView Spawn();

        public void LaunchCurrent();

        public bool TryFindMergeablePair(out CubeView a, out CubeView b);

        public bool IsCubeSettled(CubeView cube);

        public UniTask ExecuteAutoMergeAsync(CancellationToken ct);

        public void Restart();
    }
}
