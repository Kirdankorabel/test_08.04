using System.Collections.Generic;
using Game2048.Infrastructure.Core;
using UnityEngine;

namespace Game2048.Gameplay.Cubes.Merge
{
    public class MergeVFXPlayer
    {
        private readonly GameSettings _settings;
        private readonly Dictionary<int, ParticleSystem> _systems = new Dictionary<int, ParticleSystem>();
        private readonly Transform _parent;

        public MergeVFXPlayer(ParticleSystem prefab, GameSettings settings)
        {
            _settings = settings;

            _parent = new GameObject("AutoMergeVFX").transform;
            CreateSystems(prefab);
        }

        public void Cleanup()
        {
            if (_parent != null)
                Object.Destroy(_parent.gameObject);

            _systems.Clear();
        }

        public void EmitAt(Vector3 position, int po2Value, int count)
        {
            if (!_systems.TryGetValue(po2Value, out var vfx))
                return;

            var emitParams = new ParticleSystem.EmitParams
            {
                position = position,
                applyShapeToPosition = true
            };

            vfx.Emit(emitParams, count);
        }

        private void CreateSystems(ParticleSystem prefab)
        {
            var colors = _settings.CubeColors;
            for (var i = 0; i < colors.Length; i++)
            {
                var po2Value = 1 << (i + 1);
                var vfx = Object.Instantiate(prefab, _parent);
                vfx.gameObject.name = $"VFX_{po2Value}";

                var main = vfx.main;
                main.startColor = colors[i];
                main.playOnAwake = false;
                main.simulationSpace = ParticleSystemSimulationSpace.World;

                _systems[po2Value] = vfx;
            }
        }
    }
}
