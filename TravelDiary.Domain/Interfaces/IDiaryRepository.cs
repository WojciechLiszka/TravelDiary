using TravelDiary.Domain.Entities;

namespace TravelDiary.Domain.Interfaces
{
    public interface IDiaryRepository
    {
        Task Commit();

        Task Create(Diary diary);

        Task Delete(Diary diary);

        Task<Diary?> GetById(int id);

        IQueryable<Diary> Search(string phrase);
    }
}