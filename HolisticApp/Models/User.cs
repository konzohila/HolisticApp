namespace HolisticApp.Models;

public enum UserRole
{
    Patient,
    Doctor,
    Admin
}

public class User(
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
    public int Id { get; set; }
    public string Username { get; set; } = username;
    public string Email { get; set; } = email;
    public string PasswordHash { get; set; } = passwordHash;
    public string CurrentComplaint { get; set; } = currentComplaint;
    public string Gender { get; set; } = gender;

    public int? Age { get; set; } = age;
    public decimal? Height { get; set; } = height;
    public decimal? Weight { get; set; } = weight;

    public UserRole Role { get; set; } = role;
    public int? MasterAccountId { get; set; } = masterAccountId;
    public override string ToString() =>
        $"{Username} ({Email}) - {Gender}, {Age?.ToString() ?? "N/A"} Jahre, {Role}";
}