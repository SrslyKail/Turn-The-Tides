using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;
using static UnityEngine.Rendering.DebugUI.Table;


namespace TurnTheTides
{

    [ExecuteAlways]
    public class GridManager : MonoBehaviour
    {

        private int row_count; //target is 57
        private int column_count; //target is 47
        [SerializeField]
        private TextAsset dataFile;
        [SerializeField]
        List<GameObject> prefabs;
        Dictionary<TerrainType, GameObject> types;
        List<List<HexTile>> hexTiles;
        GeoGrid geoData;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            Debug.LogWarning("Refreshing map!");
            RefreshMap();
        }

        // Update is called once per frame
        void Update()
        {

        }


        /// <summary>
        /// Makes a dictionary of all the terrain types and their corrosponding tiles.
        /// I am unsure of how well this will work 
        /// </summary>
        private void GetTerrainTypes()
        {
            types = new Dictionary<TerrainType, GameObject>();
            for (int i = 0; i < prefabs.Count; i++)
            {
                GameObject obj = prefabs[i];
                obj.TryGetComponent<HexTile>(out HexTile info);
                if (info != null)
                {
                    types.TryAdd(info.Terrain, prefabs[i]);
                }
            }
        }

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

            if (types is null)
            {
                GetTerrainTypes();
            }
            CreateHexTileGrid();
        }

        private void RefreshGeoData()
        {
            geoData = JSONParser.ParseFromString(dataFile.text);
            row_count = geoData.data.Count;
            column_count = geoData.data[0].Count;
            Debug.Log($"Rows:{row_count}, columns:{column_count}");
        }

        private void CreateHexTileGrid()
        {
            Bounds tileBounds = prefabs[0].GetComponentInChildren<MeshRenderer>().bounds;
            float tileWidth = tileBounds.size.x;
            float tileHeight = tileBounds.size.z;
            float widthOffset;
            float heightOffset = (3f / 4f) * tileHeight;

            for (int column = 1; column < column_count-2; column+=2)
            {
                widthOffset = column % 2 == 1 ? tileWidth / 2 : 0;
                int indexOffset = (column % 2);
                for (int row = 0; row < row_count/3; row++)
                {
                    //Logic for getting the terrain type from a tile.
                    //We can use this once have have the terrain type from the json to spawn the correct objects.
                    //Debug.Log(prefabs[UnityEngine.Random.Range(0, prefabs.Count).GetComponent<HexTile>().Terrain);
                    GameObject newTile = Instantiate(
                        prefabs[UnityEngine.Random.Range(0, prefabs.Count)],
                        new Vector3(row * tileWidth + widthOffset, 0, column * heightOffset),
                        Quaternion.identity);
                    newTile.name = $"{row}, {column-1}";
                    double dataElevation = getAverageElevation(column + indexOffset, row);
                    newTile.GetComponentInChildren<HexTile>().Elevation = (int)Math.Floor(dataElevation);
                    newTile.transform.SetParent(this.gameObject.transform);

                }
            }
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

