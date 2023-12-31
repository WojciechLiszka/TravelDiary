﻿using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using TravelDiary.ApiTests.Helpers;
using TravelDiary.Application.AccountService.Commands.LoginUserAccount;
using TravelDiary.Application.AccountService.Commands.RegisterUserAccount;
using TravelDiary.Application.AccountService.Commands.UpdateUserAccountDetails;
using TravelDiary.Domain.Entities;
using TravelDiary.Domain.Interfaces;
using TravelDiary.Domain.Models;
using TravelDiary.Infrastructure.Persistence;

namespace TravelDiary.ApiTests.Controllers
{
    public class AccountControllerTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private const string _route = "Api/Account";
        private readonly AuthenticationSettings _authenticationSettings;
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;
        private readonly WebApplicationFactory<Program> _factory;
        private readonly Mock<IPasswordHasher<User>> _passwordHasherMock = new Mock<IPasswordHasher<User>>();
        private HttpClient _userClient;

        public AccountControllerTest(WebApplicationFactory<Program> factory)
        {
            _configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
            var authenticationSettings = new AuthenticationSettings();

            _configuration.GetSection("Authentication").Bind(authenticationSettings);
            _authenticationSettings = authenticationSettings;

            _factory = factory
               .WithWebHostBuilder(builder =>
               {
                   builder.ConfigureServices(services =>
                   {
                       var dbContextOptions = services
                           .SingleOrDefault(service => service.ServiceType == typeof(Microsoft.EntityFrameworkCore.DbContextOptions<TravelDiaryDbContext>));
                       services.Remove(dbContextOptions);

                       services.AddSingleton<IPasswordHasher<User>>(_passwordHasherMock.Object);

                       services
                        .AddDbContext<TravelDiaryDbContext>(options => options.UseInMemoryDatabase("TravelDiaryDb")
                        .EnableSensitiveDataLogging());
                   });
               });

            _client = _factory.CreateClient();
            _userClient = _factory.CreateClient();
        }

        [Fact]
        public async Task Delete_ForInvalidPassword_ReturnsBadRequest()
        {
            // arrange
            var role = new UserRole()
            {
                RoleName = "User"
            };
            await SeedRole(role);

            var user = new User()
            {
                NickName = "JDoe",
                UserDetails = new UserDetails()
                {
                    Email = "test@email.com",
                    Country = "USA",
                    FirstName = "John",
                    LastName = "Doe"
                },

                PasswordHash = "validPassword",
                UserRoleId = role.Id,
            };
            await SeedUser(user);

            var validPassword = user.PasswordHash;
            _passwordHasherMock
                .Setup(e => e.VerifyHashedPassword(It.IsAny<User>(), It.IsAny<string>(), It.Is<string>(password => password == validPassword)))
                .Returns(PasswordVerificationResult.Success);

            var userToken = GenerateJwtToken(user, role);
            _userClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userToken);

            // act

            var response = await _userClient.DeleteAsync($"{_route}?password=InvalidPassword");
            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Delete_ForvalidPassword_ReturnsNoContent()
        {
            // arrange
            var role = new UserRole()
            {
                RoleName = "User"
            };
            await SeedRole(role);

            var user = new User()
            {
                NickName = "JDoe",
                UserDetails = new UserDetails()
                {
                    Email = "test@email.com",
                    Country = "USA",
                    FirstName = "John",
                    LastName = "Doe"
                },

                PasswordHash = "validPassword",
                UserRoleId = role.Id,
            };
            await SeedUser(user);

            var validPassword = user.PasswordHash;
            _passwordHasherMock
                .Setup(e => e.VerifyHashedPassword(It.IsAny<User>(), It.IsAny<string>(), It.Is<string>(password => password == validPassword)))
                .Returns(PasswordVerificationResult.Success);

            var userToken = GenerateJwtToken(user, role);
            _userClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userToken);

            // act

