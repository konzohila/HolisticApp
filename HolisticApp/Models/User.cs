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

        public string Username { get; set; } = "";
        public string Email { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public string CurrentComplaint { get; set; } = "";
        public string Gender { get; set; } = "";

        public int? Age { get; set; }
        public decimal? Height { get; set; }
        public decimal? Weight { get; set; }

        public UserRole Role { get; set; } = UserRole.Patient;
        public int? MasterAccountId { get; set; }

        public User(
            string username = "",
            string email = "",
            string passwordHash = "",
            string currentComplaint = "",
            string gender = "",
            UserRole role = UserRole.Patient,
            int? age = null,
            decimal? height = null,
            decimal? weight = null,
            int? masterAccountId = null)
        {
            Username = username;
            Email = email;
            PasswordHash = passwordHash;
            CurrentComplaint = currentComplaint;
            Gender = gender;
            Role = role;
            Age = age;
            Height = height;
            Weight = weight;
            MasterAccountId = masterAccountId;
        }

        public override string ToString() =>
            $"{Username} ({Email}) - {Gender}, {Age?.ToString() ?? "N/A"} Jahre, {Role}";
    }
}