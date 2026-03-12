using GameLibrary.Configuration;
using GameLibrary.DataAccess;
using GameLibrary.Models;
using GameLibrary.Services;
using GameLibrary.Services.Sorting;

namespace GameLibrary.Facade
{
    public class LauncherCore
    {
        private XmlDataService _dataService;
        private LinqQueries _linqQueries;
        private StatisticsService _statisticsService;
        private GameLibraryData _gameLibrary;

        public LauncherCore(string xmlPath)
        {
            _dataService = new XmlDataService(xmlPath);
            _linqQueries = new LinqQueries(xmlPath);
            _statisticsService = new StatisticsService(new SortByTitle());
            _gameLibrary = _dataService.LoadFromXml();
        }

        public void Start(string configPath)
        {
            ConfigurationManager.SetConfigPath(configPath);
            ConfigurationManager.Instance.Save();

            _gameLibrary = _dataService.LoadFromXml();

            var config = ConfigurationManager.Instance.Config;
            Console.WriteLine("Current Configuration:");
            Console.WriteLine($"Download Path: {config.DownloadPath}");
            Console.WriteLine($"Max Download Speed: {config.MaxDownloadSpeed} MB/s");
            Console.WriteLine($"Auto Updates Enabled: {config.EnableAutoUpdates}");
            Console.WriteLine($"Current Theme: {config.CurrentTheme}");

            Console.WriteLine($"\nSystem loaded: {_gameLibrary.Games.Count} games, {_gameLibrary.Players.Count} players.");
        }

        public void GetTopPlayers()
        {
            var topPlayers = _linqQueries.GetPlayersWithMostAchievements();
            Console.WriteLine("Top Players:");
            foreach (var player in topPlayers)
                Console.WriteLine($"  {player.Username} - {player.AchievementCount} achievements");
        }

        public void GetTopDevelopers()
        {
            var topDevelopers = _linqQueries.GetDevelopersWithMostHighRatedGames();
            Console.WriteLine("Top Developers:");
            foreach (var developer in topDevelopers)
                Console.WriteLine($"  {developer.DeveloperName} - {developer.Count} high-rated games");
        }

        public void GetGamesSorted(string criteria)
        {
            // Default to title
            ISortStrategy strategy = new SortByTitle();

            switch (criteria.ToLower())
            {
                case "title":
                    strategy = new SortByTitle();
                    break;
                case "rating":
                    strategy = new SortByRating();
                    break;
                case "release":
                    strategy = new SortByReleaseDate();
                    break;
                default:
                    Console.WriteLine("Unknown sorting criteria. using 'title'.");
                    break;
            }

            _statisticsService.SortStrategy = strategy;
            var sortedGames = _statisticsService.GetSortedGames(_gameLibrary.Games);

            if (sortedGames.Any())
            {
                Console.WriteLine($"Games sorted by {criteria}:");
                foreach (var game in sortedGames)
                    Console.WriteLine($"  {game.Title} ({game.ReleaseDate:yyyy-MM-dd}) - Rating: {game.Rating:F1}");
            }
            else
            {
                Console.WriteLine("No games found in the library.");
            }
        }

        // Expose data adding capability for the UI
        public void AddGame(Game game)
        {
            // Assign valid ID
            game.Id = _gameLibrary.Games.Any() ? _gameLibrary.Games.Max(g => g.Id) + 1 : 0;
            _gameLibrary.Games.Add(game);
            _dataService.SaveToXml(_gameLibrary);
            Console.WriteLine($"Game '{game.Title}' added and saved.");
        }

        public void AddGameInteractive()
        {
            Console.WriteLine("Enter game details:");

            Console.Write("DeveloperId: ");
            int developer;
            while (!int.TryParse(Console.ReadLine(), out developer))
                Console.WriteLine("Invalid input. Please enter a valid integer for DeveloperId.");
            Console.Write("Title: ");
            string title;
            do
            {
                title = Console.ReadLine() ?? "";
                if (string.IsNullOrWhiteSpace(title))
                    Console.WriteLine("Title cannot be empty. Try again.");
            } while (string.IsNullOrWhiteSpace(title));

            Console.Write("Genre: ");
            string genre;
            do
            {
                genre = Console.ReadLine() ?? "";
                if (string.IsNullOrWhiteSpace(genre))
                    Console.WriteLine("Genre cannot be empty. Try again.");
            } while (string.IsNullOrWhiteSpace(genre));


            var game = new Game
            {
                DeveloperId = developer,
                Title = title,
                Genre = genre,
                Rating = 0,
                ReleaseDate = DateTime.Now
            };
            AddGame(game);

        }

        public void AddPlayer(Player player)
        {
            player.Id = _gameLibrary.Players.Any() ? _gameLibrary.Players.Max(p => p.Id) + 1 : 0;
            _gameLibrary.Players.Add(player);
            _dataService.SaveToXml(_gameLibrary);
            Console.WriteLine($"Player '{player.Username}' added and saved.");
        }

