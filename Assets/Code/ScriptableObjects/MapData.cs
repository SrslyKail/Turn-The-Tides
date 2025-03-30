using UnityEngine;

namespace TurnTheTides
{

    /// <summary>
    /// Holds all the data required to create a map in Turn The Tides.
    /// Should be used by the <c>WorldManager</c> to create a new map.
    /// </summary>
    /// <remarks>
    /// Made by Corey Buchan.
    /// </remarks>
    [CreateAssetMenu(fileName = "MapData", menuName = "Scriptable Objects/Tides Map")]
    public class MapData: ScriptableObject
    {
        /// <summary>
        /// The GeoGrid data for this Map.
        /// </summary>
        /// <seealso cref="GeoGrid"/>
        public GeoGrid GeoData { get; private set; }
        public const float DEFAULT_FLOOD_INCREMENT = 0.01f;

        [SerializeField]
        private TextAsset _dataFile;
        public int mapSizeOffset;
        public int dataRowCount;
        public int dataColumnCount;
        public int mapRowCount;
        public int mapColumnCount;
        public float floodIncrement = DEFAULT_FLOOD_INCREMENT;

        /// <summary>
        /// Allows us to have real-time updating within the editor.
        /// OnValidate is called whenever you change something and the editor doesn't break.
        /// </summary>
        private void OnValidate()
        {
            if (_dataFile != null)
            {
                ProcessDataFile();
            }
        }

        /// <summary>
        /// Effectively the constructor for the MapData. 
        /// <para>
        /// Allows you to insert all the data used to create new maps at runtime.
        /// </para>
        /// </summary>
        /// <param name="levelData">a JSON containing the data for the hexgrid.</param>
        /// <param name="mapSizeOffset">A scaler for the total map size. Effectively only uses every n tiles in each row and column.</param>
        /// <param name="flood_increment">The base amount you want the waters to rise by each flood action. Must be > 0, otherwise defaults to 0.01</param>
        public void LoadData(
            TextAsset dataFile,
            int map_size_offset,
            float flood_increment = DEFAULT_FLOOD_INCREMENT
            )
        {
            _dataFile = dataFile;
            mapSizeOffset = map_size_offset;

            if (flood_increment > 0)
            {
                floodIncrement = flood_increment;
            }

            ProcessDataFile();
        }

        /// <summary>
        /// Processes the currently stored DataFile and sets up the variables required based on the information retrieved.
        /// </summary>
        private void ProcessDataFile()
        {
            GeoData = JSONParser.ParseFromString(_dataFile.text);
            dataRowCount = GeoData.data.Count;
            dataColumnCount = GeoData.data[0].Count;
            mapRowCount = dataRowCount / mapSizeOffset;
            mapColumnCount = dataColumnCount / mapSizeOffset;
        }
    }
}
