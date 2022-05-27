using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Football
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"{DateTime.Now} Football Application Started");

            var digitsOnly = "0123456789";
            var pathName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";
            var fileName = "football.dat";
            var fullFileName = Path.Combine(pathName, fileName);
            List<TeamGoals> teamList = new List<TeamGoals>();

            // 1. Check if file exists
            // 2. Read text file into an array of strings
            // 3. Iterate array and parse into List of objects
            // 4. Locate and display target value

            // Data is defined into specific columns.
            // Every column of every record has data, so using Split to parse works fine.
            // Each column must be "cleansed" and tested for type.
            // First column is list #.  If not numeric, reject the entire line.
            // Second column is Team Name (allow trimmed strings)
            // Seventh column is Goals Scored FOR the team vs opponents
            // Eighth column is Goals Scored AGAINST the team by opponents

            if (File.Exists(fullFileName))
            {
                var lines = File.ReadAllLines(fullFileName);
                for (int i = 0; i < lines.Length; i++)
                {
                    var lineToCheck = lines[i];
                    if (string.IsNullOrEmpty(lineToCheck) || lineToCheck.Length < 20)
                        continue;

                    var fields = lineToCheck.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (fields.Length < 9)
                        continue;

                    var newTeamNumber = fields[0].OnlyValidCharacters(digitsOnly);
                    var newTeamName = fields[1].Trim();
                    var newGoalsFor = fields[6].OnlyValidCharacters(digitsOnly);
                    var newGoalsAgainst = fields[8].OnlyValidCharacters(digitsOnly);

                    if (string.IsNullOrEmpty(newTeamNumber))
                        continue;       // Non-numeric characters in Team # - Reject

                    teamList.Add(new TeamGoals()
                    {
                        TeamName = newTeamName,
                        GoalsFor = int.Parse(newGoalsFor),
                        GoalsAgainst = int.Parse(newGoalsAgainst)
                    });
                }

                // It was not specified that Goals For must be > Goals Against, so ABS() was used
                var result = teamList.OrderBy(x => x.GoalSpread).ToList().First();
                Console.WriteLine($"{DateTime.Now} Team with smallest Goal Spread was {result.TeamName}");
            }
            Console.WriteLine($"{DateTime.Now} Football Application Ended");
        }
    }

    internal class TeamGoals
    {
        public string? TeamName { get; set; }
        public int GoalsFor { get; set; }
        public int GoalsAgainst { get; set; }
        public int GoalSpread => Math.Abs(GoalsFor - GoalsAgainst);
    }

    public static class Extensions
    {
        public static string OnlyValidCharacters(this string inputString, string validCharacters)
        {
            if (string.IsNullOrEmpty(inputString))
                return inputString;

            var invalidCharacters = Regex.Replace(inputString, $"[{validCharacters}]", "");

            if (string.IsNullOrEmpty(invalidCharacters))
                return inputString;
            else
                return Regex.Replace(inputString, $"[{invalidCharacters}]", "");
        }
    }
}