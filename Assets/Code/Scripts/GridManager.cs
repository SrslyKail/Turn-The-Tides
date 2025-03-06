using System;
using System.Collections.Generic;
using UnityEngine;


namespace TurnTheTides
{

    [ExecuteInEditMode]
    public class GridManager : MonoBehaviour
    {

        [Range(1, 100)]
        public int row_length, column_length;
        [SerializeField]
        List<GameObject> prefabs;
        Dictionary<TerrainType, GameObject> types;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        //Only runs when in the editor.
        private void OnValidate()
        {
            UnityEditor.EditorApplication.delayCall += RefreshMap;
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

        //Used to
        public void RefreshMap()
        {
            //Delay the delete call until after validate/update via a callback
            for (int i = this.transform.childCount; i > 0; --i)
            {
                DestroyImmediate(this.transform.GetChild(0).gameObject);
            }
            GetTerrainTypes();
            CreateHexTileGrid();
        }

        private void CreateHexTileGrid()
        {
            if(this == null)
            {
                return;
            }
            float tileWidth = prefabs[0].GetComponent<MeshRenderer>().bounds.size.x;
            float tileHeight = prefabs[0].GetComponent<MeshRenderer>().bounds.size.z;
            float widthOffset;
            float heightOffset = (3f / 4f) * tileHeight;

            for (int column = 0; column < column_length; column++)
            {
                widthOffset = column % 2 == 1 ? tileWidth / 2 : 0;
                for (int row = 0; row < row_length; row++)
                {
                    //Logic for getting the terrain type from a tile.
                    //We can use this once have have the terrain type from the json to spawn the correct objects.
                    //Debug.Log(prefabs[UnityEngine.Random.Range(0, prefabs.Count).GetComponent<HexTile>().Terrain);
                    GameObject newTile = Instantiate(
                        prefabs[UnityEngine.Random.Range(0, prefabs.Count)],
                        new Vector3(row * tileWidth + widthOffset, 0, column * heightOffset),
                        Quaternion.identity);
                    newTile.name = $"{row}, {column}";
                    newTile.transform.SetParent(this.transform);
                }
            }
        }
    }
}