            var response = await _userClient.DeleteAsync($"{_route}?password={user.PasswordHash}");
            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
        }

        [Theory]
        [InlineData("test@email.com", "inValidpassword")]
        [InlineData("invalid@email.com", "inValidpassword")]
        public async Task LoginUser_ForInvalidParams_ReturnsBadRequest(string email, string password)
        {
            // arrange
            var role = new UserRole()
            {
                RoleName = "User"
            };
            await SeedRole(role);

            var validPassword = "password";
            _passwordHasherMock
                .Setup(e => e.VerifyHashedPassword(It.IsAny<User>(), It.IsAny<string>(), It.Is<string>(password => password == validPassword)))
                .Returns(PasswordVerificationResult.Success);

            var user = new User()
            {
                NickName = "JDoe",
                UserDetails = new UserDetails()
                {
                    Email = "test@email.com",
                    Country = "USA",
                    FirstName = "John",
                    LastName = "Doe"
                },

                PasswordHash = validPassword,
                UserRoleId = role.Id,
            };
            await SeedUser(user);
            var command = new LoginUserAccountCommand()
            {
                Email = email,
                Password = password,
            };
            var httpContent = command.ToJsonHttpContent();
            // act

            var response = await _client.PostAsync($"{_route}/Login", httpContent);
            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task LoginUser_ForValidParams_ReturnsOK()
        {
            // arrange
            var role = new UserRole()
            {
                RoleName = "User"
            };
            await SeedRole(role);

            var validPassword = "password";
            _passwordHasherMock
                .Setup(e => e.VerifyHashedPassword(It.IsAny<User>(), It.IsAny<string>(), It.Is<string>(password => password == validPassword)))
                .Returns(PasswordVerificationResult.Success);

            var user = new User()
            {
                NickName = "JDoe",
                UserDetails = new UserDetails()
                {
                    Email = "test@email.com",
                    Country = "USA",
                    FirstName = "John",
                    LastName = "Doe"
                },

                PasswordHash = validPassword,
                UserRoleId = role.Id,
            };
            await SeedUser(user);
            var command = new LoginUserAccountCommand()
            {
                Email = "test@email.com",
                Password = validPassword,
            };
            var httpContent = command.ToJsonHttpContent();
            // act

            var response = await _client.PostAsync($"{_route}/Login", httpContent);
            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Theory]
        [InlineData("test@email.com", "password", "notThesamepassword", "John", "Doe", "USA", "JDoe")] // Passwords are not the same
        [InlineData("", "password", "password", "John", "Doe", "USA", "JDoe")] // Empty email
        [InlineData("testemail.com", "password", "password", "John", "Doe", "USA", "JDoe")] // Invalid email format
        [InlineData("test@email.com", "", "password", "John", "Doe", "USA", "JDoe")] // Empty password
        [InlineData("test@email.com", "password", "password", "", "Doe", "USA", "JDoe")] // Empty first name
        [InlineData("test@email.com", "password", "password", "John", "", "USA", "JDoe")] // Empty last name
        [InlineData("test@email.com", "password", "password", "John", "Doe", "", "JDoe")] // Empty country
        [InlineData("test@email.com", "password", "password", "John", "Doe", "USA", "")] // Empty nickname
        [InlineData("test@email.com", "password", "password", "John", "Doe", "USA", "JDoeJDoeJDoeJDoeJDoeJDoeJDoeJDoeJDoe")] // Nickname too long
        [InlineData(null, "password", "password", "John", "Doe", "USA", "JDoe")] // Null email
        [InlineData("test@email.com", null, "password", "John", "Doe", "USA", "JDoe")] // Null password
        [InlineData("test@email.com", "password", "password", null, "Doe", "USA", "JDoe")] // Null first name
        [InlineData("test@email.com", "password", "password", "John", null, "USA", "JDoe")] // Null last name
        [InlineData("test@email.com", "password", "password", "John", "Doe", null, "JDoe")] // Null country
        [InlineData("test@email.com", "password", "password", "John", "Doe", "USA", null)] // Null nickname
        [InlineData("test@email.com", "password", null, "John", "Doe", "USA", "JDoe")] // Null confirm password
        [InlineData("test@email.com", "pass", "pass", "John", "Doe", "USA", "JDoe")] // Password less than 6 characters
        [InlineData(null, null, null, null, null, null, null)] // All values are null
        public async Task RegisterUser_ForInValidDtoParams_ReturnsBadRequest(string email, string password, string confirmPasswrod, string firstName, string lastName, string country, string nickname)
        {
            // arrange
            var role = new UserRole()
            {
                RoleName = "User"
            };
            await SeedRole(role);

            var command = new RegisterUserAccountCommand()
            {
                Email = email,
                Password = password,
                ConfirmPassword = confirmPasswrod,
                FirstName = firstName,
                LastName = lastName,
                Country = country,
                NickName = nickname
            };
            var httpContent = command.ToJsonHttpContent();
            // act

            var response = await _client.PostAsync($"{_route}/Register", httpContent);
            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task RegisterUser_ForValidDtoParams_ReturnsOk()
        {
            // arrange
            var role = new UserRole()
            {
                RoleName = "User"
            };
            await SeedRole(role);

            _passwordHasherMock
                .Setup(e => e.HashPassword(It.IsAny<User>(), It.IsAny<string>()))
               .Returns("HashedPassword");

            var command = new RegisterUserAccountCommand()
            {
                Email = "test@test.com",
                Password = "testpassword",
                ConfirmPassword = "testpassword",
                FirstName = "John",
                LastName = "Doe",
                Country = "USA",
                NickName = "JDoe"
            };
            var httpContent = command.ToJsonHttpContent();
            // act

            var response = await _client.PostAsync($"{_route}/Register", httpContent);
            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Theory]
        [InlineData(null, "Doe", "USA")]// firstName is null
        [InlineData("John", null, "USA")]// lastName is null
        [InlineData("John", "Doe", null)]// country is null
        [InlineData(null, null, null)]// all values is null
        public async Task Update_ForInvalidParams_ReturnsBadRequest(string firstName, string lastName, string country)
        {
            // arrange
            var role = new UserRole()
            {
                RoleName = "User"
            };
            await SeedRole(role);

            var user = new User()
            {
                NickName = "JDoe",
                UserDetails = new UserDetails()
                {
                    Email = "test@email.com",
                    Country = "USA",
                    FirstName = "John",
                    LastName = "Doe"
                },

                PasswordHash = "validPassword",
                UserRoleId = role.Id,
            };
            await SeedUser(user);

            var command = new UpdateUserDetailsCommand()
            {
                FirstName = firstName,
                LastName = lastName,
                Country = country
            };
            var httpContent = command.ToJsonHttpContent();

            var userToken = GenerateJwtToken(user, role);
            _userClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userToken);

            // act

            var response = await _userClient.PutAsync($"{_route}", httpContent);
            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Update_ForValidParams_ReturnsOK()
        {
            // arrange
            var role = new UserRole()
            {
                RoleName = "User"
            };
            await SeedRole(role);

            var user = new User()
            {
                NickName = "JDoe",
                UserDetails = new UserDetails()
                {
                    Email = "test@email.com",
                    Country = "USA",
                    FirstName = "John",
                    LastName = "Doe"
                },

                PasswordHash = "validPassword",
                UserRoleId = role.Id,
            };
            await SeedUser(user);

            var command = new UpdateUserDetailsCommand()
            {
                FirstName = "John",
                LastName = "Doe",
                Country = "USA"
            };
            var httpContent = command.ToJsonHttpContent();

            var userToken = GenerateJwtToken(user, role);
            _userClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userToken);

            // act

            var response = await _userClient.PutAsync($"{_route}", httpContent);
            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task Update_WithoutuserToken_ReturnsUnauthorized()
        {
            // arrange
            var role = new UserRole()
            {
                RoleName = "User"
            };
            await SeedRole(role);

            var user = new User()
            {
                NickName = "JDoe",
                UserDetails = new UserDetails()
                {
                    Email = "test@email.com",
                    Country = "USA",
                    FirstName = "John",
                    LastName = "Doe"
                },

                PasswordHash = "validPassword",
                UserRoleId = role.Id,
            };
            await SeedUser(user);

            var command = new UpdateUserDetailsCommand()
            {
                FirstName = "John",
                LastName = "Doe",
                Country = "USA"
            };
            var httpContent = command.ToJsonHttpContent();

            // act

            var response = await _client.PutAsync($"{_route}", httpContent);
            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        }

        private string GenerateJwtToken(User user, UserRole role)
        {
            // arrange

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email,"test@email.com"),
                new Claim(ClaimTypes.Name, "John Doe"),
                new Claim(ClaimTypes.Role, role.RoleName)
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

        private async Task SeedRole(UserRole role)
        {
            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();
            var _dbContext = scope.ServiceProvider.GetService<TravelDiaryDbContext>();
            if (!_dbContext.Roles.Contains(role))
            {
                _dbContext.Roles.Add(role);
                await _dbContext.SaveChangesAsync();
            }
        }

        private async Task SeedUser(User user)
        {
            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();
            var _dbContext = scope.ServiceProvider.GetService<TravelDiaryDbContext>();
            if (!_dbContext.Users.Contains(user))
            {
                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}