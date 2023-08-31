using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using TravelDiary.Application.Authorization;
using TravelDiary.Domain.Entities;
using TravelDiary.Domain.Exceptions;
using TravelDiary.Domain.Interfaces;
using TravelDiary.Domain.Models;

namespace TravelDiary.Application.PhotoService.Command.AddPhotoToEntry
{
    public class AddPhotoToEntryCommandHandler : IRequestHandler<AddPhotoToEntryCommand, string>
    {
        private readonly BlobServiceClient _blobService;
        private readonly IEntryRepository _entryRepository;
        private readonly IDiaryRepository _diaryRepository;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserContextService _userContextService;
        private readonly IPhotoRepository _photoRepository;

        public AddPhotoToEntryCommandHandler(BlobServiceClient blobService, IEntryRepository entryRepository, IDiaryRepository diaryRepository, IAuthorizationService authorizationService, IUserContextService userContextService, IPhotoRepository photoRepository)
        {
            _blobService = blobService;
            _entryRepository = entryRepository;
            _diaryRepository = diaryRepository;
            _authorizationService = authorizationService;
            _userContextService = userContextService;
            _photoRepository = photoRepository;
        }

        public async Task<string> Handle(AddPhotoToEntryCommand request, CancellationToken cancellationToken)
        {
            var entry = await _entryRepository.GetById(request.EntryId);

            if (entry == null)
            {
                throw new ItemNotFoundException("Entry not found");
            }
            var diary = await _diaryRepository.GetById(entry.DiaryId);
            if (diary == null)
            {
                throw new Exception("Entry without Diary");
            }
            var authorizationResult = await _authorizationService.AuthorizeAsync(_userContextService.User, diary, new DiaryResourceOperationRequirement(ResourceOperation.Update));

            if (!authorizationResult.Succeeded)
            {
                throw new ForbiddenException();
            }
            var containerName = "photos";

            BlobContainerClient containerClient = _blobService.GetBlobContainerClient(containerName);

            await containerClient.CreateIfNotExistsAsync();

            BlobClient blobClient = containerClient.GetBlobClient(request.File.FileName);

            var blobHttpHeaders = new BlobHttpHeaders();
            blobHttpHeaders.ContentType = request.File.ContentType;

            await blobClient.UploadAsync(request.File.OpenReadStream(), blobHttpHeaders);

            var entityPhoto = new Photo()
            {
                EntryId = entry.Id,
                FileName = request.File.FileName,
                Description = "",
                Tittle = request.File.FileName,
            };
            await _photoRepository.Create(entityPhoto);

            return entityPhoto.Id.ToString();
        }
    }
}