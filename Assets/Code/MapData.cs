using System;
using TurnTheTides;
using UnityEngine;

[CreateAssetMenu(fileName = "MapData", menuName = "Scriptable Objects/Tides Map")]
public class MapData : ScriptableObject
{
    private TextAsset _dataFile;
    public GeoGrid GeoData { get; private set; }
    public readonly int map_size_offset;
    public readonly int data_row_count;
    public readonly int data_column_count;
    public readonly int map_row_count;
    public readonly int map_column_count;

    public TextAsset DataFile
    {
        get { return _dataFile; }
        set
        {
            TextAsset old = _dataFile;
            try
            {
                _dataFile = value;
                GeoData = JSONParser.ParseFromString(_dataFile.text);            }
            catch (Exception e)
            {
                Debug.LogException(e);
                _dataFile = old;
            }
        }
    }

    public MapData(
        TextAsset dataFile, 
        int map_size_offset
        )
    {
        DataFile = dataFile;
        this.map_size_offset = map_size_offset;
        data_row_count = GeoData.data.Count;
        data_column_count = GeoData.data[0].Count;
        Debug.Log($"Rows:{data_row_count}, columns:{data_column_count}");
        map_row_count = data_row_count / this.map_size_offset;
        map_column_count = data_column_count / this.map_size_offset;
    }
}
