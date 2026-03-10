using System;
using System.DateTime;
namespace GameLibrary.Models
{
    public class Developer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public DateTime FoundedDate { get; set; }
    }
}
