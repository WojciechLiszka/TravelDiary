using FluentValidation;

namespace TravelDiary.Application.AccountService.Commands.UpdateUserAccountDetailsCommand
{
    public class UpdateUserDetailsCommandValidator : AbstractValidator<UpdateUserDetailsCommand>
    {
        public UpdateUserDetailsCommandValidator()
        {
            RuleFor(x => x.FirstName)
                .NotNull()
                .NotEmpty()
                .Length(3, 20);

            RuleFor(x => x.LastName)
                .NotNull()
                .NotEmpty()
                .Length(3, 20);

            RuleFor(x => x.Country)
                .NotNull()
                .NotEmpty()
                .Length(3, 20);
        }
    }
}