using System;
using System.Collections;
using System.Drawing;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;
using static UnityEditor.Progress;

namespace TurnTheTides
{

    [ExecuteInEditMode]
    public class GridManager : MonoBehaviour
    {

        [Range(1, 100)]
        public int row_length, column_length;
        [SerializeField]
        GameObject[] prefabs;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnValidate()
        {
            SmartDestroy();
        }

        public void SmartDestroy()
        {
            //Delay the delete call until after validate/update via a callback
            UnityEditor.EditorApplication.delayCall += () =>
            {
                for (int i = this.transform.childCount; i > 0; --i)
                {
                    DestroyImmediate(this.transform.GetChild(0).gameObject);
                }
                CreateHexTileGrid();
            };
        }

        private void CreateHexTileGrid()
        {

            float tileWidth = prefabs[0].GetComponent<MeshRenderer>().bounds.size.x;
            float tileHeight = prefabs[0].GetComponent<MeshRenderer>().bounds.size.z;
            float widthOffset;
            float heightOffset = (3f / 4f) * tileHeight;

            for (int column = 0; column < column_length; column++)
            {
                widthOffset = column % 2 == 1 ? tileWidth / 2 : 0;
                for (int row = 0; row < row_length; row++)
                {
                    GameObject newTile = Instantiate(
                        prefabs[UnityEngine.Random.Range(0, prefabs.Length)],
                        new Vector3(row * tileWidth + widthOffset, 0, column * heightOffset),
                        Quaternion.identity);
                    newTile.name = $"{row}, {column}";
                    newTile.transform.SetParent(this.transform);
                }
            }
        }
    }
}

