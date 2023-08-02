using MediatR;
using Microsoft.AspNetCore.Identity;
using TravelDiary.Domain.Entities;

namespace TravelDiary.Application.AccountService.Commands.LoginUserAccountCommand
{
    public class LoginUserAccountCommandHandler : IRequestHandler<LoginUserAccountCommand, string>
    {
        public LoginUserAccountCommandHandler(IAccountRepository repository, IPasswordHasher<User> passwordHasher, AuthenticationSettings authenticationSettings)
        {
            
        }
        public Task<string> Handle(LoginUserAccountCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}