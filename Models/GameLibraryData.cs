using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameLibrary.Models
{
    [XmlRoot("GameLibrary")]
    public class GameLibraryData
    {
        [XmlArray("Developers")]
        [XmlArrayItem("Developer")]
        public List<Developer> Developers { get; set; } = new List<Developer>();

        [XmlArray("Games")]
        [XmlArrayItem("Game")]
        public List<Game> Games { get; set; } = new List<Game>();

        [XmlArray("Players")]
        [XmlArrayItem("Player")]
        public List<Player> Players { get; set; } = new List<Player>();

        [XmlArray("Achievements")]
        [XmlArrayItem("Achievement")]
        public List<Achievement> Achievements { get; set; } = new List<Achievement>();
    }
}
