using System;
using System.IO;
using System.Text.Json;

namespace WeatherApp;


public class Functions
{
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

        entries.Add(logEntry);

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
        string entriesJson = File.ReadAllText("weatherLogEntries.json");
        List<WeatherLogEntry> entries = JsonSerializer.Deserialize<List<WeatherLogEntry>>(entriesJson);

        DateTime startDate = DateTime.Now.AddDays(-days);
        List<WeatherLogEntry> filteredEntries = entries.Where(e => e.Date >= startDate).ToList();

        Console.WriteLine($"\n{days}-Day Weather Report:");

        foreach (var entry in filteredEntries)
        {
            double temperatureDifference = Math.Round((double)(entry.UserMeasurements.air_temperature - entry.YrMeasurements.air_temperature), 1);
            double windSpeedDifference = Math.Round((double)(entry.UserMeasurements.wind_speed - entry.YrMeasurements.wind_speed), 1);
            double humidityDifference = Math.Round((double)(entry.UserMeasurements.relative_humidity - entry.YrMeasurements.relative_humidity), 1);

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
        string entriesJson = File.ReadAllText("weatherLogEntries.json");
        List<WeatherLogEntry> entries = JsonSerializer.Deserialize<List<WeatherLogEntry>>(entriesJson);

        DateTime today = DateTime.Now.Date;
        WeatherLogEntry todayEntry = entries.Find(e => e.Date.Date == today);

        if (todayEntry != null)
        {
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

        string entriesJson = JsonSerializer.Serialize(entries);
        File.WriteAllText("weatherLogEntries.json", entriesJson);
    }
}
