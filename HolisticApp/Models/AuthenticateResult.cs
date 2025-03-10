namespace HolisticApp.Models;

public readonly struct AuthenticateResult
{
    public User User { get; }
    public bool IsAuthenticated { get; }
    
    public AuthenticateResult(User user)
    {
        User = user;
        IsAuthenticated = true;
    }
}