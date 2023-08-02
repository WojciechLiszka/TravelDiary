using MediatR;
using Microsoft.AspNetCore.Mvc;
using TravelDiary.Application.AccountService.Commands.RegisterUserAccountCommand;

namespace TravelDiary.Api.Controllers
{
    [ApiController]
    [Route("/Api/Account")]
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost]
        [Route("Register")]
        public async Task<ActionResult> Register([FromBody] RegisterUserAccountCommand command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            await _mediator.Send(command);
            return Ok();
        }
    }
}