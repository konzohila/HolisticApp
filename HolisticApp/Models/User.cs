using SQLite;

namespace HolisticApp.Models
{
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        
        public string? Username { get; set; }
        
        public string? Email { get; set; }
        
        // In diesem Beispiel speichern wir das Passwort unverschlüsselt.
        // In der Praxis sollte hier ein gehashter Wert stehen.
        public string? PasswordHash { get; set; }
    }
}