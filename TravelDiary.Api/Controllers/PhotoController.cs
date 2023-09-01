using MediatR;
using Microsoft.AspNetCore.Mvc;
using TravelDiary.Application.PhotoService.Command.AddPhotoToEntry;
using TravelDiary.Application.PhotoService.Command.UpdatePhotoDetails;
using TravelDiary.Application.PhotoService.Queries;
using TravelDiary.Application.PhotoService.Queries.GetPhotoDetails;
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

        [HttpGet]
        [Route("Photo/{photoId}")]
        public async Task<IActionResult> Download([FromRoute] Guid photoId)
        {
            var query = new GetPhotoQuery()
            {
                Id = photoId
            };
            var response = await _mediator.Send(query);

            if (response.PhotoName == null || response.Content == null || response.ContentType == null)
            {
                return NotFound();
            }

            return File(response.Content, response.ContentType, fileDownloadName: response.PhotoName);
        }

        [HttpGet]
        [Route("Photo/{photoId}/Details")]
        public async Task<ActionResult<PhotoDetails>> GetPhotoDetails([FromRoute] Guid photoId)
        {
            var query = new GetPhotoDetailsQuery()
            {
                Id = photoId
            };

            var response = await _mediator.Send(query);

            return Ok(response);
        }
    }
}