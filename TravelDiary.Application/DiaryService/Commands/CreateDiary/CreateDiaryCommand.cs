using MediatR;
using TravelDiary.Domain.Dtos;

namespace TravelDiary.Application.DiaryService.Commands.CreateDiary
{
    public class CreateDiaryCommand : CreateDiaryDto, IRequest<string>
    {
    }
}