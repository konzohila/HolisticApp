using HolisticApp.Models;
using HolisticApp.Services.Interfaces;

namespace HolisticApp.Services
{
    public class UserSession : IUserSession
    {
        public User? CurrentUser { get; set; }

        public bool IsUserLoggedIn => CurrentUser != null;

        public void SetUser(User user)
        {
            CurrentUser = user;
        }

        public void ClearUser()
        {
            CurrentUser = null;
        }
    }
}