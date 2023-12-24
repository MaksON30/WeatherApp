using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApp
{
    public static class DatabaseInitializer
    {
        public static void InitializeDatabase(string databasePath)
        {
            using (var connection = new SQLiteConnection($"Data Source={databasePath};"))
            {
                connection.Open();

                using (var command = new SQLiteCommand("CREATE TABLE IF NOT EXISTS WeatherForecast (CityName TEXT, Temperature REAL);", connection))
                {
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }
    }
}

