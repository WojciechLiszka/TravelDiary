using MediatR;
using Microsoft.AspNetCore.Authorization;
using TravelDiary.Application.Authorization;
using TravelDiary.Domain.Entities;
using TravelDiary.Domain.Exceptions;
using TravelDiary.Domain.Interfaces;
using TravelDiary.Domain.Models;

namespace TravelDiary.Application.EntryService.Command.AddEntry
{
    public class AddEntryCommandHandler : IRequestHandler<AddEntryCommand, string>
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IDiaryRepository _diaryRepository;
        private readonly IEntryRepository _entryRepository;
        private readonly IUserContextService _userContextService;

        public AddEntryCommandHandler(IAuthorizationService authorizationService, IDiaryRepository diaryRepository, IEntryRepository entryRepository, IUserContextService userContextService)
        {
            _authorizationService = authorizationService;
            _diaryRepository = diaryRepository;
            _entryRepository = entryRepository;
            _userContextService = userContextService;
        }

        async Task<string> IRequestHandler<AddEntryCommand, string>.Handle(AddEntryCommand request, CancellationToken cancellationToken)
        {
            var diary = await _diaryRepository.GetById(request.DiaryId);

            if (diary == null)
            {
                throw new ItemNotFoundException("Diary not found");
            }

            var authorizationResult = await _authorizationService.AuthorizeAsync(_userContextService.User, diary, new DiaryResourceOperationRequirement(ResourceOperation.Update));

            if (!authorizationResult.Succeeded)
            {
                throw new ForbiddenException();
            }
            var entry = new Entry()
            {
                Date = request.Date,
                Tittle = request.Tittle,
                Description = request.Description,
                DiaryId = request.DiaryId,
            };
            await _entryRepository.Create(entry);

            return entry.Id.ToString();
        }
    }
}