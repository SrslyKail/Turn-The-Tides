using System;
using System.Collections.Generic;
using System.Drawing;
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
        [SerializeField]
        private TextAsset dataFile;
        [SerializeField]
        private List<GameObject> prefabs;

        private GeoGrid geoData; //For storing the JSON data

        private int data_row_count; 
        private int data_column_count; 
        private int map_row_count; //target is 57
        private int map_column_count; //target is 47

        private List<List<GameObject>> tiles;

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
            tiles = new();
            //Make the map
            CreateHexTileGrid();
            MergeWaterTiles();
        }

        /// <summary>
        /// Gets data from the JSON and updates the row/column counts.
        /// </summary>
        private void RefreshGeoData()
        {
            geoData = JSONParser.ParseFromString(dataFile.text);
            data_row_count = geoData.data.Count;
            data_column_count = geoData.data[0].Count;
            Debug.Log($"Rows:{data_row_count}, columns:{data_column_count}");
            map_row_count = data_row_count / 2;
            map_column_count = data_column_count / 2 - 1;
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
            for (int y = 0; y < data_row_count; y += 2)
            {
                //See if we need to offset the tile
                widthOffset = offset ? tileWidth / 2 : 0;
                List<GameObject> rowList = new();
                tiles.Add(rowList);

                //For each point in the row
                //This is the x-coordinate
                for (int x = 1; x < data_column_count - 1; x += 2)
                {

                    //Get the data from [row][item]
                    Geopoint pointData = geoData.data[y][x];
                    GameObject newTile = Instantiate(
                        GetPrefabOfType(pointData.TerrainType),
                        new Vector3(
                            x / 2 * tileWidth + widthOffset,
                            0,
                            y / 2 * heightOffset),
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
                    hexTile.GetComponentInChildren<HexTerrain>().landUseLabel = pointData.LandUseLabel;

                    //Set the name and parent.
                    newTile.name = $"{x / 2}, {y / 2}";
                    newTile.transform.SetParent(this.gameObject.transform);
                    rowList.Add(newTile);
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
            foreach (GameObject prefab in prefabs)
            {
                if (prefab.GetComponent<HexTile>().Terrain.Equals(type))
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

        public void MergeWaterTiles()
        {
            HashSet<GameObject> oceanTiles = this.transform
                .GetComponentsInChildren<Ocean>()
                .Select(ocean => { return ocean.gameObject; })
                .ToHashSet();

            HashSet<GameObject> visited = new();
            for(int row = 0; row < tiles.Count; row++)
            {
                for(int col = 0; col < tiles[row].Count; col++)
                {
                    GameObject obj = tiles[row][col];
                    if (!visited.Contains(obj) && oceanTiles.Contains(obj))
                    {
                        HashSet<GameObject> found = BFS_OceanTiles(row, col, visited);
                        CombineMeshes(found);
                        Debug.Log($"Combined {found.Count} tiles");
                    }
                }
            }           
        }
        private void CombineMeshes(HashSet<GameObject> toCombine)
        {
            List<MeshFilter> meshFilters = toCombine
                .Select(tile => { 
                    return tile
                    .GetComponentInChildren<HexTerrain>()
                    .gameObject
                    .GetComponent<MeshFilter>(); 
                }).ToList();
            List<CombineInstance> combine = new();

            for (int i = 0;  i < meshFilters.Count; i++)
            {
                CombineInstance instance = new()
                {
                    mesh = meshFilters[i].sharedMesh,
                    transform = meshFilters[i].transform.localToWorldMatrix
                };
                combine.Add(instance);
                
                meshFilters[i].gameObject.SetActive(false);
            }

            Mesh mesh = new();
            mesh.CombineMeshes(combine.ToArray());
            GameObject oceanParent = toCombine.First();
            oceanParent.SetActive(true);

            HexTerrain terrainTile = oceanParent.GetComponentInChildren<HexTerrain>(true);
            terrainTile.gameObject.SetActive(true);
            oceanParent.AddComponent<MeshFilter>().mesh = mesh;
            terrainTile.transform.GetComponent<MeshFilter>().mesh = mesh;

        }

        private HashSet<GameObject> BFS_OceanTiles(int row, int col, HashSet<GameObject> oldVisited)
        {
            GameObject startTile = tiles[row][col];
            oldVisited.Add(startTile);
            HashSet<GameObject> currVisited = new()
            {
                startTile
            };


            Queue <KeyValuePair<GameObject, Point>> queue = new();
            queue.Enqueue(
                new(tiles[row][col], new(row, col))
                );

            int[] adjacency = new int[3] { -1, 0, 1 };

            while(queue.Count != 0)
            {
                KeyValuePair<GameObject, Point> data = queue.Dequeue();
                int start_row = data.Value.X;
                int start_col = data.Value.Y;
                
                foreach (int adj_row in adjacency)
                {
                    int check_row = adj_row + start_row;
                    foreach (int adj_col in adjacency)
                    {
                        int check_col = adj_col + start_col;

                        //make sure we dont check off array
                        if (check_row >= 0 && check_col >= 0
                            && check_row < map_row_count && check_col < map_column_count)
                        {
                            try
                            {
                                GameObject toCheck = tiles[check_row][check_col];
                                if (!oldVisited.Contains(toCheck) && toCheck.GetComponent<Ocean>())
                                {
                                    oldVisited.Add(toCheck);
                                    currVisited.Add(toCheck);
                                    queue.Enqueue(
                                        new(toCheck, new(check_row, check_col))
                                        );
                                }
                            }
                            catch (NullReferenceException)
                            {
                                Debug.LogError($"Could not find tile at {check_row}, {check_col}");
                            }
                        }
                    }
                } 
            }
            return currVisited;
        }
    }
}

