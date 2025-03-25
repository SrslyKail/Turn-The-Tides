using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

namespace TurnTheTides
{
    /// <summary>
    /// Abstract base for all HexTiles to extend from.
    /// Allows us to store water and land tiles in the same lists.
    /// Controls the height and scaling for the child classes to deal with elevation representation.
    /// Made by Corey Buchan.
    /// </summary>
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
    abstract class HexTile : MonoBehaviour
    {
        public static readonly float height_pos_unit = 0.1f;
        public static readonly float height_scale_unit = 1f;
        [SerializeField]
        private GameObject _dirtScaler;
        [SerializeField] // Allows us to see it in the editor.
        protected int _elevation;

        public abstract TerrainType Terrain { get; }
        public double longitude;
        public double latitude;
        public string landUseLabel;
        public int x_index;
        public int y_index;

        public GameObject DirtScaler
        { 
            get { return _dirtScaler; }
            private set
            {
                _dirtScaler = value;
            } 
        }

        /// <summary>
        /// Elevation represents the base of the tile.
        /// For land tiles, this should never change.
        /// For water tiles, we will implement a Height attribute to track
        /// the difference between the base elevation and the depth of the water.
        /// </summary>
        public virtual int Elevation
        {
            get { return _elevation; }
            set
            {
                _elevation = value;
                Vector3 curPos = this.transform.position;
                this.transform.position = new(curPos.x, value * height_pos_unit, curPos.z);

                Vector3 dirtScale = DirtScaler.transform.localScale;
                if(value > 0)
                {
                    DirtScaler.transform.localScale = new(dirtScale.x, value * height_scale_unit, dirtScale.z);
                }
                
            }
        }

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