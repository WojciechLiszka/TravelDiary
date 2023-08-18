using FluentValidation;

namespace TravelDiary.Application.EntryService.UpdateEntry
{
    public class UpdateEntryCommandValidator : AbstractValidator<UpdateEntryCommand>
    {
        public UpdateEntryCommandValidator()
        {
            RuleFor(x => x.Tittle)
               .NotNull()
               .NotEmpty()
               .MinimumLength(5)
               .MaximumLength(50);

            RuleFor(x => x.Description)
                .MaximumLength(1000);

            RuleFor(x => x.Date)
                .NotEmpty()
                .NotNull();
        }
    }
}