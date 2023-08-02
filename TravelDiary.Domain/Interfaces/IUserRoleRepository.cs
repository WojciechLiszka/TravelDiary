using System.Data;
using TravelDiary.Domain.Entities;

namespace TravelDiary.Domain.Interfaces
{
    public interface IUserRoleRepository
    {
        Task<UserRole?> GetById(int id);

        Task<UserRole?> GetByName(string name);
    }
}