using MediatR;
using Microsoft.AspNetCore.Mvc;
using TravelDiary.Application.PhotoService.Command.AddPhotoToEntry;
using TravelDiary.Domain.Models;

namespace TravelDiary.Api.Controllers
{
    [ApiController]
    [Route("/Api")]
    public class PhotoController : Controller
    {
        private readonly IMediator _mediator;

        public PhotoController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("/Entry/{entryId}/Photo")]
        public async Task<ActionResult<string>> AddPhotoToEntry([FromRoute] int entryId, IFormFile file)
        {
            var command = new AddPhotoToEntryCommand()
            {
                EntryId = entryId,
                File = file
            };

            var id = await _mediator.Send(command);

            return Ok(id);
        }
    }
}