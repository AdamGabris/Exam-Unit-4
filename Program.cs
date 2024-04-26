using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace WeatherApp;

class Program
{
    static readonly HttpClient client = new HttpClient();

    static async Task Main(string[] args)
    {
        Console.Clear();
        Functions.GenerateSampleData(7);
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

        Details userMeasurements = Functions.GetUserMeasurements();


        string url = $"https://api.met.no/weatherapi/locationforecast/2.0/compact?lat={coordinates.Latitude}&lon={coordinates.Longitude}";

        client.DefaultRequestHeaders.UserAgent.ParseAdd("Exam-Unit-4 (https://github.com/AdamGabris/Exam-Unit-4)");
        try
        {
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            WeatherForecast forecast = JsonSerializer.Deserialize<WeatherForecast>(responseBody);

            Details yrMeasurements = forecast.properties.timeseries[0].data.instant.details;

            WeatherLogEntry logEntry = new WeatherLogEntry
            {
                Date = DateTime.Now,
                UserMeasurements = userMeasurements,
                YrMeasurements = yrMeasurements
            };
            Functions.SaveLogEntry(logEntry);

            //PrintCurrentDayReport();
            Functions.Print7DayReport();
            //Print30DayReport();

        }
        catch (HttpRequestException e)
        {
            Console.WriteLine("\nException Caught!");
            Console.WriteLine("Message :{0} ", e.Message);
        }



    }



}