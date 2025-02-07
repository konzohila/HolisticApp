using System;

namespace HolisticApp.Models
{
    public class Invitation
    {
        public int Id { get; set; }
        public string Token { get; set; }            // Eindeutiger Token (z. B. GUID)
        public int MasterAccountId { get; set; }       // ID des Arztes/ Therapeuten, der die Einladung erstellt hat
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }        // Ablaufdatum der Einladung
        public bool IsUsed { get; set; }               // Wurde die Einladung bereits genutzt?
    }
}