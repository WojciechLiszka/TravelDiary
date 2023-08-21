using MediatR;
using TravelDiary.Domain.Dtos;

namespace TravelDiary.Application.EntryService.Command.AddEntry
{
    public class AddEntryCommand : CreateEntryDto, IRequest<string>
    {
        public int DiaryId { get; set; }
    }
}