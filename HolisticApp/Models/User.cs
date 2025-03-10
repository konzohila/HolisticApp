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
        public int Id { get; init; }
        public string? Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; init; }
        public string CurrentComplaint { get; set; }
        public string Gender { get; set; }
        public int? Age { get; set; }
        public decimal? Height { get; set; }
        public decimal? Weight { get; set; }
        public UserRole Role { get; set; }
        public int? MasterAccountId { get; set; }
        
        public string? LinkedDoctorName { get; set; }

        public override string ToString() =>
            $"{Username} ({Email}) - {Gender}, {Age?.ToString() ?? "N/A"} Jahre, {Role}";
    }
}