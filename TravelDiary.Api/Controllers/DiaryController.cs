using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelDiary.Application.DiaryService.Commands.CreateDiary;
using TravelDiary.Application.DiaryService.Commands.DeleteDiary;
using TravelDiary.Application.DiaryService.Commands.UpdateDiaryDescription;
using TravelDiary.Application.DiaryService.Queries.GetById;
using TravelDiary.Domain.Dtos;

namespace TravelDiary.Api.Controllers
{
    [ApiController]
    [Route("/Api/Diary")]
    public class DiaryController : Controller
    {
        private readonly IMediator _mediator;

        public DiaryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Create(CreateDiaryCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok($"/Api/Diary/{id}");
        }

        [HttpPut]
        [Authorize]
        [Route("{diaryId}/Description")]
        public async Task<ActionResult> UpdateDescription([FromRoute] int diaryId, [FromBody] string description)
        {
            var Command = new UpdateDiaryDescriptionCommand()
            {
                Id = diaryId,
                Description = description
            };

            var validator = new UpdateDiaryDescriptionCommandValidator();

            var validationResult = await validator.ValidateAsync(Command);
            if (!validationResult.IsValid)
            {
                return BadRequest();
            }
            await _mediator.Send(Command);

            return Ok();
        }

        [HttpDelete]
        [Authorize]
        [Route("{diaryId}")]
        public async Task<ActionResult> DeleteDiary([FromRoute] int diaryId)
        {
            var Command = new DeleteDiaryCommand()
            {
                Id = diaryId
            };
            await _mediator.Send(Command);
            return NoContent();
        }

        [HttpGet]
        [Route("{diaryId}")]
        public async Task<ActionResult<GetDiaryDto>> GetById([FromRoute] int diaryId)
        {
            var query = new GetDiaryByIdQuery()
            {
                Id = diaryId
            };
            var result = await _mediator.Send(query);

            return Ok(result);
        }
    }
}