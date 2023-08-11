using FluentValidation;

namespace TravelDiary.Application.DiaryService.Commands.CreateDiary
{
    public class CreateDiaryCommandValidator : AbstractValidator<CreateDiaryCommand>
    {
        public CreateDiaryCommandValidator()
        {
            RuleFor(x => x.Description)
                .NotEmpty()
                .NotNull()
                .MinimumLength(1)
                .MaximumLength(500);

            RuleFor(x => x.Name)
                .NotNull()
                .NotEmpty()
                .MinimumLength(5)
                .MaximumLength(50);

            RuleFor(x => x.Starts)
                .NotEmpty()
                .NotNull();

            RuleFor(x => x.Policy)
                .NotEmpty()
                .NotNull();
        }
    }
}