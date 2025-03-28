using Mono.Cecil;
using System;
using TurnTheTides;
using Unity.Editor.Tasks;
using UnityEditor;
using UnityEditor.SearchService;
using UnityEditorInternal;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    private static WorldManager _instance;

    public MapData MapData
    {
        get
        {
            return data;
        }
        private set
        {
            data = value;
        }
    }
    [SerializeField]
    private double _pollutionLevel;

    [SerializeField]
    private GridManager gridManager;
    [SerializeField]
    private MapData data;

    public double PollutionLevel
    {
        get
        {
            return _pollutionLevel;
        }
        private set
        {
            _pollutionLevel = value;
        }
    }

    public readonly double PollutionMax = double.MaxValue;

    public void CreateNewLevel(TextAsset levelData, int mapSizeOffset, float flood_increment)
    {
        data = new(levelData, mapSizeOffset, flood_increment);
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if(_instance == null)
        {
            _instance = this;
        }
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

        if(gridManager == null)
        {
            GameObject gridManagerObj = new("Grid Manager", typeof(GridManager));
            gridManager = gridManagerObj.GetComponent<GridManager>();
            Instantiate(gridManagerObj);
        }
        else if(gridManager != null && gridManager.gameObject != null)
        {
            if (Application.isEditor)
            {
                DestroyImmediate(gridManager.gameObject);
            }
            else
            {
                Destroy(gridManager.gameObject);
            }
        }

        PollutionLevel = 0;
    }


    /// <summary>
    /// Resets the logic for the map, destroying all child objects, 
    /// regenerating geoData if needed, and creating the hexgrid.
    /// </summary>
    [ContextMenu("Refresh Game")]
    private void SetupWorld()
    {
        if(MapData == null)
        {
            EditorUtility.DisplayDialog(
                "No map data",
                "No map data has been given to the World Manager.", 
                "Close"
                );
        }
        else
        {
            gridManager.RefreshMap(MapData);
        }
            
    }

    public static WorldManager GetInstance()
    {
        return _instance;
    }

    public void IncreasePollution(double amount)
    {
        PollutionLevel += amount;
    }

    public void DecreasePollution(double amount)
    {
        PollutionLevel -= amount;
    }

    [ContextMenu("Next Turn")]
    public void NextTurn()
    {
        //double newPollution = gridManager.Flood(floodIncrement);
        double newPollution = gridManager.Flood();
        newPollution += gridManager.CalculateNewPollution();
        PollutionLevel += newPollution;
        Debug.Log($"New pollution: {PollutionLevel}");
    }
}