        public void AddPlayerInteractive()
        {
            Console.WriteLine("Enter player details:");

            Console.Write("Username: ");
            string username;
            do
            {
                username = Console.ReadLine() ?? "";
                if (string.IsNullOrWhiteSpace(username))
                    Console.WriteLine("Username cannot be empty. Try again.");
            } while (string.IsNullOrWhiteSpace(username));

            Console.Write("Email: ");
            string email;
            do
            {
                email = Console.ReadLine() ?? "";
                if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
                    Console.WriteLine("Invalid email. Try again.");
            } while (string.IsNullOrWhiteSpace(email) || !email.Contains("@"));

            var player = new Player
            {
                Username = username,
                Email = email,
                RegistrationDate = DateTime.Now,
                GameIds = new List<int>(),
            };
            AddPlayer(player);
        }

        public void UpdateSettingsInteractive()
        {
            var config = ConfigurationManager.Instance.Config;

            while (true)
            {
                Console.WriteLine("\nCurrent Settings:");
                Console.WriteLine($"1. Download Path: {config.DownloadPath}");
                Console.WriteLine($"2. Max Download Speed: {config.MaxDownloadSpeed} MB/s");
                Console.WriteLine($"3. Auto Updates Enabled: {config.EnableAutoUpdates}");
                Console.WriteLine($"4. Current Theme: {config.CurrentTheme}");
                Console.WriteLine("Enter the number of the setting you want to change (or 0 to finish):");

                Console.Write("Select an option: ");
                if (!int.TryParse(Console.ReadLine(), out int settingNumber) || settingNumber < 0 || settingNumber > 4)
                {
                    Console.WriteLine("Invalid input. Please enter a number between 0 and 4.");
                    continue;
                }

                if (settingNumber == 0)
                    break;

                switch (settingNumber)
                {
                    case 1:
                        Console.Write("Enter new Download Path: ");
                        string newPath = Console.ReadLine() ?? "";
                        if (!string.IsNullOrWhiteSpace(newPath))
                            config.DownloadPath = newPath;
                        break;
                    case 2:
                        Console.Write("Enter new Max Download Speed (MB/s): ");
                        if (double.TryParse(Console.ReadLine(), out double speed) && speed >= 0)
                            config.MaxDownloadSpeed = speed;
                        else
                            Console.WriteLine("Invalid input. Must be a non-negative number.");
                        break;
                    case 3:
                        Console.Write("Enable Auto Updates (true/false): ");
                        if (bool.TryParse(Console.ReadLine(), out bool autoUpdates))
                            config.EnableAutoUpdates = autoUpdates;
                        else
                            Console.WriteLine("Invalid input. Please enter 'true' or 'false'.");
                        break;
                    case 4:
                        Console.Write("Enter new Theme (Light, Dark, System): ");
                        string themeInput = Console.ReadLine() ?? "";
                        if (Enum.TryParse(themeInput, true, out AppConfig.Theme newTheme))
                            config.CurrentTheme = newTheme;
                        else
                            Console.WriteLine("Invalid theme. Options: Light, Dark, System.");
                        break;
                }
            }

            ConfigurationManager.Instance.Save();
            Console.WriteLine("Settings updated and saved.");
        }

        public void ShowLibrary()
        {
            Console.WriteLine("\n=== Game Library ===");
            foreach (var g in _gameLibrary.Games)
                Console.WriteLine($"{g.Id}. {g.Title} [{g.Genre}] - {g.Rating:F1}");
        }

        public void RunQuery(int queryId)
        {
            Console.WriteLine($"\n--- Running Query #{queryId} ---");
            switch (queryId)
            {
                case 1: GetTopPlayers(); break;
                case 2: GetTopDevelopers(); break;
                case 3:
                    var top5 = _linqQueries.GetTop5GamesByRating();
                    Console.WriteLine("Top 5 Games:");
                    foreach (var t in top5) Console.WriteLine($"  {t.Title} by {t.Developer} ({t.Rating})");
                    break;
                case 4:
                    var genres = _linqQueries.GetGenreStatistics();
                    Console.WriteLine("Genre Stats:");
                    foreach (var g in genres) Console.WriteLine($"  {g.Genre}: {g.Count} games, avg {g.AvgRating:F1}");
                    break;
                case 5:
                    var recent = _linqQueries.GetRecentPlayersWithRpgGames(new DateTime(2020, 1, 1));
                    Console.WriteLine("Players registered > 2020 with RPGs:");
                    foreach (var p in recent) Console.WriteLine($"  {p.Username} ({p.RegistrationDate})");
                    break;
                case 6:
                    var ach = _linqQueries.GetRecentAchievementsByGame();
                    Console.WriteLine("Recent Achievements:");
                    foreach (var a in ach) Console.WriteLine($"  {a.GameTitle}: {a.Count}");
                    break;
                default: Console.WriteLine("Invalid query number."); break;
            }
        }
    }
}