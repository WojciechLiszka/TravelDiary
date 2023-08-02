using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TravelDiary.Domain.Entities;
using TravelDiary.Domain.Exceptions;
using TravelDiary.Domain.Interfaces;
using TravelDiary.Domain.Models;

namespace TravelDiary.Application.AccountService.Commands.LoginUserAccountCommand
{
    public class LoginUserAccountCommandHandler : IRequestHandler<LoginUserAccountCommand, string>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly AuthenticationSettings _authenticationSettings;
        private readonly IUserRoleRepository _userRoleRepository;

        public LoginUserAccountCommandHandler(IAccountRepository repository, IPasswordHasher<User> passwordHasher, AuthenticationSettings authenticationSettings, IUserRoleRepository userRoleRepository)
        {
            _accountRepository = repository;
            _passwordHasher = passwordHasher;
            _authenticationSettings = authenticationSettings;
            _userRoleRepository = userRoleRepository;
        }

        public async Task<string> Handle(LoginUserAccountCommand request, CancellationToken cancellationToken)
        {
            var user = await _accountRepository.GetByEmail(request.Email);

            if (user == null)
            {
                throw new BadRequestException("Invalid username or password");
            }
            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                throw new BadRequestException("Invalid username or password");
            }
            var role = await _userRoleRepository.GetById(user.UserRoleId);
            if (role == null)
            {
                throw new Exception("Role not found");
            }

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.UserDetails.Email),
                new Claim(ClaimTypes.Name, $"{user.NickName}"),
                new Claim(ClaimTypes.Role, $"{role.RoleName}")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.JwtKey));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(_authenticationSettings.JwtExpireDays);

            var token = new JwtSecurityToken(_authenticationSettings.JwtIssuer,
                _authenticationSettings.JwtIssuer,
                claims,
                expires: expires,
                signingCredentials: cred);

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }
    }
}