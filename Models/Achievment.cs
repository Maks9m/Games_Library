using System;
using System.DateTime;
namespace GameLibrary.Models
{
    public class Achievements
  {
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime UnlockedDate { get; set; }
    public int PlayerId { get; set; }
    public int GameId { get; set; }
  }
}