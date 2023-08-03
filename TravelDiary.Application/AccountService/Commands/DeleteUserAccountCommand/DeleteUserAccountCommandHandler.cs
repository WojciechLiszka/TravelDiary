using MediatR;
using Microsoft.AspNetCore.Identity;
using TravelDiary.Domain.Entities;
using TravelDiary.Domain.Exceptions;
using TravelDiary.Domain.Interfaces;

namespace TravelDiary.Application.AccountService.Commands.DeleteUserAccountCommand
{
    public class DeleteUserAccountCommandHandler : IRequestHandler<DeleteUserAccountCommand>
    {
        private readonly IUserContextService _userContextService;
        private readonly IAccountRepository _accountRepository;
        private readonly IPasswordHasher<User> _passwordHasher;

        public DeleteUserAccountCommandHandler(IUserContextService userContextService, IAccountRepository accountRepository, IPasswordHasher<User> passwordHasher)
        {
            _userContextService = userContextService;
            _accountRepository = accountRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task Handle(DeleteUserAccountCommand request, CancellationToken cancellationToken)
        {
            var userEmail = _userContextService.GetUserEmail;
            if (userEmail == null)
            {
                throw new Exception("Invalid User Token");
            }
            var user = await _accountRepository.GetByEmail(userEmail);
            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                throw new BadRequestException("Invalid password");
            }
            await _accountRepository.Delete(user);

        }
    }
}