using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

/// <summary>
/// !!!Deprecated!!!
/// Data is collected through a python script instead.
/// Ben made this.
/// </summary>
public class ElevationPoint
{
    public double Altitude { get; set; }
    public bool Vertex { get; set; }
    public Geometry Geometry { get; set; }
}

public class Geometry
{
    public string Type { get; set; }
    public List<double> Coordinates { get; set; }
}

public class Program
{
    private static readonly HttpClient client = new();

    public static async Task Main(string[] args)
    {
        double startLongitude = -123.18;
        double startLatitude = 49.5;
        double endLongitude = -121.25;
        double endLatitude = 49;

        int longitudeSteps = 168;
        int latitudeSteps = 60;

        List<ElevationPoint[]> allRows = new();

        double latitudeIncrement = (endLatitude - startLatitude) / (latitudeSteps + 1);

        for (int i = 0; i < latitudeSteps; i++)
        {
            double currentLongitude = startLongitude;
            double currentLatitude = startLatitude + (i * latitudeIncrement);

            string path = $"LINESTRING({startLongitude} {currentLatitude}, {endLongitude} {currentLatitude})";

            string apiUrl = $"https://geogratis.gc.ca/services/elevation/cdem/profile?path={Uri.EscapeDataString(path)}&steps={longitudeSteps}";

            var row = await GetElevationProfileAsync(apiUrl, i);
            if (row != null)
            {
                allRows.Add(row);
            }
        }

        PrintGrid(allRows);
        SaveResultsAsJson(allRows, "elevation_data.json");
    }

    public static async Task<ElevationPoint[]> GetElevationProfileAsync(string apiUrl, int step)
    {
        try
        {
            Console.WriteLine("Pinging api step:" + step);
            HttpResponseMessage response = await client.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();

            ElevationPoint[] elevationPoints = JsonConvert.DeserializeObject<ElevationPoint[]>(responseBody);
            return elevationPoints;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error occurred while fetching data: {ex.Message}");
            return null;
        }
    }

    public static void PrintGrid(List<ElevationPoint[]> allRows)
    {
        Console.WriteLine("Index\tLongitude\tLatitude\tAltitude");

        for (int i = 0; i < allRows.Count; i++)
        {
            var row = allRows[i];
            foreach (var point in row)
            {
                string longitude = point.Geometry.Coordinates[0].ToString("F4");
                string latitude = point.Geometry.Coordinates[1].ToString("F4");
                string altitude = point.Altitude.ToString("F2");
                Console.WriteLine($"{i + 1}\t{longitude}\t{latitude}\t{altitude}");
            }
        }
    }

    public static void SaveResultsAsJson(List<ElevationPoint[]> allRows, string fileName)
    {
        try
        {
            string json = JsonConvert.SerializeObject(allRows, Formatting.Indented);
            File.WriteAllText(fileName, json);
            Console.WriteLine($"Results saved to {fileName}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error occurred while saving the JSON file: {ex.Message}");
        }
    }
}
