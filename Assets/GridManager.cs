using NUnit.Framework;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;
using static UnityEditor.Progress;

[ExecuteInEditMode]
public class GridManager : MonoBehaviour
{
    [SerializeField]
    int width;
    [SerializeField]
    int height;
    [SerializeField]
    GameObject[] prefabs;

    GameObject[] tiles;

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
        SmartDestroy(new GameObject());
        //Debug.LogWarning($"Currently have {this.transform.childCount} children");
        //for (int i = this.transform.childCount; i > 0; --i)
        //{
        //    //DestroyImmediate(this.transform.GetChild(0).gameObject);
        //    SmartDestroy(this.transform.GetChild(0).gameObject);
        //}
        

        //Debug.LogWarning($"After delete loop: {this.transform.childCount} children");

        ////foreach (GameObject item in tiles)
        ////{
        ////    //DestroyImmediate(item);
        ////    SmartDestroy(item);
        ////}

        //tiles = new GameObject[width * height];

        //float tileWidth = prefabs[0].GetComponent<MeshRenderer>().bounds.size.x;
        //for (int y = 0; y < height; y++)
        //{
        //    for (int x = 0; x < width; x++)
        //    {
        //        GameObject newTile = Instantiate(
        //            prefabs[UnityEngine.Random.Range(0, prefabs.Length)],
        //            new Vector3(x * tileWidth, 0, y * tileWidth), 
        //            Quaternion.identity);
        //        newTile.transform.SetParent(this.transform);
        //        tiles[x + y] = newTile;
        //    }
            
        //}


    }

    public void SmartDestroy(GameObject obj)
    {
        UnityEditor.EditorApplication.delayCall += () =>
        {
            //DestroyImmediate(obj);
            Debug.LogWarning($"Currently have {this.transform.childCount} children");
            for (int i = this.transform.childCount; i > 0; --i)
            {
                DestroyImmediate(this.transform.GetChild(0).gameObject);
                //SmartDestroy(this.transform.GetChild(0).gameObject);
            }


            Debug.LogWarning($"After delete loop: {this.transform.childCount} children");

            //foreach (GameObject item in tiles)
            //{
            //    //DestroyImmediate(item);
            //    SmartDestroy(item);
            //}

            tiles = new GameObject[width * height];

            float tileWidth = prefabs[0].GetComponent<MeshRenderer>().bounds.size.x;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    GameObject newTile = Instantiate(
                        prefabs[UnityEngine.Random.Range(0, prefabs.Length)],
                        new Vector3(x * tileWidth, 0, y * tileWidth),
                        Quaternion.identity);
                    newTile.transform.SetParent(this.transform);
                    tiles[x + y] = newTile;
                }

            }
        };
    }
}
