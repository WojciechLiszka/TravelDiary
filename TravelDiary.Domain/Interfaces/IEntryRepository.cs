using TravelDiary.Domain.Entities;

namespace TravelDiary.Domain.Interfaces
{
    public interface IEntryRepository
    {
        Task Commit();

        Task Create(Entry entry);

        Task Delete(Entry entry);

        Task<Entry?> GetById(int id);
    }
}