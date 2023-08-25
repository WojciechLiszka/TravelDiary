using MediatR;
using Microsoft.AspNetCore.Mvc;
using TravelDiary.Application.PhotoService.Command.AddPhotoToEntry;
using TravelDiary.Application.PhotoService.Command.UpdatePhotoDetails;
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
        [Route("Entry/{entryId}/Photo")]
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

        [HttpPut]
        [Route("Photo/{photoId}")]
        public async Task<ActionResult> UpdatePhotoDetails([FromRoute] Guid photoId, PhotoDetails photoDetails)
        {
            var command = new UpdatePhotoDetailsCommand()
            {
                PhotoId = photoId,
                Description = photoDetails.Description,
                Title = photoDetails.Title
            };

            await _mediator.Send(command);

            return Ok();
        }
    }
}