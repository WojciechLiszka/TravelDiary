using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TravelDiary.ApiTests.Helpers;
using TravelDiary.Application.AccountService.Commands.LoginUserAccountCommand;
using TravelDiary.Application.AccountService.Commands.RegisterUserAccountCommand;
using TravelDiary.Application.AccountService.Commands.UpdateUserAccountDetailsCommand;
using TravelDiary.Domain.Entities;
using TravelDiary.Domain.Interfaces;
using TravelDiary.Infrastructure.Persistence;

namespace TravelDiary.ApiTests.Controllers
{
    public class AccountControllerTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private const string _route = "Api/Account";
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;
        private readonly Mock<IUserRoleRepository> _userRoleRepositoryMock = new Mock<IUserRoleRepository>();
        private readonly Mock<IPasswordHasher<User>> _passwordHasherMock = new Mock<IPasswordHasher<User>>();

        public AccountControllerTest(WebApplicationFactory<Program> factory)
        {
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
                        .EnableSensitiveDataLogging())
                        ;
                   });
               });

            _client = _factory.CreateClient();
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
        public async Task LoginUser_ForvalidParams_ReturnsOK()
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
                UserRoleId =role.Id,
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

        public async Task Update_ForValidParams_ReturnsOK()
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
            var command = new UpdateUserDetailsCommand()
            {
                FirstName = "John",
                LastName="Doe",
                Country="USA"
            };
            var httpContent = command.ToJsonHttpContent();
            // act

            var response = await _client.PostAsync($"{_route}/Login", httpContent);
            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }
    }
}