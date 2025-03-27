
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

        public override float Elevation
        {
            get { return _elevation; }
            set
            {
                base._elevation = value;

                double evaluated = ClampElevation(value);

                Vector3 scaler = this.transform.localScale;
                if (value > 0)
                {
                    this.transform.localScale = new(
                        scaler.x,
                        (float)(evaluated) * height_scale_unit,
                        scaler.z);
                }

            }
        }

        public Ocean()
        {
            pollutionValue = 0;
        }

        private void Awake()
        {
            pollutionValue = 0;
        }
    }
}

