using MediatR;
using Microsoft.AspNetCore.Authorization;
using TravelDiary.Application.Authorization;
using TravelDiary.Domain.Exceptions;
using TravelDiary.Domain.Interfaces;
using TravelDiary.Domain.Models;

namespace TravelDiary.Application.EntryService.Command.DeleteEntry
{
    public class DeleteEntryCommandHandler : IRequestHandler<DeleteEntryCommand>
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserContextService _userContextService;
        private readonly IDiaryRepository _diaryRepository;
        private readonly IEntryRepository _entryRepository;

        public DeleteEntryCommandHandler(IAuthorizationService authorizationService, IUserContextService userContextService, IDiaryRepository diaryRepository, IEntryRepository entryRepository)
        {
            _authorizationService = authorizationService;
            _userContextService = userContextService;
            _diaryRepository = diaryRepository;
            _entryRepository = entryRepository;
        }

        public async Task Handle(DeleteEntryCommand request, CancellationToken cancellationToken)
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

            await _entryRepository.Delete(entry);
        }
    }
}