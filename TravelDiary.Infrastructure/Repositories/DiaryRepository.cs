using TravelDiary.Domain.Entities;
using TravelDiary.Domain.Interfaces;
using TravelDiary.Infrastructure.Persistence;

namespace TravelDiary.Infrastructure.Repositories
{
    public class DiaryRepository : IDiaryRepository
    {
        private readonly TravelDiaryDbContext _dbContext;

        public DiaryRepository(TravelDiaryDbContext travelDiaryDBContext)
        {
            _dbContext = travelDiaryDBContext;
        }

        public async Task Create(Diary diary)
        {
            _dbContext.Add(diary);
            await _dbContext.SaveChangesAsync();
        }
    }
}