
using UnityEngine;

namespace TurnTheTides
{
    /// <summary>
    /// Class that will contain the logic for the "tides" in "turn the tides".
    /// Made by Corey Buchan.
    /// </summary>
    class Ocean : Water
    {
        public override TerrainType Terrain => TerrainType.Ocean;

        public override int Elevation
        {
            get { return _elevation; }
            set
            {
                _elevation = value;

                Vector3 scaler = this.transform.localScale;
                if (value > 0)
                {
                    this.transform.localScale = new(
                        scaler.x,
                        (value + 1 + height_scale_unit) * height_scale_unit,
                        scaler.z);
                }

            }
        }
    }
}

