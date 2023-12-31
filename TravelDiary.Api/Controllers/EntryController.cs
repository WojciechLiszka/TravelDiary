﻿using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelDiary.Application.EntryService.Command.AddEntry;
using TravelDiary.Application.EntryService.Command.DeleteEntry;
using TravelDiary.Application.EntryService.Command.UpdateEntry;
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
        [Authorize]
        public async Task<ActionResult<string>> AddEntryToDiary([FromRoute] int diaryId, [FromBody] CreateEntryDto dto)
        {
            var command = new AddEntryCommand()
            {
                DiaryId = diaryId,
                Date = dto.Date,
                Tittle = dto.Tittle,
                Description = dto.Description,
            };
            var validator = new AddEntryCommandValidator();

            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return BadRequest();
            }

            var Id = await _mediator.Send(command);

            return Created($"/Api/EntryController/{Id}", null);
        }

        [HttpDelete]
        [Route("Entry/{entryId}")]
        [Authorize]
        public async Task<ActionResult> DeleteEntry([FromRoute] int entryId)
        {
            var command = new DeleteEntryCommand()
            {
                EntryId = entryId
            };

            await _mediator.Send(command);

            return NoContent();
        }

        [HttpPut]
        [Route("Entry/{entryId}")]
        [Authorize]
        public async Task<ActionResult> UpdateEntry([FromRoute] int entryId, [FromBody] CreateEntryDto dto)
        {
            var command = new UpdateEntryCommand()
            {
                EntryId = entryId,
                Date = dto.Date,
                Tittle = dto.Tittle,
                Description = dto.Description,
            };

            var validator = new UpdateEntryCommandValidator();

            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return BadRequest();
            }

            await _mediator.Send(command);

            return Ok();
        }
    }
}