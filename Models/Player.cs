using System;
using System.Collections.Generic;
namespace GameLibrary.Models
{
    public class Player
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime RegistrationDate { get; set; }
        public List<int> OwnedGameIds { get; set; }
    }
}