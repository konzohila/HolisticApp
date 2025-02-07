namespace HolisticApp.Models
{
    public enum UserRole
    {
        Patient,
        Doctor,
        Admin
    }

    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string CurrentComplaint { get; set; }
        public int? Age { get; set; }
        public string Gender { get; set; }
        public decimal? Height { get; set; }
        public decimal? Weight { get; set; }
        
        // Neue Felder für die Rollentrennung:
        public UserRole Role { get; set; } = UserRole.Patient; // Standardmäßig Patient
        public int? MasterAccountId { get; set; } // Bei Patienten: Verweis auf den Doktor; bei Doktoren und Admins ggf. null oder andere Logik
    }
}