using System.Security.Claims;

namespace TravelDiary.Domain.Interfaces
{
    public interface IUserContextService
    {
        string? GetUserEmail { get; }
        Guid? GetUserId { get; }
        string? GetUserRole { get; }
        ClaimsPrincipal User { get; }
    }
}