using MediatR;
using TravelDiary.Domain.Models;

namespace TravelDiary.Application.PhotoService.Queries.GetPhotoDetails
{
    public class GetPhotoDetailsQuery : IRequest<PhotoDetails>
    {
        public Guid Id { get; set; }
    }
}