using UnityEngine;

namespace TurnTheTides
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
    public class Terrain : MonoBehaviour
    {
        public TerrainType Type { get; }

        private void Awake()
        {
            MeshCollider collider = GetComponent<MeshCollider>();
            if (collider.sharedMesh == null)
            {
                collider.sharedMesh = GetComponent<MeshFilter>().mesh;
            }
        }
    }
}

