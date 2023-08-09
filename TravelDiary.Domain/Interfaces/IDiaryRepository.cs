using TravelDiary.Domain.Entities;

namespace TravelDiary.Domain.Interfaces
{
    public interface IDiaryRepository
    {
        Task Create(Diary diary);

        Task<Diary?> GetDiaryById(int id);
    }
}