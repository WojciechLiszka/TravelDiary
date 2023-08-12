using MediatR;
using TravelDiary.Domain.Dtos;

namespace TravelDiary.Application.DiaryService.Queries.GetById
{
    public class GetDiaryByIdQuery : IRequest<GetDiaryDto>
    {
        public int Id { get; set; }
    }
}