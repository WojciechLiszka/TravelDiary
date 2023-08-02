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

        public bool EmailInUse(string email)
            => _dbContext.Users.Any(u => u.UserDetails.Email == email);

        public async Task Register(User user)
        {
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            return;
        }

        public async Task<User?> TakeFirst() //To Delete
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync();
            return user;
        }
    }
}