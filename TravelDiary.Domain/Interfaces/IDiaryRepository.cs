using TravelDiary.Domain.Entities;

namespace TravelDiary.Domain.Interfaces
{
    public interface IDiaryRepository
    {
        Task Create(Diary diary);

        Task<Diary?> GetById(int id);

        Task Delete(Diary diary);

        IQueryable<Diary> Search(string phrase);
    }
}