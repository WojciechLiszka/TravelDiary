using MediatR;
using TravelDiary.Domain.Models;

namespace TravelDiary.Application.PhotoService.Command.UpdatePhotoDetails
{
    public class UpdatePhotoDetailsCommand : PhotoDetails, IRequest
    {
        public Guid PhotoId { get; set; }
    }
}