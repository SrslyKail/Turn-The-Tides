using UnityEngine;

namespace TurnTheTides
{
    /// <summary>
    /// Script that all hextiles should have on a child object.
    /// Represents and contains the graphical elements on top of the core hex tile.
    /// Ensures our objects have the required components
    /// Made by Corey Buchan
    /// </summary>
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

