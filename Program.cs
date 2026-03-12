using System;
using System.Collections.Generic;
using System.Linq;
using GameLibrary.Models;
using GameLibrary.DataAccess;
using GameLibrary.Facade;

namespace GameLibrary
{
    class Program
    {
        static void Main(string[] args)
        {
            var launcherCore = new LauncherCore("./Data/gameLibrary.xml");
            launcherCore.Start("./Data/settings.json");
            while (true)
            {
                Console.WriteLine("\n=== Game Launcher Core ===");
                Console.WriteLine("   1. Show Game Library");
                Console.WriteLine("   2. Add Game");
                Console.WriteLine("   3. Add Player");
                Console.WriteLine("   4. Sort Games (Rating / Title / Release Date)");
                Console.WriteLine("   5. Run LINQ Queries (1–6)");
                Console.WriteLine("   6. Settings");
                Console.WriteLine("   7. Top Players (Achievements)");
                Console.WriteLine("   8. Top Developers (High Rated Games)");
                Console.WriteLine("   0. Exit");

                Console.Write("Select an option: ");
                var input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        launcherCore.ShowLibrary();
                        break;
                    case "2":
                        launcherCore.AddGameInteractive();
                        break;
                    case "3":
                        launcherCore.AddPlayerInteractive();
                        break;
                    case "4":
                        Console.WriteLine("Sort by: 1. Rating, 2. Title, 3. Release Date");
                        if (int.TryParse(Console.ReadLine(), out int sortOption) && sortOption >= 1 && sortOption <= 3)
                        {
                            string[] criteria = { "rating", "title", "release" };
                            launcherCore.GetGamesSorted(criteria[sortOption - 1]);
                        }
                        else
                        {
                            Console.WriteLine("Invalid choice.");
                        }
                        break;
                    case "5":
                        Console.WriteLine("Enter query number (1-6):");
                        Console.WriteLine("1. Players with most achievements + avg per game");
                        Console.WriteLine("2. Developers with most high-rated games (>= 8.0)");
                        Console.WriteLine("3. Top 5 games by rating with developer name");
                        Console.WriteLine("4. Genre statistics (count + avg rating)");
                        Console.WriteLine("5. Players registered > 2020 with RPG games");
                        Console.WriteLine("6. Recent achievements grouped by game");

                        if (int.TryParse(Console.ReadLine(), out int queryId) && queryId >= 1 && queryId <= 6)
                        {
                            launcherCore.RunQuery(queryId);
                        }
                        else 
                        {
                            Console.WriteLine("Invalid query number.");
                        }
                        break;
                    case "6":
                        launcherCore.UpdateSettingsInteractive();
                        break;
                    case "7":
                        launcherCore.GetTopPlayers();
                        break;
                    case "8":
                        launcherCore.GetTopDevelopers();
                        break;
                    case "0":
                        Console.WriteLine("Exiting...");
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }
    }
}
