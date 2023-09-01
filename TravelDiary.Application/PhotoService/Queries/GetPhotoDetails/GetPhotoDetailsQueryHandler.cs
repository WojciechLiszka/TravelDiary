using Mapster;
using MediatR;
using TravelDiary.Domain.Exceptions;
using TravelDiary.Domain.Interfaces;
using TravelDiary.Domain.Models;

namespace TravelDiary.Application.PhotoService.Queries.GetPhotoDetails
{
    public class GetPhotoDetailsQueryHandler : IRequestHandler<GetPhotoDetailsQuery, PhotoDetails>
    {
        private readonly IPhotoRepository _photoRepository;

        public GetPhotoDetailsQueryHandler(IPhotoRepository photoRepository)
        {
            _photoRepository = photoRepository;
        }

        public async Task<PhotoDetails> Handle(GetPhotoDetailsQuery request, CancellationToken cancellationToken)
        {
            var photo = await _photoRepository.GetById(request.Id);
            if (photo == null)
            {
                throw new ItemNotFoundException("Photo not found");
            }

            var details = photo.Adapt<PhotoDetails>();

            return details;
        }
    }
}