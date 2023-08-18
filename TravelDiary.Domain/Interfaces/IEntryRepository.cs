using TravelDiary.Domain.Entities;

namespace TravelDiary.Domain.Interfaces
{
    public interface IEntryRepository
    {
        Task Create(Entry entry);
        Task<Entry?> GetById(int id);
    }
}