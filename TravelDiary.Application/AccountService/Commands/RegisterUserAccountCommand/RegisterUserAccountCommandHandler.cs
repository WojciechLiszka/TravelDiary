using MediatR;
using Microsoft.AspNetCore.Identity;
using TravelDiary.Domain.Entities;
using TravelDiary.Domain.Interfaces;

namespace TravelDiary.Application.AccountService.Commands.RegisterUserAccountCommand
{
    public class RegisterUserAccountCommandHandler : IRequestHandler<RegisterUserAccountCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;

        public RegisterUserAccountCommandHandler(IUserRepository userRepository, IPasswordHasher<User> passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task Handle(RegisterUserAccountCommand request, CancellationToken cancellationToken)
        {
            var newUser = new User()
            {
                NickName = request.NickName,
                UserDetails = new UserDetails()
                {
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Country = request.Country
                }
            };

            var passwordhash = _passwordHasher.HashPassword(newUser, request.Password);

            newUser.PasswordHash = passwordhash;

            await _userRepository.Register(newUser);

        }
    }
}