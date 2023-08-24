using TravelDiary.Domain.Entities;
using TravelDiary.Domain.Interfaces;
using TravelDiary.Infrastructure.Persistence;

namespace TravelDiary.Infrastructure.Repositories
{
    public class PhotoRepository : IPhotoRepository
    {
        private readonly TravelDiaryDbContext _dbContext;

        public PhotoRepository(TravelDiaryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Create(Photo photo)
        {
            _dbContext.Photos.Add(photo);
            await _dbContext.SaveChangesAsync();
        }
    }
}