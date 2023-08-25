using MediatR;
using Microsoft.AspNetCore.Authorization;
using TravelDiary.Application.Authorization;
using TravelDiary.Domain.Exceptions;
using TravelDiary.Domain.Interfaces;
using TravelDiary.Domain.Models;

namespace TravelDiary.Application.PhotoService.Command.UpdatePhotoDetails
{
    public class UpdatePhotoDetailsCommandHandler : IRequestHandler<UpdatePhotoDetailsCommand>
    {
        private readonly IEntryRepository _entryRepository;
        private readonly IDiaryRepository _diaryRepository;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserContextService _userContextService;
        private readonly IPhotoRepository _photoRepository;

        public UpdatePhotoDetailsCommandHandler(IEntryRepository entryRepository, IDiaryRepository diaryRepository, IAuthorizationService authorizationService, IUserContextService userContextService, IPhotoRepository photoRepository)
        {
            _entryRepository = entryRepository;
            _diaryRepository = diaryRepository;
            _authorizationService = authorizationService;
            _userContextService = userContextService;
            _photoRepository = photoRepository;
        }

        public async Task Handle(UpdatePhotoDetailsCommand request, CancellationToken cancellationToken)
        {
            var photo = await _photoRepository.GetById(request.PhotoId);
            if (photo == null)
            {
                throw new ItemNotFoundException("Entry not found");
            }

            var entry = await _entryRepository.GetById(photo.EntryId);

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

            photo.Tittle = request.Title;
            photo.Description = request.Description;

            await _photoRepository.Commit();
        }
    }
}