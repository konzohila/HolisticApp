namespace HolisticApp.Services.Interfaces;

public interface INavigationService
{
    Task NavigateToAsync(string route);
    Task GoBackAsync();
}