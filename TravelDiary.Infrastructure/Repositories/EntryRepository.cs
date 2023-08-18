using TravelDiary.Domain.Entities;
using TravelDiary.Domain.Interfaces;
using TravelDiary.Infrastructure.Persistence;

namespace TravelDiary.Infrastructure.Repositories
{
    internal class EntryRepository : IEntryRepository
    {
        private readonly TravelDiaryDbContext _dbContext;

        public EntryRepository(TravelDiaryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Create(Entry entry)
        {
            _dbContext.Entries.Add(entry);
            await _dbContext.SaveChangesAsync();
        }
    }
}