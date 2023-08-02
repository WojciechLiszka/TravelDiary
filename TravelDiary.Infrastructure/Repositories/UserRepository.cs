using Microsoft.EntityFrameworkCore;
using TravelDiary.Domain.Entities;
using TravelDiary.Domain.Interfaces;
using TravelDiary.Infrastructure.Persistence;

namespace TravelDiary.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly TravelDiaryDbContext _dbContext;

        public UserRepository(TravelDiaryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User?> TakeFirst() //To Delete
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync();
            return user;
        }
    }
}