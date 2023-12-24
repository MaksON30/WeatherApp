using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace WeatherApp
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly string url = "https://api.openweathermap.org/data/2.5/forecast?appid=78d0dc21439022fab6b342da6b900429&q=Cherkasy&cnt=5&units=metric";

        async static Task Main(string[] args)
        {
            string databasePath = "WeatherDatabase.db";

            if (!File.Exists(databasePath))
            {
                DatabaseInitializer.InitializeDatabase(databasePath);
            }

            await WeatherProcessor.GetWeather(databasePath, client, url);
            await WeatherProcessor.GetAndSaveTodayForecast(databasePath, client);
            WeatherProcessor.DisplayAllRecords(databasePath);

            Console.WriteLine("Hello, World!");
        }
    }



    public class Coord
    {
        public double? lat { get; set; }
        public double? lon { get; set; }
    }

    public class City
    {
        public int? id { get; set; }
        public string? name { get; set; }
        public Coord? coord { get; set; }
        public string? country { get; set; }
        public int? population { get; set; }
        public int? timezone { get; set; }
        public int? sunrise { get; set; }
        public int? sunset { get; set; }
    }

    public class WeatherInfoMain
    {
        public double? temp { get; set; }
        public double? feels_like { get; set; }
        public double? temp_min { get; set; }
        public double? temp_max { get; set; }
        public int? pressure { get; set; }
        public int? sea_level { get; set; }
        public int? grnd_level { get; set; }
        public int? humidity { get; set; }
        public double? temp_kf { get; set; }
    }

    public class WeatherInfo
    {
        public int? dt { get; set; }
        public WeatherInfoMain? main { get; set; }
        public string? dt_txt { get; set; }
        public City? city { get; set; }
    }

    public class WeatherForecast
    {
        public string? cod { get; set; }
        public int? message { get; set; }
        public int? cnt { get; set; }
        public City? city { get; set; }
        public List<WeatherInfo>? list { get; set; }
    }
}
