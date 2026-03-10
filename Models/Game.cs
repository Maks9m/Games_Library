using System;
using System.Xml.Serialization;

namespace GameLibrary.Models
{
    public class Game
    {
        [XmlAttribute("Id")]
        public int Id { get; set; }
        [XmlAttribute("DeveloperId")]
        public int DeveloperId { get; set; }
        public string Title { get; set; } = "";
        public string Genre { get; set; } = "";
        public double Rating { get; set; }
        public DateTime ReleaseDate { get; set; }
    }
}