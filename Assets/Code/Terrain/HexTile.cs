using System;
using UnityEngine;

namespace TurnTheTides
{
    /// <summary>
    /// Abstract base for all HexTiles to extend from.
    /// Allows us to store water and land tiles in the same lists.
    /// Controls the height and scaling for the child classes to deal with elevation representation.
    /// Made by Corey Buchan.
    /// </summary>
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
    public abstract class HexTile: MonoBehaviour
    {
        public static readonly float height_pos_unit = 0.1f;
        public static readonly float height_scale_unit = 1f;
        [SerializeField]
        private GameObject _dirtScaler;
        [SerializeField] // Allows us to see it in the editor.
        protected float _elevation;
        [SerializeField]
        private TerrainType _terrain;
        //yearly pollution amount
        protected float pollutionValue;
        //When you destroy a tile, gain this
        protected float storedPollution = 2;

        public float PollutionValue { get; }
        public float StoredPollution { get; }

        public virtual TerrainType Terrain => _terrain;
        public double longitude;
        public double latitude;
        public string landUseLabel;
        public int col_index;
        public int row_index;
        public int x_index;
        public int y_index;

        public GameObject DirtScaler
        {
            get => _dirtScaler; private set => _dirtScaler = value;
        }

        /// <summary>
        /// Elevation represents the base of the tile.
        /// For land tiles, this should never change.
        /// For water tiles, we will implement a Height attribute to track
        /// the difference between the base elevation and the depth of the water.
        /// </summary>
        public virtual float Elevation
        {
            get => _elevation;
            set
            {
                _elevation = value;
                double evaluated = ClampElevation(value);

                Vector3 curPos = transform.position;
                transform.position = new(curPos.x, (float)(evaluated * height_pos_unit), curPos.z);

                Vector3 dirtScale = DirtScaler.transform.localScale;
                if (value > 0)
                {
                    DirtScaler.transform.localScale = new(dirtScale.x, (float)(evaluated * height_scale_unit), dirtScale.z);
                }

            }
        }

        protected double ClampElevation(double elevation)
        {
            double A = 0.8; //Overall exaggeration
            double N = 0.58; //exponential factor, increase to increase differences, higher effect at higher raw elevations
            double evaluated = elevation < 0 ? -1 * Math.Pow(Math.Abs(elevation), N) : elevation > 0 ? A * Math.Pow(elevation, N) : elevation;
            return evaluated;
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