using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TravelDiary.Application.Authorization;
using TravelDiary.Domain.Dtos;
using TravelDiary.Domain.Entities;
using TravelDiary.Domain.Exceptions;
using TravelDiary.Domain.Interfaces;

namespace TravelDiary.Application.DiaryService.Queries.GetById
{
    public class GetDiaryByIdQueryHandler : IRequestHandler<GetDiaryByIdQuery, GetDiaryDto>
    {
        private readonly IDiaryRepository _diaryRepository;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserContextService _userContextService;

        public GetDiaryByIdQueryHandler(IDiaryRepository diaryRepository, IAuthorizationService authorizationService, IUserContextService userContextService)
        {
            _diaryRepository = diaryRepository;
            _authorizationService = authorizationService;
            _userContextService = userContextService;
            
        }

        public async Task<GetDiaryDto> Handle(GetDiaryByIdQuery request, CancellationToken cancellationToken)
        {
            var diary = await _diaryRepository.GetById(request.Id);

            if (diary == null)
            {
                throw new ItemNotFoundException("DiaryNotFound");
            }

                var authorizationResult = await _authorizationService.AuthorizeAsync(_userContextService.User, diary, new DiaryResourceOperationRequirement(Domain.Models.ResourceOperation.Read));

            if (!authorizationResult.Succeeded)
            {
                throw new ForbiddenException();
            }

            return diary.Adapt<GetDiaryDto>();
        }
    }
}