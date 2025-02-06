using SQLite;

namespace HolisticApp.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string CurrentComplaint { get; set; }
        public int? Age { get; set; }
        public string Gender { get; set; }
        public decimal? Height { get; set; } // in cm
        public decimal? Weight { get; set; } // in kg
    }
}