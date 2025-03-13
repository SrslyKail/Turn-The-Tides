using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


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
        private int row_count; //target is 57
        private int column_count; //target is 47
        [SerializeField]
        private TextAsset dataFile;
        [SerializeField]
        private List<GameObject> prefabs;
        private GeoGrid geoData;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
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
            //Delay the delete call until after validate/update via a callback
            for (int i = gameObject.transform.childCount; i > 0; --i)
            {
                DestroyImmediate(gameObject.transform.GetChild(0).gameObject);
            }

            if (geoData is null)
            {
                RefreshGeoData();
            }

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
            Bounds tileBounds = prefabs[0].GetComponentInChildren<MeshRenderer>().bounds;
            float tileWidth = tileBounds.size.x;
            float tileHeight = tileBounds.size.z;
            float widthOffset;
            float heightOffset = (3f / 4f) * tileHeight;
            bool offset = false;

            for (int column = 1; column < column_count-2; column+=2)
            {
                widthOffset = offset ? tileWidth / 2 : 0;
                int indexOffset = offset ? 0 : 1;
                for (int row = 0; row < row_count/3; row++)
                {
                    GameObject prefab = getPrefabOfType(geoData.data[row][column].TerrainType);
                    GameObject newTile = Instantiate(
                        // prefabs[UnityEngine.Random.Range(0, prefabs.Count)],
                        prefab,
                        new Vector3(row * tileWidth + widthOffset, 0, column/2 * heightOffset),
                        Quaternion.identity);

                    double dataElevation = getAverageElevation(column + indexOffset, row);
                    newTile.GetComponent<HexTile>().Elevation = (int)Math.Floor(dataElevation);


                    newTile.name = $"{row}, {column / 2}";
                    newTile.transform.SetParent(this.gameObject.transform);
                }
                offset = !offset;
            }
        }

        private GameObject getPrefabOfType(TerrainType type)
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

        private double getAverageElevation(int column, int row)
        {
            List<double> elevations = new();
            elevations.Add(geoData.data[row][column - 1].Elevation);
            elevations.Add(geoData.data[row][column].Elevation);
            elevations.Add(geoData.data[row][column + 1].Elevation);

            return elevations.Sum() / elevations.Count();
        }
    }
}

