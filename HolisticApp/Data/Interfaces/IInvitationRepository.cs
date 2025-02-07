using HolisticApp.Models;
using System.Threading.Tasks;

namespace HolisticApp.Data.Interfaces
{
    public interface IInvitationRepository
    {
        Task<Invitation> CreateInvitationAsync(Invitation invitation);
        Task<Invitation> GetInvitationByTokenAsync(string token);
        Task MarkInvitationAsUsedAsync(int invitationId);
    }
}