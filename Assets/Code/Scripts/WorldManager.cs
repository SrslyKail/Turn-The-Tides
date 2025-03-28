using Mono.Cecil;
using System;
using TurnTheTides;
using Unity.Editor.Tasks;
using UnityEditor.SearchService;
using UnityEditorInternal;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    private static WorldManager _instance;
    [SerializeField]
    private TextAsset dataFile;
    [SerializeField]
    private double _pollutionLevel;

    [Range(0.01f, 1f)]
    private float floodIncrement = 0.08f;
    private GeoGrid geoGrid;
    [SerializeField]
    private GridManager gridManager;


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
            gridManager = Resources.Load("/Prefabs/Managers/GridManager") as GridManager;
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

        if(gridManager == null)
        {
            GameObject gridManagerObj = new("Grid Manager", typeof(GridManager));
            gridManager = gridManagerObj.GetComponent<GridManager>();
            Instantiate(gridManagerObj);
        }

        //If we have no data, get data
        if (geoGrid is null)
        {
            RefreshGeoData();
        }

        PollutionLevel = 0;
    }

    /// <summary>
    /// Gets data from the JSON and updates the row/column counts.
    /// </summary>
    private void RefreshGeoData()
    {
        geoGrid = JSONParser.ParseFromString(dataFile.text);
        Debug.Log($"Rows:{geoGrid.data.Count}, columns:{geoGrid.data[0].Count}");

    }

    /// <summary>
    /// Resets the logic for the map, destroying all child objects, 
    /// regenerating geoData if needed, and creating the hexgrid.
    /// </summary>
    [ContextMenu("Refresh Game")]
    private void SetupWorld()
    {
        RefreshGeoData();
        gridManager.RefreshMap();
        //gridManager.CreateMap(geoGrid);
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
