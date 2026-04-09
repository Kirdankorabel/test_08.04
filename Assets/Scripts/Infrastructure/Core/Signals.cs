using Game2048.Gameplay.Cubes;

namespace Game2048.Infrastructure.Core
{
    public struct MergeRequestSignal
    {
        public CubeView CubeA;
        public CubeView CubeB;
    }

    public struct MergeCompletedSignal
    {
        public CubeView ResultCube;
        public int ScoreReward;
    }
}
