using HolisticApp.Enums;

namespace HolisticApp.Models;

public readonly struct LoginResult(User? user, LoginStatus status)
{
    public User? User { get; } = user;
    public LoginStatus Status { get; } = status;
}