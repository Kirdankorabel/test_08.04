using System;

namespace Game2048.Gameplay.Cubes
{
    [Serializable]
    public class CubeData
    {
        private int _po2Value;
        private bool _isPendingMerge;
        private bool _isLaunched;

        public int Po2Value
        {
            get => _po2Value;
            set => _po2Value = value;
        }

        public bool IsPendingMerge
        {
            get => _isPendingMerge;
            set => _isPendingMerge = value;
        }

        public bool IsLaunched
        {
            get => _isLaunched;
            set => _isLaunched = value;
        }
    }
}
