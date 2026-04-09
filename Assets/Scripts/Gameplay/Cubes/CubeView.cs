using System;
using Game2048.Infrastructure.Core;
using TMPro;
using UnityEngine;
using Zenject;

namespace Game2048.Gameplay.Cubes
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(BoxCollider))]
    public class CubeView : MonoBehaviour
    {
        [SerializeField] private TMP_Text[] _faceLabels;
        [SerializeField] private MeshRenderer _renderer;

        private Rigidbody _rigidbody;
        private Collider _collider;
        private GameSettings _settings;

        public event Action<CubeView> OnDespawnRequested;

        public CubeData Data { get; private set; }
        public Rigidbody Rigidbody => _rigidbody;
        public Collider Collider => _collider;

        [Inject]
        public void Construct(GameSettings settings)
        {
            _settings = settings;
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
            Data = new CubeData();
        }

        public void ResetState(int po2Value)
        {
            Data.Po2Value = po2Value;
            Data.IsPendingMerge = false;
            Data.IsLaunched = false;
            UpdateVisuals();
        }

        public void SetValue(int po2Value)
        {
            Data.Po2Value = po2Value;
            UpdateVisuals();
        }

        public void Despawn()
        {
            OnDespawnRequested?.Invoke(this);
        }

        private void UpdateVisuals()
        {
            string text = Data.Po2Value.ToString();

            for (int i = 0; i < _faceLabels.Length; i++)
            {
                if (_faceLabels[i] != null)
                    _faceLabels[i].text = text;
            }

            if (_renderer != null)
            {
                var block = new MaterialPropertyBlock();
                block.SetColor("_Color", _settings.GetColorForValue(Data.Po2Value));
                _renderer.SetPropertyBlock(block);
            }
        }
    }
}
