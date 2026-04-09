using Game2048.Gameplay.Board;
using Game2048.Infrastructure.Core;
using UnityEngine;

namespace Game2048.Gameplay.Cubes
{
    public class CubeSpawner
    {
        private readonly CubePool _pool;
        private readonly GameSettings _settings;
        private readonly BoardService _boardService;
        private readonly CubeRegistry _registry;

        public CubeSpawner(
            CubePool pool,
            GameSettings settings,
            BoardService boardService,
            CubeRegistry registry)
        {
            _pool = pool;
            _settings = settings;
            _boardService = boardService;
            _registry = registry;
        }

        public void DespawnAll()
        {
            for (var i = _registry.ActiveCubes.Count - 1; i >= 0; i--)
            {
                _registry.ActiveCubes[i].Despawn();
            }
        }

        public CubeView Spawn()
        {
            var value = Random.value < _settings.SpawnChanceOf2 ? 2 : 4;
            var cube = _pool.Spawn(value);

            cube.transform.position = _boardService.GetSpawnPosition();
            cube.Rigidbody.isKinematic = true;

            _registry.Register(cube);
            cube.OnDespawnRequested += HandleDespawn;

            return cube;
        }

        private void HandleDespawn(CubeView cube)
        {
            cube.OnDespawnRequested -= HandleDespawn;
            _registry.Unregister(cube);
            _pool.Despawn(cube);
        }
    }
}
