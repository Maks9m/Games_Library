using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameLibrary.Models;

namespace GameLibrary.DataAccess
{
    class LinqQueries(string xmlFilePath)
    {
        XDocument _xmlDoc = XDocument.Load(xmlFilePath);

        // 1. Гравці з найбільшою кількістю досягнень + середня кількість досягнень на гру
        public void GetPlayersWithMostAchievements()
        {
            int gameCount = _xmlDoc.Descendants("Game").Count();
            double avgPerGame = gameCount > 0
                ? (double)_xmlDoc.Descendants("Achievement").Count() / gameCount
                : 0;

            var countByPlayer = _xmlDoc.Descendants("Achievement")
                .GroupBy(a => a.Element("PlayerId")?.Value ?? "")
                .ToDictionary(g => g.Key, g => g.Count());

            var rankings = _xmlDoc.Descendants("Player")
                .Select(p => (
                    Username: p.Element("Username")?.Value,
                    AchievementCount: countByPlayer.GetValueOrDefault(p.Attribute("Id")?.Value ?? "", 0)
                ))
                .OrderByDescending(x => x.AchievementCount);

            Console.WriteLine($"Average achievements per game: {avgPerGame:F2}");
            Console.WriteLine("Players ranked by achievement count:");
            foreach (var (username, count) in rankings)
            {
                Console.WriteLine($"  {username}: {count} achievements");
            }
        }

        // 2. Розробники з найбільшою кількістю ігор з високим рейтингом (≥ 8.0)
        public void GetDevelopersWithMostHighRatedGames()
        {
            var highRatedByDev = _xmlDoc.Descendants("Game")
                .Where(g => double.TryParse(g.Element("Rating")?.Value, out var r) && r >= 8.0)
                .GroupBy(g => g.Element("DeveloperId")?.Value ?? "")
                .Select(grp => (DeveloperId: grp.Key, Count: grp.Count()))
                .OrderByDescending(x => x.Count);

            var devNames = _xmlDoc.Descendants("Developer")
                .ToDictionary(d => d.Attribute("Id")?.Value ?? "", d => d.Element("Name")?.Value ?? "Unknown");

            Console.WriteLine("Developers ranked by high-rated games (Rating >= 8.0):");
            foreach (var (devId, count) in highRatedByDev)
            {
                string name = devNames.GetValueOrDefault(devId, "Unknown");
                Console.WriteLine($"  {name}: {count} high-rated games");
            }
        }

        // 3. Топ-5 ігор за рейтингом з іменем розробника
        public void GetTop5GamesByRating()
        {
            var devNames = _xmlDoc.Descendants("Developer")
                .ToDictionary(d => d.Attribute("Id")?.Value ?? "", d => d.Element("Name")?.Value ?? "Unknown");

            var top5 = _xmlDoc.Descendants("Game")
                .OrderByDescending(g => double.TryParse(g.Element("Rating")?.Value, out var r) ? r : 0)
                .Take(5)
                .Select(g => (
                    Title: g.Element("Title")?.Value,
                    Rating: g.Element("Rating")?.Value,
                    Developer: devNames.GetValueOrDefault(g.Attribute("DeveloperId")?.Value ?? "", "Unknown")
                ));

            Console.WriteLine("Top 5 games by rating:");
            foreach (var (title, rating, developer) in top5)
            {
                Console.WriteLine($"  {title} (Rating: {rating}) — {developer}");
            }
        }

        // 4. Список жанрів з кількістю ігор та середнім рейтингом
        public void GetGenreStatistics()
        {
            var genres = _xmlDoc.Descendants("Game")
                .GroupBy(g => g.Element("Genre")?.Value)
                .Select(grp => (
                    Genre: grp.Key,
                    Count: grp.Count(),
                    AvgRating: grp.Average(g => double.TryParse(g.Element("Rating")?.Value, out var r) ? r : 0)
                ))
                .OrderByDescending(x => x.Count);

            Console.WriteLine("Genre statistics:");
            foreach (var (genre, count, avgRating) in genres)
            {
                Console.WriteLine($"  {genre}: {count} games, avg rating {avgRating:F2}");
            }
        }

        // 5. Гравці, зареєстровані після дати, що мають доступ до ігор жанру "RPG"
        public void GetRecentPlayersWithRpgGames(DateTime afterDate)
        {
            var rpgGameIds = _xmlDoc.Descendants("Game")
                .Where(g => g.Element("Genre")?.Value == "RPG")
                .Select(g => g.Attribute("Id")?.Value)
                .ToHashSet();

            var players = _xmlDoc.Descendants("Player")
                .Where(p => DateTime.TryParse(p.Element("RegistrationDate")?.Value, out var regDate) && regDate > afterDate)
                .Where(p => p.Element("GameIds")?.Elements("GameId")
                    .Any(gid => rpgGameIds.Contains(gid.Value)) == true)
                .Select(p => (
                    Username: p.Element("Username")?.Value,
                    RegistrationDate: p.Element("RegistrationDate")?.Value,
                    RpgCount: p.Element("GameIds")?.Elements("GameId")
                        .Count(gid => rpgGameIds.Contains(gid.Value)) ?? 0
                ));

            Console.WriteLine($"Players registered after {afterDate:yyyy-MM-dd} with RPG games:");
            foreach (var (username, regDate, rpgCount) in players)
            {
                Console.WriteLine($"  {username} (registered {regDate}): {rpgCount} RPG games");
            }
        }

        // 6. Досягнення за останній рік, згруповані за грою
        public void GetRecentAchievementsByGame()
        {
            DateTime oneYearAgo = DateTime.Now.AddYears(-1);

            var gameNames = _xmlDoc.Descendants("Game")
                .ToDictionary(g => g.Attribute("Id")?.Value ?? "", g => g.Element("Title")?.Value);

            var grouped = _xmlDoc.Descendants("Achievement")
                .Where(a => DateTime.TryParse(a.Element("UnlockedDate")?.Value, out var date) && date > oneYearAgo)
                .GroupBy(a => a.Attribute("GameId")?.Value ?? "")
                .Select(grp => (
                    GameTitle: gameNames.GetValueOrDefault(grp.Key, "Unknown"),
                    Count: grp.Count()
                ))
                .OrderByDescending(x => x.Count);

            Console.WriteLine($"Achievements unlocked after {oneYearAgo:yyyy-MM-dd}, by game:");
            foreach (var (gameTitle, count) in grouped)
            {
                Console.WriteLine($"  {gameTitle}: {count} achievements");
            }
        }
    }
}