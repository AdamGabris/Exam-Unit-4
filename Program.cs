using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;

class Program
{
    static readonly HttpClient client = new HttpClient();

    static async Task Main(string[] args)
    {
        Console.Clear();
        var cities = new Dictionary<string, (string Latitude, string Longitude)>
        {
            { "Grimstad", ("58.34", "8.59") },
            { "Oslo", ("59.91", "10.75") },
            { "Bergen", ("60.39", "5.32") },
            { "London", ("51.51", "-0.13") },
            { "Berlin", ("52.52", "13.41") },
            { "Paris", ("48.86", "2.35") },
            { "New York", ("40.71", "-74.01") }
        };

        string chosenCity;
        (string Latitude, string Longitude) coordinates;

        while (true)
        {
            Console.WriteLine("Please choose a city (default city = Grimstad): ");
            foreach (var city in cities.Keys)
            {
                Console.WriteLine(city);
            }
            Console.WriteLine();

            chosenCity = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(chosenCity))
            {
                coordinates = cities["Grimstad"];
                break;
            }

            if (cities.TryGetValue(chosenCity, out coordinates))
            {
                break;
            }

            Console.WriteLine("Invalid city name. Please try again.");
        }

        string url = $"https://api.met.no/weatherapi/locationforecast/2.0/compact?lat={coordinates.Latitude}&lon={coordinates.Longitude}";

        client.DefaultRequestHeaders.UserAgent.ParseAdd("Exam-Unit-4 (https://github.com/AdamGabris/Exam-Unit-4)");
        try
        {
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            WeatherForecast forecast = JsonSerializer.Deserialize<WeatherForecast>(responseBody);

            double? temperature = forecast.properties.timeseries[0].data.instant.details.air_temperature;
            double? windSpeed = forecast.properties.timeseries[0].data.instant.details.wind_speed;

            Console.WriteLine($"Temperature: {temperature}°C");
            Console.WriteLine($"Wind Speed: {windSpeed} m/s");

            File.WriteAllText("weather.json", responseBody);
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine("\nException Caught!");
            Console.WriteLine("Message :{0} ", e.Message);
        }
    }

    public class WeatherForecast
    {
        public string type { get; set; }
        public Geometry geometry { get; set; }
        public Properties properties { get; set; }
    }

    public class Geometry
    {
        public string type { get; set; }
        public List<double> coordinates { get; set; }
    }

    public class Properties
    {
        public Meta meta { get; set; }
        public List<TimeData> timeseries { get; set; }
    }

    public class Meta
    {
        public string updated_at { get; set; }
        public Units units { get; set; }
    }

    public class Units
    {
        public string air_pressure_at_sea_level { get; set; }
        public string air_temperature { get; set; }
        public string cloud_area_fraction { get; set; }
        public string precipitation_amount { get; set; }
        public string relative_humidity { get; set; }
        public string wind_from_direction { get; set; }
        public string wind_speed { get; set; }
    }

    public class TimeData
    {
        public string time { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        public Instant instant { get; set; }
        public NextHours next_12_hours { get; set; }
        public NextHours next_1_hours { get; set; }
        public NextHours next_6_hours { get; set; }
    }

    public class Instant
    {
        public Details details { get; set; }
    }

    public class NextHours
    {
        public Summary summary { get; set; }
        public Details details { get; set; }
    }

    public class Summary
    {
        public string symbol_code { get; set; }
    }

    public class Details
    {
        public double? precipitation_amount { get; set; }
        public double? air_pressure_at_sea_level { get; set; }
        public double? air_temperature { get; set; }
        public double? cloud_area_fraction { get; set; }
        public double? relative_humidity { get; set; }
        public double? wind_from_direction { get; set; }
        public double? wind_speed { get; set; }
    }
}