using MediatR;
using Microsoft.AspNetCore.Authorization;
using TravelDiary.Application.Authorization;
using TravelDiary.Domain.Exceptions;
using TravelDiary.Domain.Interfaces;
using TravelDiary.Domain.Models;

namespace TravelDiary.Application.EntryService.UpdateEntry
{
    public class UpdateEntryCommandHandler : IRequestHandler<UpdateEntryCommand>
    {
        private readonly IUserContextService _userContextService;
        private readonly IDiaryRepository _diaryRepository;
        private readonly IAuthorizationService _authorizationService;
        private readonly IEntryRepository _entryRepository;

        public UpdateEntryCommandHandler(IUserContextService userContextService, IDiaryRepository diaryRepository, IAuthorizationService authorizationService, IEntryRepository entryRepository)
        {
            _userContextService = userContextService;
            _diaryRepository = diaryRepository;
            _authorizationService = authorizationService;
            _entryRepository = entryRepository;
        }

        public async Task Handle(UpdateEntryCommand request, CancellationToken cancellationToken)
        {
            var entry = await _entryRepository.GetById(request.EntryId);
            if (entry == null)
            {
                throw new ItemNotFoundException("Entry not found");
            }
            var diary = _diaryRepository.GetById(entry.DiaryId);
            if (diary == null)
            {
                throw new Exception("Entry without Diary");
            }
            var authorizationResult = await _authorizationService.AuthorizeAsync(_userContextService.User, diary, new DiaryResourceOperationRequirement(ResourceOperation.Update));

            if (!authorizationResult.Succeeded)
            {
                throw new ForbiddenException();
            }

            entry.Tittle = request.Tittle;
            entry.Description = request.Description;
            entry.Date = request.Date;

            await _entryRepository.Commit();

        }
    }
}