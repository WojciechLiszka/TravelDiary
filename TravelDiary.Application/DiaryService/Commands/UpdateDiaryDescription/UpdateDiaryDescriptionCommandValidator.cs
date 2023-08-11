using FluentValidation;

namespace TravelDiary.Application.DiaryService.Commands.UpdateDiaryDescription
{
    public class UpdateDiaryDescriptionCommandValidator : AbstractValidator<UpdateDiaryDescriptionCommand>
    {
        public UpdateDiaryDescriptionCommandValidator()
        {
            RuleFor(x => x.Description)
                .NotEmpty()
                .NotNull()
                .MinimumLength(1)
                .MaximumLength(500);
        }
    }
}