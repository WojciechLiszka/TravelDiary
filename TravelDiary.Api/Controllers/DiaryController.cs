using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelDiary.Application.DiaryService.Commands.CreateDiary;
using TravelDiary.Application.DiaryService.Commands.UpdateDiaryDescription;

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
        [Route("{diaryId}/Descryption")]
        public async Task<ActionResult> UpdateDescription([FromRoute] int diaryId, [FromBody] string description)
        {
            var Command = new UpdateDiaryDescriptionCommand()
            {
                Id = diaryId,
                Description = description
            };
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            await _mediator.Send(Command);
            return Ok();
        }
    }
}