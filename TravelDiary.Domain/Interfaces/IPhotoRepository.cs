using TravelDiary.Domain.Entities;

namespace TravelDiary.Domain.Interfaces
{
    public interface IPhotoRepository
    {
        Task Create(Photo photo);

        Task<Photo?> GetById(Guid id);

        Task Commit();
    }
}