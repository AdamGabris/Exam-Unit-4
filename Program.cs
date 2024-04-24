using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;


class Program
{
    static readonly HttpClient client = new HttpClient();

    static async Task Main(string[] args)
    {
        string latitude = "58.20";
        string longitude = "8.35";
        string url = $"https://api.met.no/weatherapi/locationforecast/2.0/compact?lat={latitude}&lon={longitude}";

        client.DefaultRequestHeaders.UserAgent.ParseAdd("Exam-Unit-4 (https://github.com/AdamGabris/Exam-Unit-4)");
        try
        {
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            Console.WriteLine(responseBody);
            File.WriteAllText("weather.json", responseBody);

        }
        catch (HttpRequestException e)
        {
            Console.WriteLine("\nException Caught!");
            Console.WriteLine("Message :{0} ", e.Message);
        }
    }

}