using MediatR;
using Microsoft.AspNetCore.Http;
using TravelDiary.Domain.Models;

namespace TravelDiary.Application.PhotoService.Command.AddPhotoToEntry
{
    public class AddPhotoToEntryCommand : IRequest<string>
    {
        public IFormFile File { get; set; } = null!;
        public int EntryId { get; set; }
    }
}