using MediatR;
using Microsoft.AspNetCore.Mvc;
using TravelDiary.Application.DiaryService.Commands.CreateDiaryCommand;

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
        public async Task<ActionResult> Create(CreateDiaryCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(id);
        }
    }
}