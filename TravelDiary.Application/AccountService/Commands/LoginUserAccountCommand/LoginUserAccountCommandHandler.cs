using MediatR;
using Microsoft.AspNetCore.Identity;
using TravelDiary.Domain.Entities;
using TravelDiary.Domain.Interfaces;
using TravelDiary.Domain.Models;

namespace TravelDiary.Application.AccountService.Commands.LoginUserAccountCommand
{
    public class LoginUserAccountCommandHandler : IRequestHandler<LoginUserAccountCommand, string>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly AuthenticationSettings _authenticationSettings;

        public LoginUserAccountCommandHandler(IAccountRepository repository, IPasswordHasher<User> passwordHasher, AuthenticationSettings authenticationSettings)
        {
            _accountRepository = repository;
            _passwordHasher = passwordHasher;
            _authenticationSettings = authenticationSettings;
        }

        public Task<string> Handle(LoginUserAccountCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}