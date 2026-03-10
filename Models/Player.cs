using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameLibrary.Models
{
    public class Player
    {
        [XmlAttribute("Id")]
        public int Id { get; set; }
        public string Username { get; set; } = "";
        public string Email { get; set; } = "";
        public DateTime RegistrationDate { get; set; }
        [XmlArray("GameIds")]
        [XmlArrayItem("GameId")]
        public List<int> GameIds { get; set; } = new List<int>();
    }
}