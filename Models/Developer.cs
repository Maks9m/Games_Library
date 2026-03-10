using System.Xml.Serialization;

namespace GameLibrary.Models
{
    public class Developer
    {
        [XmlAttribute("Id")]
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Country { get; set; } = "";
        public int FoundedYear { get; set; }
    }
}
