using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Unity.Mathematics;
using UnityEngine;

namespace TurnTheTides
{
    public class Geopoint
    {
        private static readonly Dictionary<string, TerrainType> LandUseMapping = new()
        {
            { "Wetlands", TerrainType.River },
            { "Alpine" , TerrainType.Forest },
            { "Mining", TerrainType.Barren },
            { "Young Forest", TerrainType.Forest },
            { "Urban", TerrainType.Urban },
            { "Sub alpine Avalanche Chutes", TerrainType.Barren },
            { "Agriculture", TerrainType.Farm },
            { "Fresh Water" , TerrainType.Lake },
            { "Recreation Activities", TerrainType.Forest },
            { "Estuaries", TerrainType.River },
            { "Range Lands", TerrainType.Barren },
            { "Residential Agriculture Mixtures", TerrainType.Rural },
            { "Barren Surfaces", TerrainType.Barren },
            { "Salt Water" , TerrainType.Ocean },
            { "Recently Burned", TerrainType.Barren },
            { "Old Forest" , TerrainType.Forest },
            { "Recently Logged", TerrainType.Barren },
            { "Glaciers and Snow", TerrainType.Snow },
        };

        public static readonly Dictionary<TerrainType, float> PollutionMapping = new()
        {
            {TerrainType.River, 0 },
            {TerrainType.Ocean, 0 },
            {TerrainType.Lake, 0 },
            {TerrainType.Snow, 0 },
            {TerrainType.Forest, -0.1f },
            {TerrainType.Barren, 0 },
            {TerrainType.Urban, 24000f },
            {TerrainType.Farm,  0.05f},
            {TerrainType.Rural, 0.0f }

        };

        private double _elevation;

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string LandUseLabel { get; set; }
        public double Elevation {
            get {
                return _elevation;
            }
            set { _elevation = value; }
        }
        public TerrainType TerrainType {
            get
            {
                LandUseMapping.TryGetValue(LandUseLabel, out TerrainType type);
                if(type == TerrainType.Invalid)
                {
                    Debug.LogError($"Could not find mapping for terrain type {LandUseLabel}");
                    return TerrainType.Barren;
                }
                else
                {
                    return type;
                }
            }
        }
    }
    public class GeoGrid
    {
        public readonly List<List<Geopoint>> data;

        public GeoGrid(List<List<Geopoint>> data)
        {
            this.data = data;
        }
    }

    class JSONParser: MonoBehaviour
    {
        /// <summary>
        /// Parses a JSON string and returns a GeoGrid.
        /// </summary>
        /// <param name="input">The JSON formatted string.</param>
        /// <returns>A GeoGrid.</returns>
        public static GeoGrid ParseFromString(String input)
        {
            List<List<Geopoint>> multiDimensionalArray = new();
            try
            {
                   
                JsonSerializer serializer = new();
                var rows = JsonConvert.DeserializeObject<List<Dictionary<string, List<Geopoint>>>>(input);

                foreach (Dictionary<string, List<Geopoint>> rowDict in rows)
                {
                    foreach(List<Geopoint> row in rowDict.Values)
                    {
                        multiDimensionalArray.Add(row);
                    }
                }
            }
            //TODO: Add more specific exception handling.
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading or parsing the file: {ex.Message}");
            }
            

            return new GeoGrid(multiDimensionalArray);
        }

        public static List<List<Geopoint>> ParseFromFile(String filePath)
        {
            List<List<Geopoint>> multiDimensionalArray = new();

            if (!File.Exists(filePath))
            {
                Console.WriteLine("The file does not exist.");
                return null;
            }

            try
            {
                using StreamReader file = File.OpenText(filePath);
                using JsonTextReader reader = new(file);
                JsonSerializer serializer = new();
                var rows = serializer.Deserialize<List<Dictionary<string, List<Geopoint>>>>(reader);

                foreach (var row in rows)
                {
                    foreach (var key in row.Keys)
                    {
                        multiDimensionalArray.Add(row[key]);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading or parsing the file: {ex.Message}");
            }

            return multiDimensionalArray;
        }
    }
}
