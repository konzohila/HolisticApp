using SQLite;

namespace HolisticApp.Models
{
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        
        public string? Username { get; set; }
        
        public string? Email { get; set; }
        
        // Hier steht im Beispiel unverschlüsselt das Passwort.
        public string? PasswordHash { get; set; }
        
        // Neue Eigenschaft für die Anamnese (z. B. aktuelle Beschwerden)
        public string? CurrentComplaint { get; set; }
    }
}