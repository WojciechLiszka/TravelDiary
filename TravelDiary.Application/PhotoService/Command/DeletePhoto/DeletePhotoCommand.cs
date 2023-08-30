using MediatR;

namespace TravelDiary.Application.PhotoService.Command.DeletePhoto
{
    public class DeletePhotoCommand : IRequest
    {
        public Guid PhotoId { get; set; }
    }
}