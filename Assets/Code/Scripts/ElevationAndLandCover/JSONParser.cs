using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace TurnTheTides
{
    public class Geopoint
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string LandUseLabel { get; set; }
        public double Elevation { get; set; }
    }
    public class GeoGrid
    {
        public readonly List<List<Geopoint>> data;
        public readonly double min_long;
        public readonly double max_long;
        public readonly double min_lat;
        public readonly double max_lat;

        public GeoGrid(List<List<Geopoint>> data)
        {
            this.data = data;
            double min_long = double.MaxValue;
            double max_long = double.MinValue;
            double min_lat = double.MaxValue;
            double max_lat = double.MinValue;
            foreach(List<Geopoint> row in data)
            {
                ParseRowData(ref min_long, ref max_long, ref min_lat, ref max_lat, row);
            }
            this.min_long = min_long;
            this.min_lat = min_lat;
            this.max_long = max_long;
            this.max_lat = max_lat;
        }

        public override string ToString()
        {
            return $"Lat: {min_lat}, {max_lat}\nLong: {min_long}, {max_long}";
        }

        private static void ParseRowData(
            ref double min_long,
            ref double max_long,
            ref double min_lat,
            ref double max_lat,
            List<Geopoint> row)
        {
            foreach (Geopoint point in row)
            {
                if (point.Longitude < min_long)
                {
                    min_long = point.Longitude;
                }
                else if (point.Longitude > max_long)
                {
                    max_long = point.Longitude;
                }
                if (point.Latitude < min_lat)
                {
                    min_lat = point.Latitude;
                }
                else if (point.Latitude > max_lat)
                {
                    max_lat = point.Latitude;
                }
            }
        }
    }

    class JSONParser: MonoBehaviour
    {
        public static GeoGrid ParseFromString(String input)
        {
            List<List<Geopoint>> multiDimensionalArray = new List<List<Geopoint>>();
            try
            {
                   
                JsonSerializer serializer = new JsonSerializer();
                var rows = JsonConvert.DeserializeObject<List<Dictionary<string, List<Geopoint>>>>(input);

                foreach (Dictionary<string, List<Geopoint>> rowDict in rows)
                {
                    foreach(List<Geopoint> row in rowDict.Values)
                    {
                        multiDimensionalArray.Add(row);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading or parsing the file: {ex.Message}");
            }
            

            return new GeoGrid(multiDimensionalArray);
        }
        public static List<List<Geopoint>> ParseFromFile(String filePath)
        {
            List<List<Geopoint>> multiDimensionalArray = new List<List<Geopoint>>();

            if (!File.Exists(filePath))
            {
                Console.WriteLine("The file does not exist.");
                return null;
            }

            try
            {
                using StreamReader file = File.OpenText(filePath);
                using JsonTextReader reader = new JsonTextReader(file);
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
}
