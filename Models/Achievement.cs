using System;
using System.Xml.Serialization;

namespace GameLibrary.Models
{
    public class Achievement
    {
        [XmlAttribute("Id")]
        public int Id { get; set; }
        [XmlAttribute("PlayerId")]
        public int PlayerId { get; set; }
        [XmlAttribute("GameId")]
        public int GameId { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime UnlockedDate { get; set; }
    }
}