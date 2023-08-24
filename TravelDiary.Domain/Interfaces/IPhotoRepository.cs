using TravelDiary.Domain.Entities;

namespace TravelDiary.Domain.Interfaces
{
    public interface IPhotoRepository
    {
        Task Create(Photo photo);
    }
}