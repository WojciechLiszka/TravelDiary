using MediatR;
using TravelDiary.Domain.Dtos;

namespace TravelDiary.Application.EntryService.Command.UpdateEntry
{
    public class UpdateEntryCommand : CreateEntryDto, IRequest
    {
        public int EntryId { get; set; }
    }
}