using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using TravelDiary.ApiTests.Helpers;
using TravelDiary.Domain.Dtos;
using TravelDiary.Domain.Entities;
using TravelDiary.Domain.Models;
using TravelDiary.Infrastructure.Persistence;

namespace TravelDiary.ApiTests.Controllers
{
    public class EntryControllerTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private const string _route = "/Api";
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;
        private readonly AuthenticationSettings _authenticationSettings;

        private readonly IConfiguration _configuration;
        private HttpClient _userClient;

        public EntryControllerTest(WebApplicationFactory<Program> factory)
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
                       services
                        .AddDbContext<TravelDiaryDbContext>(options => options.UseInMemoryDatabase("TravelDiaryDb")
                        .EnableSensitiveDataLogging());
                   });
               });

            _client = _factory.CreateClient();
            _userClient = _factory.CreateClient();
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

        private async Task SeedDiary(Diary diary)
        {
            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();
            var _dbContext = scope.ServiceProvider.GetService<TravelDiaryDbContext>();
            if (!_dbContext.Diaries.Contains(diary))
            {
                _dbContext.Diaries.Add(diary);
                await _dbContext.SaveChangesAsync();
            }
        }

        private async Task PrepareUserClient(User user, UserRole role)
        {
            var userToken = GenerateJwtToken(user, role);
            _userClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userToken);
        }

        [Fact]
        public async Task Create_ForValidParams_ReturnsCreated()
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
            await PrepareUserClient(user, role);
            var diary = new Diary()
            {
                CreatedById = user.Id,
                Description = "Description",
                Name = "Name",
                Starts = new DateTime(2008, 5, 1, 8, 30, 0),
                Ends = new DateTime(2009, 5, 1, 8, 30, 0)
            };
            await SeedDiary(diary);

            var dto = new CreateEntryDto()
            {
                Tittle = "TestTittle",
                Description = "Description",
                Date = new DateTime(2008, 5, 1, 8, 30, 0)
            };
            var httpContent = dto.ToJsonHttpContent();

            // act

            var response = await _userClient.PostAsync($"{_route}/Diary/{diary.Id}/Entry", httpContent);

            //assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
        }

        [Theory]
        [InlineData("", "description", "2023 - 08 - 11T12: 00:00")] // Empty tittle
        [InlineData(null, "description", "2023 - 08 - 11T12: 00:00")] // Null tittle
        [InlineData("abc", "description", "2023 - 08 - 11T12: 00:00")] // Tittle to short
        [InlineData("Tittle", "description", null)] // Empty date
        public async Task Create_ForInvalidParams_ReturnsBadRequest(string title, string description, DateTime date)
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
            await PrepareUserClient(user, role);
            var diary = new Diary()
            {
                CreatedById = user.Id,
                Description = "Description",
                Name = "Name",
                Starts = new DateTime(2008, 5, 1, 8, 30, 0),
                Ends = new DateTime(2009, 5, 1, 8, 30, 0)
            };
            await SeedDiary(diary);

            var dto = new CreateEntryDto()
            {
                Tittle = title,
                Description = description,
                Date = date
            };
            var httpContent = dto.ToJsonHttpContent();

            // act

            var response = await _userClient.PostAsync($"{_route}/Diary/{diary.Id}/Entry", httpContent);

            //assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Create_ForNotDiaryOwner_ReturnsForbidden()
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

            var invalidUser = new User()
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

            await SeedUser(invalidUser);

            await PrepareUserClient(invalidUser, role);

            var diary = new Diary()
            {
                CreatedById = user.Id,
                Description = "Description",
                Name = "Name",
                Starts = new DateTime(2008, 5, 1, 8, 30, 0),
                Ends = new DateTime(2009, 5, 1, 8, 30, 0)
            };
            await SeedDiary(diary);

            var dto = new CreateEntryDto()
            {
                Tittle = "TestTittle",
                Description = "Description",
                Date = new DateTime(2008, 5, 1, 8, 30, 0)
            };
            var httpContent = dto.ToJsonHttpContent();

            // act

            var response = await _userClient.PostAsync($"{_route}/Diary/{diary.Id}/Entry", httpContent);

            //assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);
        }
    }
}