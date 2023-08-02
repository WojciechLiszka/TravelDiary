using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TravelDiary.ApiTests.Helpers;
using TravelDiary.Application.AccountService.Commands.RegisterUserAccountCommand;
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

                       services.AddSingleton<IUserRoleRepository>(_userRoleRepositoryMock.Object);
                       services
                        .AddDbContext<TravelDiaryDbContext>(options => options.UseInMemoryDatabase("TravelDiaryDb")
                        .EnableSensitiveDataLogging())
                        ;
                   });
               });

            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task RegisterUser_ForValidDtoParams_ReturnsOk()
        {
            // arrange
            _userRoleRepositoryMock
                .Setup(e => e.GetByName(It.IsAny<string>()))
                .ReturnsAsync(new UserRole()
                {
                    Id = 1,
                    RoleName = "User",
                })
                ;

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
            _userRoleRepositoryMock
                .Setup(e => e.GetByName(It.IsAny<string>()))
                .ReturnsAsync(new UserRole()
                {
                    Id = 1,
                    RoleName = "User",
                })
                ;

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
    }
}