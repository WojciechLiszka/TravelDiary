using Azure.Storage.Blobs;
using MediatR;
using TravelDiary.Domain.Dtos;
using TravelDiary.Domain.Exceptions;
using TravelDiary.Domain.Interfaces;

namespace TravelDiary.Application.PhotoService.Queries.GetPhoto
{
    public class GetPhotoQueryHandler : IRequestHandler<GetPhotoQuery, PhotoDto>
    {
        private readonly BlobServiceClient _blobService;
        private readonly IPhotoRepository _photoRepository;

        public GetPhotoQueryHandler(BlobServiceClient blobService, IPhotoRepository photoRepository)
        {
            _blobService = blobService;
            _photoRepository = photoRepository;
        }

        async Task<PhotoDto> IRequestHandler<GetPhotoQuery, PhotoDto>.Handle(GetPhotoQuery request, CancellationToken cancellationToken)
        {
            var containerName = "photos";
            BlobContainerClient containerClient = _blobService.GetBlobContainerClient(containerName);

            await containerClient.CreateIfNotExistsAsync();

            var EntityPhoto = await _photoRepository.GetById(request.Id);

            if (EntityPhoto == null)
            {
                throw new ItemNotFoundException("Photo not found");
            }
            var photoName = EntityPhoto.FileName;

            BlobClient blobClient = containerClient.GetBlobClient(photoName);

            var downloadResponse = await blobClient.DownloadContentAsync();
            var content = downloadResponse.Value.Content.ToStream();
            var contentType = blobClient.GetProperties().Value.ContentType;

            var dto = new PhotoDto()
            {
                Content = content,
                ContentType = contentType,
                PhotoName = photoName,
            };

            return dto;
        }
    }
}