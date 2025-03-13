using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;


namespace TurnTheTides
{

    /// <summary>
    /// Object to manage the state of the grid.
    /// Responsible for generating the map and tracking the bounds.
    /// Made by Corey Buchan
    /// </summary>
    [ExecuteAlways]
    public class GridManager : MonoBehaviour
    {
        [SerializeField]
        private TextAsset dataFile;
        [SerializeField]
        private List<GameObject> prefabs;

        private GeoGrid geoData; //For storing the JSON data

        private int row_count; //target is 57
        private int column_count; //target is 47

        // Start is called once before the first execution of Update
        // after the MonoBehaviour is created
        void Start()
        {
            Debug.LogWarning("Setting up map!");
            RefreshMap();
        }

        /// <summary>
        /// Resets the logic for the map, destroying all child objects, 
        /// regenerating geoData if needed, and creating the hexgrid.
        /// </summary>
        [ContextMenu("Refresh Map")]
        private void RefreshMap()
        {
            //Delete all the current children
            for (int i = gameObject.transform.childCount; i > 0; --i)
            {
                DestroyImmediate(gameObject.transform.GetChild(0).gameObject);
            }

            //If we have no data, get data
            if (geoData is null)
            {
                RefreshGeoData();
            }

            //Make the map
            CreateHexTileGrid();
        }

        /// <summary>
        /// Gets data from the JSON and updates the row/column counts.
        /// </summary>
        private void RefreshGeoData()
        {
            geoData = JSONParser.ParseFromString(dataFile.text);
            row_count = geoData.data.Count;
            column_count = geoData.data[0].Count;
            Debug.Log($"Rows:{row_count}, columns:{column_count}");
        }

        /// <summary>
        /// Using all gathered data, create the hexgrid.
        /// </summary>
        private void CreateHexTileGrid()
        {
            //All tiles should be the same size, so we can use 1 to set the defaults.
            Bounds tileBounds = prefabs[0]
                .GetComponentInChildren<MeshRenderer>()
                .bounds;

            //Get the width and height so we can calculate world pos with it.
            float tileWidth = tileBounds.size.x;
            float tileHeight = tileBounds.size.z;

            float widthOffset;
            //Offset for hexagons to sit together in staggered rows
            float heightOffset = (3f / 4f) * tileHeight; 
            bool offset = false;
            Debug.Log($"Width: {tileWidth}, Height:{tileHeight}");

            //Start by figuring out what row we're in
            //This changes the z-coordinate of the tile
            for (int y = 0; y < row_count; y+=2)
            {
                //See if we need to offset the tile
                widthOffset = offset ? tileWidth / 2 : 0;

                //For each point in the row
                //This is the x-coordinate
                for (int x = 1; x < column_count-1; x+=2)
                {

                    //Get the data from [row][item]
                    Geopoint pointData = geoData.data[y][x];
                    GameObject newTile = Instantiate(
                        GetPrefabOfType(pointData.TerrainType),
                        new Vector3(
                            x/2 * tileWidth + widthOffset,
                            0, 
                            y/2 * heightOffset),
                        Quaternion.identity);

                    //Cleanup for terrain type. Ocean elevation should be 0.
                    double dataElevation = pointData
                        .TerrainType.Equals(TerrainType.Ocean)
                        ? 0d
                        : pointData.Elevation;

                    HexTile hexTile = newTile.GetComponent<HexTile>();

                    //Set other metadata
                    hexTile.Elevation = (int)Math.Floor(dataElevation);
                    hexTile.longitude = pointData.Longitude;
                    hexTile.latitude = pointData.Latitude;

                    //Set the name and parent.
                    newTile.name = $"{x/2}, {y/2}";
                    newTile.transform.SetParent(this.gameObject.transform);
                }
                offset = !offset;
            }
        }

        /// <summary>
        /// Gets a prefab from the list of prefabs that matches the terrain type.
        /// </summary>
        /// <param name="type">The TerrainType you want to use.</param>
        /// <returns>A Prefab of that tile type.</returns>
        /// <exception cref="ArgumentException">If no tile was found.</exception>
        private GameObject GetPrefabOfType(TerrainType type)
        {
            foreach(GameObject prefab in prefabs)
            {
                if(prefab.GetComponent<HexTile>().Terrain.Equals(type))
                {
                    return prefab;
                }
            }
            throw new ArgumentException($"Could not find prefab for type {type}");
        }

        /// <summary>
        /// Gets the average elevation to help smooth out steep slopes.
        /// This may be removed later, but its useful for the current data setup.
        /// </summary>
        /// <param name="column">the X coordinate of the data</param>
        /// <param name="row">The Y coordinate of the data</param>
        /// <returns>The average elevation between this tile and its neighbors</returns>
        private double GetAverageElevation(int column, int row)
        {
            List<double> elevations = new()
            {
                geoData.data[row][column - 1].Elevation,
                geoData.data[row][column].Elevation,
                geoData.data[row][column + 1].Elevation
            };

            return elevations.Sum() / elevations.Count();
        }
    }
}

