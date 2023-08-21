using MediatR;

namespace TravelDiary.Application.EntryService.Command.DeleteEntry
{
    public class DeleteEntryCommand : IRequest
    {
        public int EntryId { get; set; }
    }
}