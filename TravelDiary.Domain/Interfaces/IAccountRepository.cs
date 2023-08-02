using TravelDiary.Domain.Entities;

namespace TravelDiary.Domain.Interfaces
{
    public interface IAccountRepository
    {
        Task<User?> TakeFirst(); //To Delete

        Task Register(User user);

        bool EmailInUse(string email);

        Task<User> GetByEmail(string email);
    }
}