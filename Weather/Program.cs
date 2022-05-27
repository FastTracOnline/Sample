using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Weather
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"{DateTime.Now} Weather Application Started");

            var digitsOnly = "0123456789";
            var pathName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";
            var fileName = "weather.dat";
            var fullFileName = Path.Combine(pathName, fileName);
            List<DailyWeather> weatherList = new List<DailyWeather>();

            // 1. Check if file exists
            // 2. Read text file into an array of strings
            // 3. Iterate array and parse into List of objects
            // 4. Locate and display target value

            // Data is defined into specific columns.
            // Not every column of every record has data (sparse), so using Substring to parse works fine.
            // Each column must be "cleansed" and tested for type.
            // First column is day #.  If not numeric, reject the entire line.
            // Second column is Max Temperature on that day
            // Third column is Min Temperature on that day

            if (File.Exists(fullFileName))
            {
                var lines = File.ReadAllLines(fullFileName);
                for (int i = 0; i < lines.Length; i++)
                {
                    var lineToCheck = lines[i];
                    if (string.IsNullOrEmpty(lineToCheck) || lineToCheck.Length < 20)
                        continue;

                    var newDayofMonth = lineToCheck.Substring(0, 4).OnlyValidCharacters(digitsOnly);
                    var newMaxTemperature = lineToCheck.Substring(5, 6).OnlyValidCharacters(digitsOnly);
                    var newMinTemperature = lineToCheck.Substring(12, 6).OnlyValidCharacters(digitsOnly);

                    if (string.IsNullOrEmpty(newDayofMonth))
                        continue;       // Non-numeric characters in Day of Month - Reject

                    weatherList.Add(new DailyWeather()
                    {
                        DayofMonth = int.Parse(newDayofMonth),
                        MaxTemperature = int.Parse(newMaxTemperature),
                        MinTemperature = int.Parse(newMinTemperature)
                    });
                }

                var result = weatherList.OrderBy(x => x.TemperatureSpread).ToList().First();
                Console.WriteLine($"{DateTime.Now} Day with smallest Temperature Spread was {result.DayofMonth}");
            }
            Console.WriteLine($"{DateTime.Now} Weather Application Ended");
        }
    }

    internal class DailyWeather
    {
        public int DayofMonth { get; set; }
        public int MaxTemperature { get; set; }
        public int MinTemperature { get; set; }
        public int TemperatureSpread => MaxTemperature - MinTemperature;
    }

    public static class Extensions
    {
        public static string OnlyValidCharacters(this string inputString, string validCharacters)
        {
            if (string.IsNullOrEmpty(inputString))
                return inputString;

            var invalidCharacters = Regex.Replace(inputString, $"[{validCharacters}]", "");

            return Regex.Replace(inputString, $"[{ invalidCharacters}]", "");
        }
    }
}