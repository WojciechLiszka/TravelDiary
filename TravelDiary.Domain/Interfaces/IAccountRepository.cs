using TravelDiary.Domain.Entities;

namespace TravelDiary.Domain.Interfaces
{
    public interface IAccountRepository
    {
        Task Commit();

        Task Delete(User user);

        bool EmailInUse(string email);

        Task<User?> GetByEmail(string email);

        Task<User?> GetById(Guid id);

        Task Register(User user);

        Task<User?> TakeFirst(); //To Delete
    }
}