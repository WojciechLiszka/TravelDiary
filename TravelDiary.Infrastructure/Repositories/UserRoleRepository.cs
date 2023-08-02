using Microsoft.EntityFrameworkCore;
using TravelDiary.Domain.Entities;
using TravelDiary.Domain.Interfaces;
using TravelDiary.Infrastructure.Persistence;

namespace TravelDiary.Infrastructure.Repositories
{
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly TravelDiaryDbContext _dbContext;

        public UserRoleRepository(TravelDiaryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UserRole?> GetById(int id)
        {
            var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Id == id);
            return role;
        }

        public async Task<UserRole?> GetByName(string name)
        {
            var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.RoleName == name);
            return role;
        }
    }
}