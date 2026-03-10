using System;
using System.DateTime;
namespace GameLibrary.Models
{
    public class Game
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }
        public double Rating { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int DeveloperId { get; set; }
    }
}