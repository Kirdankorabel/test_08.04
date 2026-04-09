using System.Collections.Generic;

namespace Game2048.Gameplay.Cubes
{
    public class CubeRegistry
    {
        private readonly List<CubeView> _cubes = new List<CubeView>();

        public IReadOnlyList<CubeView> ActiveCubes => _cubes;

        public void Register(CubeView cube)
        {
            if (!_cubes.Contains(cube))
                _cubes.Add(cube);
        }

        public void Unregister(CubeView cube)
        {
            _cubes.Remove(cube);
        }

        public bool TryFindMergeablePair(out CubeView a, out CubeView b)
        {
            a = null;
            b = null;

            var valueMap = new Dictionary<int, CubeView>();

            for (var i = 0; i < _cubes.Count; i++)
            {
                var cube = _cubes[i];
                if (cube == null || cube.Data.IsPendingMerge || !cube.Data.IsLaunched)
                    continue;

                var value = cube.Data.Po2Value;

                if (valueMap.TryGetValue(value, out var existing))
                {
                    a = existing;
                    b = cube;
                    return true;
                }

                valueMap[value] = cube;
            }

            return false;
        }
    }
}
