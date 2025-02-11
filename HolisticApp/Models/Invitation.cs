namespace HolisticApp.Models;

public class Invitation
{
    public int Id { get; set; }
    public string Token { get; set; } = string.Empty;
    public int MasterAccountId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
}