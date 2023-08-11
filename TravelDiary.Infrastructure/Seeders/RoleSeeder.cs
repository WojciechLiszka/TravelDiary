using Microsoft.EntityFrameworkCore;
using TravelDiary.Domain.Entities;
using TravelDiary.Infrastructure.Persistence;

namespace TravelDiary.Infrastructure.Seeders
{
    public class RoleSeeder
    {
        private readonly TravelDiaryDbContext _dbContext;
        private readonly IEnumerable<UserRole> _roles;

        public RoleSeeder(TravelDiaryDbContext dbContext)
        {
            _dbContext = dbContext;
            _roles = GetRoles().ToList();
        }

        public async Task Seed()
        {
            if (_dbContext.Database.CanConnect() && _dbContext.Database.IsRelational())
            {
                var pendingMigrations = _dbContext.Database.GetPendingMigrations();
                if (pendingMigrations != null && pendingMigrations.Any())
                {
                    _dbContext.Database.Migrate();
                }
                if (!_dbContext.Roles.Any())
                {
                    _dbContext.Roles.AddRange(_roles);
                    await _dbContext.SaveChangesAsync();
                }
            }
        }

        private IEnumerable<UserRole> GetRoles()
        {
            var roles = new List<UserRole>()
            {
                new UserRole()
                {
                    RoleName = "User"
                },

                new UserRole()
                {
                    RoleName = "Admin"
                }
            };

            return roles;
        }
    }
}