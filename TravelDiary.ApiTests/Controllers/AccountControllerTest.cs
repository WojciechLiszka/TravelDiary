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
                .Setup(e => e.GetByName(It.IsAny<String>()))
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
    }
}