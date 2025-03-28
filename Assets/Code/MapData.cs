using System;
using TurnTheTides;
using UnityEngine;

[CreateAssetMenu(fileName = "MapData", menuName = "Scriptable Objects/Tides Map")]
public class MapData : ScriptableObject
{
    public GeoGrid GeoData { get; private set; }

    [SerializeField]
    private TextAsset _dataFile;
    public  int mapSizeOffset;
    public  int dataRowCount;
    public  int dataColumnCount;
    public  int mapRowCount;
    public  int mapColumnCount;

    private void OnValidate()
    {
        if(_dataFile != null)
        {
            ProcessDataFile();
        }
    }

    public MapData(
        TextAsset dataFile, 
        int map_size_offset
        )
    {
        _dataFile = dataFile;
        this.mapSizeOffset = map_size_offset;
        ProcessDataFile();
    }

    private void ProcessDataFile()
    {
        GeoData = JSONParser.ParseFromString(_dataFile.text);
        dataRowCount = GeoData.data.Count;
        dataColumnCount = GeoData.data[0].Count;
        Debug.Log($"Rows:{dataRowCount}, columns:{dataColumnCount}");
        mapRowCount = dataRowCount / this.mapSizeOffset;
        mapColumnCount = dataColumnCount / this.mapSizeOffset;
    }
}
