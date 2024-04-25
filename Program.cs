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
        GenerateSampleData(7);
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

        Details userMeasurements = GetUserMeasurements();


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
            SaveLogEntry(logEntry);

            //PrintCurrentDayReport();
            Print7DayReport();
            //Print30DayReport();

        }
        catch (HttpRequestException e)
        {
            Console.WriteLine("\nException Caught!");
            Console.WriteLine("Message :{0} ", e.Message);
        }



    }

    public static Details GetUserMeasurements()
    {
        Details userMeasurements = new Details();

        Console.Write("Enter air temperature (°C): ");
        userMeasurements.air_temperature = double.Parse(Console.ReadLine());

        Console.Write("Enter wind speed (m/s): ");
        userMeasurements.wind_speed = double.Parse(Console.ReadLine());

        Console.Write("Enter relative humidity (%): ");
        userMeasurements.relative_humidity = double.Parse(Console.ReadLine());

        return userMeasurements;
    }

    public static void SaveLogEntry(WeatherLogEntry logEntry)
    {
        // Read the existing entries
        List<WeatherLogEntry> entries;
        if (File.Exists("weatherLogEntries.json"))
        {
            string entriesJson = File.ReadAllText("weatherLogEntries.json");
            entries = JsonSerializer.Deserialize<List<WeatherLogEntry>>(entriesJson);
        }
        else
        {
            entries = new List<WeatherLogEntry>();
        }

        // Add the new entry
        entries.Add(logEntry);

        // Save the entries
        string newEntriesJson = JsonSerializer.Serialize(entries);
        File.WriteAllText("weatherLogEntries.json", newEntriesJson);
    }

    public static void Print7DayReport()
    {
        PrintReport(7);
    }

    public static void Print30DayReport()
    {
        PrintReport(30);
    }

    public static void PrintReport(int days)
    {
        // Read the entries
        string entriesJson = File.ReadAllText("weatherLogEntries.json");
        List<WeatherLogEntry> entries = JsonSerializer.Deserialize<List<WeatherLogEntry>>(entriesJson);

        // Filter the entries for the past days
        DateTime startDate = DateTime.Now.AddDays(-days);
        List<WeatherLogEntry> filteredEntries = entries.Where(e => e.Date >= startDate).ToList();

        Console.WriteLine($"\n{days}-Day Weather Report:");

        foreach (var entry in filteredEntries)
        {
            // Calculate the differences
            double temperatureDifference = Math.Round((double)(entry.UserMeasurements.air_temperature - entry.YrMeasurements.air_temperature), 1);
            double windSpeedDifference = Math.Round((double)(entry.UserMeasurements.wind_speed - entry.YrMeasurements.wind_speed), 1);
            double humidityDifference = Math.Round((double)(entry.UserMeasurements.relative_humidity - entry.YrMeasurements.relative_humidity), 1);

            // Print the measurements and differences
            Console.WriteLine($"\nDate: {entry.Date.ToShortDateString()}");
            Console.WriteLine("\nUser's Measurements:");
            Console.WriteLine($"Air Temperature: {Math.Round((double)entry.UserMeasurements.air_temperature, 1)}°C");
            Console.WriteLine($"Wind Speed: {Math.Round((double)entry.UserMeasurements.wind_speed, 1)} m/s");
            Console.WriteLine($"Relative Humidity: {Math.Round((double)entry.UserMeasurements.relative_humidity, 1)}%");
            Console.WriteLine("\nYR's Measurements:");
            Console.WriteLine($"Air Temperature: {Math.Round((double)entry.YrMeasurements.air_temperature, 1)}°C");
            Console.WriteLine($"Wind Speed: {Math.Round((double)entry.YrMeasurements.wind_speed, 1)} m/s");
            Console.WriteLine($"Relative Humidity: {Math.Round((double)entry.YrMeasurements.relative_humidity, 1)}%");
            Console.WriteLine($"\nTemperature Difference: {temperatureDifference}°C");
            Console.WriteLine($"Wind Speed Difference: {windSpeedDifference} m/s");
            Console.WriteLine($"Humidity Difference: {humidityDifference}%");
        }
    }

    public static void PrintCurrentDayReport()
    {
        // Read the entries
        string entriesJson = File.ReadAllText("weatherLogEntries.json");
        List<WeatherLogEntry> entries = JsonSerializer.Deserialize<List<WeatherLogEntry>>(entriesJson);

        // Find the entry for the current day
        DateTime today = DateTime.Now.Date;
        WeatherLogEntry todayEntry = entries.Find(e => e.Date.Date == today);

        if (todayEntry != null)
        {
            // Print the measurements and differences
            Console.WriteLine($"\nWeather Report for {DateTime.Now.Date}:");
            Console.WriteLine("\nUser's Measurements:");
            Console.WriteLine($"Air Temperature: {todayEntry.UserMeasurements.air_temperature}°C");
            Console.WriteLine($"Wind Speed: {todayEntry.UserMeasurements.wind_speed} m/s");
            Console.WriteLine($"Relative Humidity: {todayEntry.UserMeasurements.relative_humidity}%");
            Console.WriteLine("\nYR's Measurements:");
            Console.WriteLine($"Air Temperature: {todayEntry.YrMeasurements.air_temperature}°C");
            Console.WriteLine($"Wind Speed: {todayEntry.YrMeasurements.wind_speed} m/s");
            Console.WriteLine($"Relative Humidity: {todayEntry.YrMeasurements.relative_humidity}%");
            Console.WriteLine($"\nTemperature Difference: {Math.Round((double)(todayEntry.UserMeasurements.air_temperature - todayEntry.YrMeasurements.air_temperature), 1)}°C");
            Console.WriteLine($"Wind Speed Difference: {Math.Round((double)(todayEntry.UserMeasurements.wind_speed - todayEntry.YrMeasurements.wind_speed), 1)} m/s");
            Console.WriteLine($"Humidity Difference: {Math.Round((double)(todayEntry.UserMeasurements.relative_humidity - todayEntry.YrMeasurements.relative_humidity), 1)}%");
        }
        else
        {
            Console.WriteLine("No weather data for today.");
        }
    }

    public static void GenerateSampleData(int days)
    {
        Random random = new Random();
        List<WeatherLogEntry> entries = new List<WeatherLogEntry>();

        for (int i = 0; i < days; i++)
        {
            WeatherLogEntry entry = new WeatherLogEntry
            {
                Date = DateTime.Now.AddDays(-i),
                UserMeasurements = new Details
                {
                    air_temperature = random.Next(-30, 40),
                    wind_speed = random.Next(0, 20),
                    relative_humidity = random.Next(0, 100)
                },
                YrMeasurements = new Details
                {
                    air_temperature = random.Next(-30, 40),
                    wind_speed = random.Next(0, 20),
                    relative_humidity = random.Next(0, 100)
                }
            };

            entries.Add(entry);
        }

        // Write the entries to the JSON file
        string entriesJson = JsonSerializer.Serialize(entries);
        File.WriteAllText("weatherLogEntries.json", entriesJson);
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
    public class WeatherLogEntry
    {
        public DateTime Date { get; set; }
        public Details UserMeasurements { get; set; }
        public Details YrMeasurements { get; set; }
    }
}