using MediatR;
using TravelDiary.Domain.Dtos;
using TravelDiary.Domain.Models;

namespace TravelDiary.Application.DiaryService.Queries.GetDiaries
{
    public class GetDiariesQuery : PaginationQueryDto, IRequest<PagedResult<GetDiaryDto>>
    {
        public string OrderBy { get; set; }

    }
}