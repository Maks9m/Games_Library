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
        public IEnumerable<(string Username, int AchievementCount)> GetPlayersWithMostAchievements()
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
                    Username: p.Element("Username")?.Value ?? "Unknown",
                    AchievementCount: countByPlayer.GetValueOrDefault(p.Attribute("Id")?.Value ?? "", 0)
                ))
                .OrderByDescending(x => x.AchievementCount);
            return rankings;
        }

        // 2. Розробники з найбільшою кількістю ігор з високим рейтингом (≥ 8.0)
        public IEnumerable<(string DeveloperName, int Count)> GetDevelopersWithMostHighRatedGames()
        {
            var highRatedByDev = _xmlDoc.Descendants("Game")
                .Where(g => double.TryParse(g.Element("Rating")?.Value, out var r) && r >= 8.0)
                .GroupBy(g => g.Element("DeveloperId")?.Value ?? "")
                .Select(grp => (DeveloperId: grp.Key, Count: grp.Count()))
                .OrderByDescending(x => x.Count);

            var devNames = _xmlDoc.Descendants("Developer")
                .ToDictionary(d => d.Attribute("Id")?.Value ?? "", d => d.Element("Name")?.Value ?? "Unknown");

			return highRatedByDev.Select(x => (DeveloperName: devNames.GetValueOrDefault(x.DeveloperId, "Unknown"), Count: x.Count));
        }

        // 3. Топ-5 ігор за рейтингом з іменем розробника
        public IEnumerable<(string Title, string Rating, string Developer)> GetTop5GamesByRating()
        {
            var devNames = _xmlDoc.Descendants("Developer")
                .ToDictionary(d => d.Attribute("Id")?.Value ?? "", d => d.Element("Name")?.Value ?? "Unknown");

            var top5 = _xmlDoc.Descendants("Game")
                .OrderByDescending(g => double.TryParse(g.Element("Rating")?.Value, out var r) ? r : 0)
                .Take(5)
                .Select(g => (
                    Title: g.Element("Title")?.Value ?? "Unknown",
                    Rating: g.Element("Rating")?.Value ?? "0",
                    Developer: devNames.GetValueOrDefault(g.Attribute("DeveloperId")?.Value ?? "", "Unknown")
                ));

            return top5;
        }

        // 4. Список жанрів з кількістю ігор та середнім рейтингом
        public IEnumerable<(string Genre, int Count, double AvgRating)> GetGenreStatistics()
        {
            var genres = _xmlDoc.Descendants("Game")
                .GroupBy(g => g.Element("Genre")?.Value ?? "Unknown")
                .Select(grp => (
                    Genre: grp.Key,
                    Count: grp.Count(),
                    AvgRating: grp.Average(g => double.TryParse(g.Element("Rating")?.Value, out var r) ? r : 0)
                ))
                .OrderByDescending(x => x.Count);

			return genres;
        }

        // 5. Гравці, зареєстровані після дати, що мають доступ до ігор жанру "RPG"
        public IEnumerable<(string Username, string RegistrationDate, int RpgCount)> GetRecentPlayersWithRpgGames(DateTime afterDate)
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
                    Username: p.Element("Username")?.Value ?? "Unknown",
                    RegistrationDate: p.Element("RegistrationDate")?.Value ?? "",
                    RpgCount: p.Element("GameIds")?.Elements("GameId")
                        .Count(gid => rpgGameIds.Contains(gid.Value)) ?? 0
                ));

            return players;
        }

        // 6. Досягнення за останній рік, згруповані за грою
        public IEnumerable<(string GameTitle, int Count)> GetRecentAchievementsByGame()
        {
            DateTime oneYearAgo = DateTime.Now.AddYears(-1);

            var gameNames = _xmlDoc.Descendants("Game")
                .ToDictionary(g => g.Attribute("Id")?.Value ?? "", g => g.Element("Title")?.Value);

            var grouped = _xmlDoc.Descendants("Achievement")
                .Where(a => DateTime.TryParse(a.Element("UnlockedDate")?.Value, out var date) && date > oneYearAgo)
                .GroupBy(a => a.Attribute("GameId")?.Value ?? "")
                .Select(grp => (
                    GameTitle: gameNames.GetValueOrDefault(grp.Key, "Unknown") ?? "Unknown",
                    Count: grp.Count()
                ))
                .OrderByDescending(x => x.Count);

            return grouped;
        }
    }
}