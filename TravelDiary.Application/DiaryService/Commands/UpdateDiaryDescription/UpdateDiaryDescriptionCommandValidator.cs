using FluentValidation;

namespace TravelDiary.Application.DiaryService.Commands.UpdateDiaryDescription
{
    public class UpdateDiaryDescriptionCommandValidator : AbstractValidator<UpdateDiaryDescriptionCommand>
    {
        public UpdateDiaryDescriptionCommandValidator()
        {
            RuleFor(x => x.Description)
                .MinimumLength(1)
                .MaximumLength(500);
        }
    }
}