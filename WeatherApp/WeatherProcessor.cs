using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WeatherApp
{
    public static class WeatherProcessor
    {
        public async static Task GetWeather(string databasePath, HttpClient client, string url)
        {
            Console.WriteLine("Getting JSON...");
            var responseString = await client.GetStringAsync(url);
            Console.WriteLine("Parsing JSON...");
            WeatherForecast weatherForecast = JsonSerializer.Deserialize<WeatherForecast>(responseString);
            Console.WriteLine($"cod: {weatherForecast?.cod}");
            Console.WriteLine($"City: {weatherForecast?.city?.name}");
            Console.WriteLine($"list count: {weatherForecast?.list?.Count}");

            using (var connection = new SQLiteConnection($"Data Source={databasePath};"))
            {
                connection.Open();

                // Очистка попередніх записів в базі даних перед вставкою нових
                using (var clearCommand = new SQLiteCommand("DELETE FROM WeatherForecast;", connection))
                {
                    clearCommand.ExecuteNonQuery();
                }

                if (weatherForecast?.city?.name != null && weatherForecast.list != null)
                {
                    foreach (var weather in weatherForecast.list)
                    {
                        if (weather?.main?.temp != null)
                        {
                            using (var insertCommand = new SQLiteCommand("INSERT INTO WeatherForecast (CityName, Temperature) VALUES (@CityName, @Temperature);", connection))
                            {
                                insertCommand.Parameters.AddWithValue("@CityName", weatherForecast.city.name);
                                insertCommand.Parameters.AddWithValue("@Temperature", weather.main.temp);

                                insertCommand.ExecuteNonQuery();
                            }
                        }
                    }
                }

                connection.Close();
            }

            foreach (var weather in weatherForecast?.list)
            {
                Console.WriteLine($"weather temp : {weather?.main?.temp}");
            }
        }

        public async static Task GetAndSaveTodayForecast(string databasePath, HttpClient client)
        {
            string todayUrl = "https://api.openweathermap.org/data/2.5/weather?appid=78d0dc21439022fab6b342da6b900429&q=Cherkasy";

            Console.WriteLine("Getting today's weather...");
            var todayResponseString = await client.GetStringAsync(todayUrl);
            Console.WriteLine("Parsing today's weather...");
            WeatherInfo todayWeather = JsonSerializer.Deserialize<WeatherInfo>(todayResponseString);

            if (todayWeather != null)
            {
                Console.WriteLine($"Today's temperature: {todayWeather.main?.temp}°C");

                if (todayWeather.city?.coord != null)
                {
                    Console.WriteLine($"Coordinates: {todayWeather.city.coord.lat}, {todayWeather.city.coord.lon}");
                }
                else
                {
                    Console.WriteLine("Unable to retrieve coordinates.");
                }

                using (var connection = new SQLiteConnection($"Data Source={databasePath};"))
                {
                    connection.Open();

                    using (var insertCommand = new SQLiteCommand("INSERT INTO WeatherForecast (CityName, Temperature) VALUES (@CityName, @Temperature);", connection))
                    {
                        insertCommand.Parameters.AddWithValue("@CityName", todayWeather.city?.name);
                        insertCommand.Parameters.AddWithValue("@Temperature", todayWeather.main.temp);

                        insertCommand.ExecuteNonQuery();
                    }

                    connection.Close();
                }
            }
            else
            {
                Console.WriteLine("Unable to retrieve today's weather information.");
            }
        }

        public static void DisplayAllRecords(string databasePath)
        {
            using (var connection = new SQLiteConnection($"Data Source={databasePath};"))
            {
                connection.Open();

                using (var selectCommand = new SQLiteCommand("SELECT * FROM WeatherForecast;", connection))
                {
                    var reader = selectCommand.ExecuteReader();

                    Console.WriteLine("All Weather Records:");
                    while (reader.Read())
                    {
                        Console.WriteLine($"City: {reader["CityName"]}, Temperature: {reader["Temperature"]}°C");
                    }
                }

                connection.Close();
            }
        }
    }
}
