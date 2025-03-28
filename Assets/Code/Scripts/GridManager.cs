using System;
using System.Collections;
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
        [Range(0.01f, 1f)]
        private readonly float floodIncrement = 0.08f;

        private GeoGrid geoData; //For storing the JSON data

        private int data_row_count; 
        private int data_column_count; 
        private int map_row_count; //target is 57
        private int map_column_count; //target is 47
        public readonly int map_size_offset = 2;

        readonly int[] adjacency = new int[3] { -1, 0, 1 };

        private List<List<GameObject>> tiles;
        private static GridManager _instance;


        private GridManager()
        {
            
        }

        public static GridManager GetInstance()
        {
            return _instance;
        }


        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            if (_instance != null && _instance != this)
            {
                if (Application.isEditor)
                {
                    DestroyImmediate(this.gameObject);
                }
                else
                {
                    Destroy(this.gameObject);
                }
            }
        }


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
        public void RefreshMap()
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
            map_row_count = data_row_count / map_size_offset;
            map_column_count = data_column_count / map_size_offset - 1;
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
            for (int y = 0; y < data_row_count; y += map_size_offset)
            {
                //See if we need to offset the tile
                widthOffset = offset ? tileWidth / map_size_offset : 0;
                List<GameObject> rowList = new();
                tiles.Add(rowList);

                //For each point in the row
                //This is the x-coordinate
                for (int x = 1; x < data_column_count - 1; x += map_size_offset)
                {

                    //Get the data from [row][item]
                    Geopoint pointData = geoData.data[y][x];
                    GameObject newTile = Instantiate(
                        GetPrefabOfType(pointData.TerrainType),
                        new Vector3(
                            x / map_size_offset * tileWidth + widthOffset,
                            0,
                            y / map_size_offset * heightOffset),
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
                    hexTile.landUseLabel = pointData.LandUseLabel;
                    hexTile.x_index = x;
                    hexTile.y_index = y;

                    //Set the name and parent.
                    newTile.name = $"{x / map_size_offset}, {y / map_size_offset}";
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

        [ContextMenu("Flood")]
        public float Flood()
        {
            float freedPollution = 0f;

            // Get all the ocean tiles.
            // Use a set to ensure it doesnt contain duplicates.
            List<GameObject> oceanTiles = this.transform
                .GetComponentsInChildren<Ocean>(true) // Make sure we get the inactive ocean tiles as well :)
                .Select(ocean => { return ocean.gameObject; }).ToList();

            //Increment the elevation for each of the ocean tiles.
            foreach(GameObject tile in oceanTiles)
            {
                tile.GetComponent<Ocean>().Elevation += floodIncrement;
            }

            Queue checkQueue = new(oceanTiles);
            while(checkQueue.Count != 0)
            {
                GameObject oceanTile = checkQueue.Dequeue() as GameObject;
                HexTile details = oceanTile.GetComponent<HexTile>();
                int start_row = details.y_index;
                int start_col = details.x_index;

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
                                HexTile checkDetails = toCheck.GetComponent<HexTile>();
                                // If it IS an ocean tile, we ignore it.
                                if (!checkDetails.TryGetComponent<Ocean>(out _) &&
                                    checkDetails.Elevation < details.Elevation)
                                {
                                    freedPollution += 0;
                                    GameObject newTile = Instantiate(oceanTile);

                                    newTile.transform.parent = this.gameObject.transform;
                                    newTile.transform.position = new Vector3(
                                        toCheck.transform.position.x,
                                        oceanTile.transform.position.y,
                                        toCheck.transform.position.z
                                    );

                                    HexTile newDetails = newTile.GetComponent<HexTile>();
                                    newDetails.x_index = checkDetails.x_index;
                                    newDetails.y_index = checkDetails.y_index;
                                    newDetails.Elevation = details.Elevation;

                                    newTile.transform.localScale = oceanTile.transform.localScale;
                                    newTile.name = $"Flooded {checkDetails.landUseLabel}";

                                    DestroyImmediate(toCheck);
                                    newTile.SetActive(true);

                                    tiles[check_row][check_col] = newTile;
                                    checkQueue.Enqueue(newTile);
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
            MergeWaterTiles();
            return freedPollution;
        }

        public float CalculateNewPollution()
        {
            float newPollution = 0;
            List<HexTile> children = transform.GetComponentsInChildren<HexTile>().ToList();
            foreach(HexTile tile in children)
            {
                newPollution += Geopoint.PollutionMapping.GetValueOrDefault(tile.Terrain, 0); ;
            }
            return newPollution;
        }

        public void MergeWaterTiles()
        {
            // Get all the ocean tiles.
            // Use a set to ensure it doesnt contain duplicates.
            HashSet<GameObject> oceanTiles = this.transform
                .GetComponentsInChildren<Ocean>(true) // Make sure we get the inactive ocean tiles as well :)
                .Select(ocean => { return ocean.gameObject; })
                .ToHashSet();

            List<List<GameObject>> oceanTrees = BFS_OceanTiles(oceanTiles);
            foreach(List<GameObject> tree in oceanTrees)
            {
                //Combine the meshes for all the found connected ocean tiles.
                CombineMeshes(tree);
            }
        }

        private List<List<GameObject>> BFS_OceanTiles(HashSet<GameObject> oceanTiles)
        {
            //A Hash set that contains all the "visited" water files.
            // Makes sure we don't merge multiple meshes.
            HashSet<GameObject> visited = new();

            //List of all the trees we find
            List<List<GameObject>> trees = new();

            //Iterate over the entire map.
            for (int row = 0; row < tiles.Count; row++)
            {
                for (int col = 0; col < tiles[row].Count; col++)
                {
                    GameObject obj = tiles[row][col];
                    // If we havent visted it, and it is also an ocean tile.
                    if (!visited.Contains(obj) && oceanTiles.Contains(obj))
                    {
                        //Do a BFS to get all the adjacent tiles to this one
                        trees.Add(BFS_OceanTiles_Helper(row, col, visited).ToList());                        
                    }
                }
            }
            return trees;
        }

        private HashSet<GameObject> BFS_OceanTiles_Helper(int row, int col, HashSet<GameObject> oldVisited)
        {
            GameObject startTile = tiles[row][col];
            Queue<KeyValuePair<GameObject, Point>> queue = new();

            oldVisited.Add(startTile);
            HashSet<GameObject> currVisited = new()
            {
                startTile
            };

            queue.Enqueue(
                new(startTile, new(row, col))
            );

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


        private void CombineMeshes(List<GameObject> toCombine)
        {
            //Convert the has to an array so we can index
            MeshFilter[] meshFilters = toCombine
                .Where(tile => { return tile.activeInHierarchy; })
                .Select(tile => { return tile.GetComponent<MeshFilter>();})
                .ToArray();


            // Create a combine instance array
            // This has the added benefit of creating the objects at the same time.
            CombineInstance[] combine = new CombineInstance[meshFilters.Length];

            // This is the object that will hold the combine mesh
            // We also need it for relative positioning of the mesh.
            GameObject oceanParent = toCombine.First();
            for (int i = 0; i < meshFilters.Length; i++)
            {
                combine[i].mesh = meshFilters[i].sharedMesh;
                //Offset the current mesh WORLD position by the parents LOCAL position to effectively get the difference.
                combine[i].transform = oceanParent.transform.worldToLocalMatrix * meshFilters[i].transform.localToWorldMatrix;
                meshFilters[i].gameObject.SetActive(false);
            }

            Mesh mesh = new();
            mesh.CombineMeshes(combine);
            mesh.name = "Ocean";
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();

            //Reactivate the parent.
            oceanParent.SetActive(true);
            oceanParent.GetComponent<HexTile>().DirtScaler.SetActive(false);
            oceanParent.GetComponent<MeshFilter>().mesh = mesh;
            oceanParent.transform.localScale += new Vector3(0, HexTile.height_scale_unit, 0);
            Material mat = Resources.Load("WaterMaterial") as Material;
            oceanParent.GetComponent<MeshRenderer>().material = mat;
        }
    }
}

