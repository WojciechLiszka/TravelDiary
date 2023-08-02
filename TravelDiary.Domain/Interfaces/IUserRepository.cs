using TravelDiary.Domain.Entities;

namespace TravelDiary.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> TakeFirst(); //To Delete
    }
}