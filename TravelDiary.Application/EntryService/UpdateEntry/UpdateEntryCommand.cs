using MediatR;
using TravelDiary.Domain.Dtos;

namespace TravelDiary.Application.EntryService.UpdateEntry
{
    public class UpdateEntryCommand : CreateEntryDto ,IRequest
    {
        public int EntryId { get; set; }
    }
}