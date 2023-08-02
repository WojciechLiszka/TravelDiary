using MediatR;
using TravelDiary.Domain.Dtos;

namespace TravelDiary.Application.DiaryService.Commands.CreateDiaryCommand
{
    public class CreateDiaryCommand : CreateDiaryDto, IRequest<string>
    {
    }
}