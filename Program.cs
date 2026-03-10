using System;
using GameLibrary.Models;
using GameLibrary.DataAccess;

namespace GameLibrary
{
  class Program
  {
    static void Main(string[] args)
    {
      Console.WriteLine("=== Game Launcher Core ===");
      // Example usage of data access
      var xmlDataService = new XmlDataService("./Data/gameLibrary.xml");
      var gameLibrary = xmlDataService.LoadFromXml();
      Console.WriteLine($"Loaded {gameLibrary.Games.Count} games,\n {gameLibrary.Developers.Count} developers,\n {gameLibrary.Players.Count} players,\n and {gameLibrary.Achievements.Count} achievements.");
    }
  }
}
