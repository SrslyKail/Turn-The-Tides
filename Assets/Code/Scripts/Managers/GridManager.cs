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
    /// 
    /// GridManager should be a singleton, so do not instanciate it directly.
    /// All interactions with the GridManager should go through the WorldManager.
    /// If you need data that isn't available via the WorldManager, let Corey know to add it.
    /// 
    /// Made by Corey Buchan
    /// </summary>
    public class GridManager: MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> prefabs;
        private readonly List<List<GameObject>> tiles = new();
        private static GridManager _instance;
        /// <summary>
        /// Get the current instance of the GridManager. 
        /// Will create one if one does not exist.
        /// </summary>
        public static GridManager Instance
        {
            get
            {  
                if (_instance == null)
                {
                    _instance = Helper.FindOrCreateSingleton<GridManager>("Prefabs/Managers/GridManager");
                }

                return _instance;
            }
        }
        
        // CB: TODO Remove this. The WorldManager should be telling the GridManager how much to flood by so we
        //      can use pollution calculations in the future.
        private float floodIncrement;
        private static readonly Point[] odd_adjacency = new Point[6]{
            new(0, 1),
            new(-1, 0),
            new(0, -1),
            new(1, 1),
            new(1, 0),
            new(1, -1) 
        };
        private static readonly Point[] even_adjacency = new Point[6]
        {
            new(-1, 1),
            new(-1, 0),
            new(-1, -1),
            new(0, 1),
            new(1, 0),
            new(0, -1)
        };
        public int startingWaterTiles = 0;
        public int floodedTiles;
        public int totalTiles;

        /// <summary>
        /// Think of "Awake" and "Start" as creating a new object.
        /// But monobehaviors shouldn't be created with 'new' so we can use this in its place.
        /// </summary>
        private void Start()
        {
            if (Instance != null && Instance != this)
            {
                DestroyAllChildTiles();
                Helper.SmartDestroy(gameObject);
            }
        }

        /// <summary>
        /// Get a specific tile from the current hex grid.
        /// </summary>
        /// <param name="row">The 'y' coordinate of the tile you want to access.</param>
        /// <param name="col">The 'x' coordinate of the tile you want to access.</param>
        /// <returns>A GameObject encapsulating all the hextile information.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If either Row or Col are too low or high.</exception>
        public GameObject GetTile(int row, int col)
        {
            if (row < 0 || row >= tiles.Count)
            {
                throw new ArgumentOutOfRangeException($"{row} is out of range {tiles.Count}");
            }
            else if (col < 0 || col >= tiles[row].Count)
            {
                throw new ArgumentOutOfRangeException($"{col} is out of range {tiles[row].Count}");
            }

            return tiles[row][col];
        }

        public float GetFloodedRatio()
        {
            Debug.Log($"{floodedTiles} / {totalTiles}");
            return (float)floodedTiles / totalTiles;
        }

        /// <summary>
        /// Builds the map using the given mapData.
        /// Also resets any specific runtime variables,
        /// destroys all existing tiles, and creates new water meshes.
        /// </summary>
        /// <param name="mapData">The data to build the map from.</param>
        public void BuildMap(MapData mapData)
        {
            DestroyAllChildTiles();

            floodIncrement = mapData.floodIncrement;
            
            //Make the map
            CreateHexTileGrid(mapData);
            MergeWaterTiles();
        }

        private void DestroyAllChildTiles()
        {
            tiles.Clear();
            IEnumerable<Transform> children = transform.Cast<Transform>().ToList();
            //Delete all the current children
            foreach (Transform child in children)
            {
                Helper.SmartDestroy(child.gameObject);
            }
        }

        /// <summary>
        /// Creates the map to play the game on using the passed in map data.
        /// </summary>
        /// <param name="mapData">A MapData object that stores the data required to make the grid.</param>
        private void CreateHexTileGrid(MapData mapData)
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
            float heightOffset = 3f / 4f * tileHeight;
            bool offset = false;

            //Start by figuring out what row we're in
            //This changes the z-coordinate of the tile
            int mapSizeOffset = mapData.mapSizeOffset;
            for (int y = 0; y < mapData.dataRowCount; y += mapSizeOffset)
            {
                //See if we need to offset the tile
                widthOffset = offset ? tileWidth / 2 : 0;
                List<GameObject> rowList = new();
                tiles.Add(rowList);

                //For each point in the row
                //This is the x-coordinate
                for (int x = 0; x < mapData.dataColumnCount; x += mapSizeOffset)
                {

                    //Get the data from [row][item]
                    Geopoint pointData = mapData.GeoData.data[y][x];
                    GameObject newTile = Instantiate(
                        GetPrefabOfType(pointData.TerrainType),
                        new Vector3(
                            (x / mapSizeOffset * tileWidth) + widthOffset,
                            0,
                            y / mapSizeOffset * heightOffset),
                        Quaternion.identity);

                    

                    //Cleanup for terrain type. Ocean elevation should be 0 to start.
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
                    hexTile.x_index = x / mapSizeOffset;
                    hexTile.y_index = y / mapSizeOffset;

                    //Set the name and parent.
                    newTile.name = $"{x / mapSizeOffset}, {y / mapSizeOffset}";
                    newTile.transform.SetParent(this.transform);
                    rowList.Add(newTile);
                }

                offset = !offset;
            }

            totalTiles = tiles.Sum((List<GameObject> row) => { return row.Count; });
            startingWaterTiles = transform.GetComponentsInChildren<Ocean>(true).ToList().Count;
            Debug.Log($"Starting water tiles: {startingWaterTiles}");
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
        /// Flood the world.
        /// </summary>
        /// <returns>The amount of pollution freed by destroying tiles during the flood.</returns>
        public float Flood()
        {
            float freedPollution = 0f;
            
            List<GameObject> oceanTiles = transform
                .GetComponentsInChildren<Ocean>(true) // Make sure we get the inactive ocean tiles as well :)
                .Select(ocean => { return ocean.gameObject; })
                .ToList();

            //Increment the elevation for each of the ocean tiles.
            oceanTiles.ForEach(tile => tile.GetComponent<Ocean>().Elevation += floodIncrement);
            
            Queue checkQueue = new(oceanTiles);

            while (checkQueue.Count != 0)
            {
                GameObject oceanTile = checkQueue.Dequeue() as GameObject;
                HexTile details = oceanTile.GetComponent<HexTile>();
                int start_row = details.y_index;
                int start_col = details.x_index;
                foreach(Point adj in odd_adjacency)
                {
                    int check_row = adj.Y + start_row;
                    int check_col = adj.X + start_col;

                    //make sure we dont check off array
                    if (check_row >= 0
                        && check_col >= 0
                        && check_row < tiles.Count
                        && check_col < tiles[0].Count)
                    {
                        try
                        {
                            GameObject toCheck = tiles[check_row][check_col];
                            HexTile checkDetails = toCheck.GetComponent<HexTile>();
                            // If it IS an ocean tile, we ignore it.
                            if (!toCheck.TryGetComponent<Ocean>(out _) &&
                                checkDetails.Elevation < details.Elevation)
                            {
                                // TODO CB: Make the tile release its stored pollution.
                                freedPollution += 0;
                                GameObject newTile = Instantiate(oceanTile);

                                newTile.transform.parent = gameObject.transform;
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
                                Helper.SmartDestroy(toCheck);
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

            MergeWaterTiles();
            return freedPollution;
        }

        /// <summary>
        /// Calculates how much pollution the current map will create each turn.
        /// </summary>
        /// <returns></returns>
        public float CalculatePollutionPerTurn()
        {
            return transform.GetComponentsInChildren<HexTile>()
                .Sum(tile => Geopoint.PollutionMapping.GetValueOrDefault(tile.Terrain, 0));

        }

        /// <summary>
        /// "Merge" all the water tiles that are connected on the map.
        /// 
        /// Does a BFS on all the ocean tiles in the map to find the ones
        /// that are adjacent, deactivates all but one, and merges their meshes.
        /// 
        /// This enables us us, in the future, use a single mesh deformation or 
        /// texture for the water.
        /// </summary>
        public void MergeWaterTiles()
        {
            // Get all the ocean tiles.
            // Use a set to ensure it doesnt contain duplicates.
            HashSet<GameObject> oceanTiles = transform
                .GetComponentsInChildren<Ocean>(true) // Make sure we get the inactive ocean tiles as well :)
                .Select(ocean => { return ocean.gameObject; })
                .ToHashSet();
            floodedTiles = oceanTiles.Count - startingWaterTiles;
            List<List<GameObject>> oceanTrees = BFS_OceanTiles(oceanTiles);
            foreach (List<GameObject> tree in oceanTrees)
            {
                //Combine the meshes for all the found connected ocean tiles.
                CombineOceanMeshes(tree);
            }
        }

        //CB: TODO - Make this a generic function so we can do BFS on other tile types.

        /// <summary>
        /// Run a BFS on the ocean tiles to create lists of adjacent ocean tiles.
        /// </summary>
        /// <param name="oceanTiles">A Set of all ocean tiles to search over.</param>
        /// <returns>A list of lists representing the adjacent ocean tiles.</returns>
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

        // CB: TODO - Make this take a gameobject as the starting tile rather than an row/col.

        /// <summary>
        /// A helper function for the BFS function. Generates a single "tree" from the map.
        /// </summary>
        /// <param name="row">The y index of the starting tile.</param>
        /// <param name="col">the x index of the starting tile.</param>
        /// <param name="oldVisited"></param>
        /// <returns></returns>
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

            while (queue.Count != 0)
            {
                KeyValuePair<GameObject, Point> data = queue.Dequeue();
                int start_row = data.Value.X;
                int start_col = data.Value.Y;

                Point[] adjacency = start_row % 2 == 0 ? even_adjacency : odd_adjacency;
                foreach (Point adj in adjacency)
                {
                    int check_row = adj.Y + start_row;
                    int check_col = adj.X + start_col;

                    //make sure we dont check off array
                    if (check_row >= 0 && check_col >= 0
                        && check_row < tiles.Count && check_col < tiles[0].Count)
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

            return currVisited;
        }

        /// <summary>
        /// Combines the meshes of the passed in ocean tiles.
        /// </summary>
        /// <param name="toCombine">A list of ocean tiles to combine.</param>
        private void CombineOceanMeshes(List<GameObject> toCombine)
        {
            if(toCombine.Count <= 1)
            {
                return;
            }

            toCombine = toCombine.Where(tile => { return tile.activeInHierarchy; }).ToList();
            //Convert the has to an array so we can index
            MeshFilter[] meshFilters = toCombine
                .Where(tile => { return tile.activeInHierarchy; })
                .Select(tile => { return tile.GetComponent<MeshFilter>(); })
                .ToArray();

            // Create a combine instance array
            // This has the added benefit of creating the objects at the same time.
            CombineInstance[] combine = new CombineInstance[meshFilters.Length];

            // This is the object that will hold the combine mesh
            // We also need it for relative positioning of the mesh.
            GameObject oceanParent = toCombine.First();
            Mesh parentMesh = oceanParent.GetComponent<MeshFilter>().sharedMesh;
            for (int i = 0; i < meshFilters.Length; i++)
            {
                combine[i].mesh = meshFilters[i].sharedMesh;
                //Offset the current mesh WORLD position by the parents LOCAL position to effectively get the difference.
                combine[i].transform = oceanParent.transform.worldToLocalMatrix * meshFilters[i].transform.localToWorldMatrix;
                meshFilters[i].gameObject.SetActive(false);
            }


            Mesh mesh = new()
            {
                indexFormat = UnityEngine.Rendering.IndexFormat.UInt32
            };

            mesh.CombineMeshes(combine);
            mesh.name = "Ocean";
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            //Reactivate the parent.
            oceanParent.SetActive(true);
            //re-deactivate the scaler.
            oceanParent.GetComponent<HexTile>().DirtScaler.SetActive(false);
            //Update the mesh and collider.
            oceanParent.GetComponent<MeshFilter>().mesh = mesh;
            oceanParent.GetComponent<MeshCollider>().sharedMesh = mesh;
            //Update the scale
            oceanParent.transform.localScale += new Vector3(0, HexTile.height_scale_unit, 0);

            //CB: Loading the resource every time we run this function is probably a massive waste, but it works for now.
            oceanParent.GetComponent<MeshRenderer>().material = Resources.Load("WaterMaterial") as Material;
            
        }
    }
}

