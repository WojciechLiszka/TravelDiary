using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq.Expressions;
using System.Security.Claims;
using TravelDiary.Application.Authorization;
using TravelDiary.Domain.Dtos;
using TravelDiary.Domain.Entities;
using TravelDiary.Domain.Interfaces;
using TravelDiary.Domain.Models;

namespace TravelDiary.Application.DiaryService.Queries.GetDiaries
{
    public class GetDiariesQueryHandler : IRequestHandler<GetDiariesQuery, PagedResult<GetDiaryDto>>
    {
        private readonly IDiaryRepository _diaryRepository;
        private readonly IUserContextService _userContextService;
 

        public GetDiariesQueryHandler(IDiaryRepository diaryRepository, IUserContextService userContextService)
        {
            _diaryRepository = diaryRepository;
            _userContextService = userContextService;
        
        }

        public async Task<PagedResult<GetDiaryDto>> Handle(GetDiariesQuery request, CancellationToken cancellationToken)
        {
            var baseQuery = _diaryRepository.Search(request.SearchPhrase);
            if (!string.IsNullOrEmpty(request.SortBy))
            {
                var columnsSelectors = new Dictionary<string, Expression<Func<Diary, object>>>
                {
                    { nameof(Diary.Name), b => b.Name },
                    { nameof(Diary.Description), b => b.Description },
                };
                var selectedColumn = columnsSelectors[request.SortBy];

                baseQuery = request.SortDirection == SortDirection.ASC
                   ? baseQuery.OrderBy(selectedColumn)
                   : baseQuery.OrderByDescending(selectedColumn);
            }
            var diaries = await baseQuery
                .Skip(request.PageSize * (request.PageNumber - 1))
                .Take(request.PageSize)
                .ToListAsync();

            var totalItemsCount = baseQuery.Count();
            var dtos = diaries.Adapt<List<GetDiaryDto>>();
            var pagedResult = new PagedResult<GetDiaryDto>(dtos, totalItemsCount, request.PageSize, request.PageNumber);
            return pagedResult;
        }
    }
}