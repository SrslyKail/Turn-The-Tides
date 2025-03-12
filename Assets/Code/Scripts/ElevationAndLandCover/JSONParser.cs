using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

public class Geopoint
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string LandUseLabel { get; set; }
    public double Elevation { get; set; }
}

class JSONParser
{
    static List<List<Geopoint>> Parse(String filePath)
    {
        List<List<Geopoint>> multiDimensionalArray = new List<List<Geopoint>>();

        if (!File.Exists(filePath))
        {
            Console.WriteLine("The file does not exist.");
            return null;
        }

        try
        {
            using (StreamReader file = File.OpenText(filePath))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                JsonSerializer serializer = new JsonSerializer();
                var rows = serializer.Deserialize<List<Dictionary<string, List<Geopoint>>>>(reader);

                foreach (var row in rows)
                {
                    foreach (var key in row.Keys)
                    {
                        multiDimensionalArray.Add(row[key]);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading or parsing the file: {ex.Message}");
        }

        return multiDimensionalArray;
    }

    //public static void Main(string[] args)
    //{
    //    List<List<Geopoint>> newList = JSONParser.Parse(/*Put the file path here*/);
    //    foreach (var row in newList)
    //    {
    //        foreach (var geopoint in row)
    //        {
    //            Console.WriteLine($"Latitude: {geopoint.Latitude}, Longitude: {geopoint.Longitude}, Land Use Label: {geopoint.LandUseLabel}, Elevation: {geopoint.Elevation}");
    //        }
    //    }
    //}
}