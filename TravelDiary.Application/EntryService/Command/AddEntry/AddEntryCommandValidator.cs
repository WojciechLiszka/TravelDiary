using FluentValidation;

namespace TravelDiary.Application.EntryService.Command.AddEntry
{
    public class AddEntryCommandValidator : AbstractValidator<AddEntryCommand>
    {
        public AddEntryCommandValidator()
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