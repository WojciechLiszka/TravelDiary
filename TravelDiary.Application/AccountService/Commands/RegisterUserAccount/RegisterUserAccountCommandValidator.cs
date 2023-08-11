using FluentValidation;
using TravelDiary.Domain.Interfaces;

namespace TravelDiary.Application.AccountService.Commands.RegisterUserAccount
{
    public class RegisterUserCommandValidaTor : AbstractValidator<RegisterUserAccountCommand>
    {
        public RegisterUserCommandValidaTor(IAccountRepository repository)
        {
            RuleFor(x => x.Email)
                    .NotEmpty()
                    .EmailAddress()
                    .Length(5, 254);

            RuleFor(x => x.Password).MinimumLength(6);

            RuleFor(x => x.ConfirmPassword).Equal(e => e.Password);

            RuleFor(x => x.Email)
                    .Custom((value, context) =>
                    {
                        var emailInUse = repository.EmailInUse(value);
                        if (emailInUse)
                        {
                            context.AddFailure("Email", "That email is taken");
                        }
                    });

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

            RuleFor(x => x.NickName)
               .NotNull()
               .NotEmpty()
               .Length(3, 20);
        }
    }
}