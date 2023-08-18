﻿using MediatR;
using Microsoft.AspNetCore.Mvc;
using TravelDiary.Application.EntryService.Command;
using TravelDiary.Domain.Dtos;

namespace TravelDiary.Api.Controllers
{
    [ApiController]
    [Route("/Api")]
    public class EntryController : Controller
    {
        private readonly IMediator _mediator;

        public EntryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("Diary/{diaryId}/Entry")]
        public async Task<ActionResult<string>> AddEntryToDiary([FromRoute] int diaryId, [FromBody] CreateEntryDto dto)
        {
            var command = new AddEntryCommand()
            {
                DiaryId = diaryId,
                Date = dto.Date,
                Tittle = dto.Tittle,
                Description = dto.Description,
            };
            var Id = await _mediator.Send(command);

            return Created($"/Api/EntryController/{Id}", null);
        }
    }
}