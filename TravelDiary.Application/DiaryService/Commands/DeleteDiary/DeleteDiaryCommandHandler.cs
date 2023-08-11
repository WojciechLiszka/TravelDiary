using MediatR;
using Microsoft.AspNetCore.Authorization;
using TravelDiary.Application.Authorization;
using TravelDiary.Domain.Exceptions;
using TravelDiary.Domain.Interfaces;
using TravelDiary.Domain.Models;

namespace TravelDiary.Application.DiaryService.Commands.DeleteDiary
{
    public class DeleteDiaryCommandHandler : IRequestHandler<DeleteDiaryCommand>
    {
        private readonly IUserContextService _userContextService;
        private readonly IDiaryRepository _diaryRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IAuthorizationService _authorization;

        public DeleteDiaryCommandHandler(IUserContextService userContextService, IDiaryRepository diaryRepository, IAccountRepository accountRepository, IAuthorizationService authorization)
        {
            _userContextService = userContextService;
            _diaryRepository = diaryRepository;
            _accountRepository = accountRepository;
            _authorization = authorization;
        }

        public async Task Handle(DeleteDiaryCommand request, CancellationToken cancellationToken)
        {
            var diary = await _diaryRepository.GetById(request.Id);

            if (diary == null)
            {
                throw new ItemNotFoundException("Diary Not Found");
            }
            var authorizationResult = await _authorization.AuthorizeAsync(_userContextService.User, diary, new DiaryResourceOperationRequirement(ResourceOperation.Delete));
            if (!authorizationResult.Succeeded)
            {
                throw new ForbiddenException();
            }
            await _diaryRepository.Delete(diary);
        }
    }
}