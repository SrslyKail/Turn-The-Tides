
using UnityEngine;

namespace TurnTheTides
{
    /// <summary>
    /// Class that will contain the logic for the "tides" in "turn the tides".
    /// Made by Corey Buchan.
    /// </summary>
    public class Ocean: Water
    {
        private static readonly int MESH_HEIGHT_OFFSET = 1;
        public override TerrainType Terrain => TerrainType.Ocean;

        public override float Elevation
        {
            get => _elevation;
            set
            {
                base._elevation = value;

                double evaluated = ClampElevation(value + MESH_HEIGHT_OFFSET);

                Vector3 scaler = transform.localScale;
                if (value > 0)
                {
                    transform.localScale = new(
                        scaler.x,
                        (float)evaluated * height_scale_unit,
                        scaler.z);
                }

            }
        }
    }
}

