using Microsoft.EntityFrameworkCore;
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

        public async Task Commit()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task Create(Diary diary)
        {
            _dbContext.Add(diary);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Delete(Diary diary)
        {
            _dbContext?.Remove(diary);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Diary?> GetById(int id)
        {
            return await _dbContext.Diaries.FirstOrDefaultAsync(d => d.Id == id);
        }

        public IQueryable<Diary> Search(string phrase)
        {
            var diaries = _dbContext
              .Diaries
              .Where(b => phrase == null
              || b.Name.ToLower().Contains(phrase.ToLower())
              || b.Description.ToLower().Contains(phrase.ToLower()))
              .Where(d => d.Policy == PrivacyPolicy.Public);
            return diaries;
        }
    }
}