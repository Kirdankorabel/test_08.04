using UnityEngine;
using Zenject;

namespace Game2048.Gameplay.Cubes
{
    public class CubePool : MemoryPool<int, CubeView>
    {
        protected override void OnCreated(CubeView item)
        {
            item.gameObject.SetActive(false);
        }

        protected override void Reinitialize(int po2Value, CubeView item)
        {
            item.transform.rotation = Quaternion.identity;
            item.Collider.enabled = true;
            item.gameObject.SetActive(true);
            item.ResetState(po2Value);
        }

        protected override void OnDespawned(CubeView item)
        {
            item.Rigidbody.velocity = Vector3.zero;
            item.Rigidbody.angularVelocity = Vector3.zero;
            item.Rigidbody.isKinematic = true;
            item.transform.rotation = Quaternion.identity;
            item.Data.IsPendingMerge = false;
            item.gameObject.SetActive(false);
        }
    }
}
