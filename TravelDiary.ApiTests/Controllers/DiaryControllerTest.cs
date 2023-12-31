﻿using FluentAssertions;
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
using TravelDiary.Application.DiaryService.Commands.CreateDiary;
using TravelDiary.Domain.Entities;
using TravelDiary.Domain.Models;
using TravelDiary.Infrastructure.Persistence;

namespace TravelDiary.ApiTests.Controllers
{
    public class DiaryControllerTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private const string _route = "Api/Diary";
        private readonly AuthenticationSettings _authenticationSettings;
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;
        private readonly WebApplicationFactory<Program> _factory;
        private HttpClient _userClient;

        public DiaryControllerTest(WebApplicationFactory<Program> factory)
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

        [Theory]
        [InlineData(null, "ValidDescription", "2023 - 08 - 11T12: 00:00", PrivacyPolicy.Public)] // Null name
        [InlineData("", "ValidDescription", "2023 - 08 - 11T12: 00:00", PrivacyPolicy.Public)] // Empty name
        [InlineData("ValidName", null, "2023 - 08 - 11T12: 00:00", PrivacyPolicy.Public)] // Null description
        [InlineData("ValidName", "", "2023 - 08 - 11T12: 00:00", PrivacyPolicy.Public)] // Empty description
        [InlineData("N", "ValidDescription", "2023 - 08 - 11T12: 00:00", PrivacyPolicy.Public)] // Name To short
        [InlineData("VeryLongNameThatExceedsMaxLengthVeryLongNameThatExceedsMaxLengthVeryLongNameThatExceedsMaxLength", "ValidDescription", "2023 - 08 - 11T12: 00:00", PrivacyPolicy.Public)] // Name to long
        [InlineData("ValidName", "VeryLongDescriptionThatExceedsMaxLengthVeryLongDescriptionThatExceedsMaxLengthVeryLongDescriptionThatExceedsMaxLengthVeryLongDescriptionThatExceedsMaxLengthVeryLongDescriptionThatExceedsMaxLengthVeryLongDescriptionThatExceedsMaxLengthVeryLongDescriptionThatExceedsMaxLengthVeryLongDescriptionThatExceedsMaxLengthVeryLongDescriptionThatExceedsMaxLengthVeryLongDescriptionThatExceedsMaxLengthVeryLongDescriptionThatExceedsMaxLengthVeryLongDescriptionThatExceedsMaxLengthVeryLongDescriptionThatExceedsMaxLengthVeryLongDescriptionThatExceedsMaxLengthVeryLongDescriptionThatExceedsMaxLengthVeryLongDescriptionThatExceedsMaxLengthVeryLongDescriptionThatExceedsMaxLengthVeryLongDescriptionThatExceedsMaxLengthVeryLongDescriptionThatExceedsMaxLengthVeryLongDescriptionThatExceedsMaxLengthVeryLongDescriptionThatExceedsMaxLengthVeryLongDescriptionThatExceedsMaxLengthVeryLongDescriptionThatExceedsMaxLengthVeryLongDescriptionThatExceedsMaxLengthVeryLongDescriptionThatExceedsMaxLengthVeryLongDescriptionThatExceedsMaxLengthVeryLongDescriptionThatExceedsMaxLengthVeryLongDescriptionThatExceedsMaxLength", "2023 - 08 - 11T12: 00:00", PrivacyPolicy.Public)] // Description To long
        public async Task Create_ForInvalidParams_ReturnsBadRequest(string name, string description, DateTime starts, PrivacyPolicy privacyPolicy)
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
            var command = new CreateDiaryCommand()
            {
                Description = description,
                Name = name,
                Starts = starts,
                Policy = privacyPolicy
            };
            var httpContent = command.ToJsonHttpContent();
            //act

            var response = await _userClient.PostAsync($"{_route}", httpContent);
            //assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
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
            var command = new CreateDiaryCommand()
            {
                Description = "Test Description",
                Name = "TestName",
                Starts = new DateTime(2008, 5, 1, 8, 30, 0),
                Policy = PrivacyPolicy.Public
            };
            var httpContent = command.ToJsonHttpContent();
            //act

            var response = await _userClient.PostAsync($"{_route}", httpContent);
            //assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
        }

        [Fact]
        public async Task Delete_ForInvalidId_ReturnsNotFound()
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
            //act

            var response = await _userClient.DeleteAsync($"{_route}/34563");
            //assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Delete_ForNotDiaryOwner_ReturnsForbidden()
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
            var invaliduser = new User()
            {
                NickName = "FakeJDoe",
                UserDetails = new UserDetails()
                {
                    Email = "Faketest@email.com",
                    Country = "USA",
                    FirstName = "John",
                    LastName = "Doe"
                },

                PasswordHash = "validPassword",
                UserRoleId = role.Id,
            };
            await SeedUser(invaliduser);
            await PrepareUserClient(invaliduser, role);
            var diary = new Diary()
            {
                CreatedById = user.Id,
                Description = "Description",
                Name = "Name",
                Starts = new DateTime(2008, 5, 1, 8, 30, 0),
                Ends = new DateTime(2009, 5, 1, 8, 30, 0)
            };
            await SeedDiary(diary);
            //act

            var response = await _userClient.DeleteAsync($"{_route}/{diary.Id}");
            //assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Delete_ForValidId_ReturnsNoContent()
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
            //act

            var response = await _userClient.DeleteAsync($"{_route}/{diary.Id}");
            //assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
        }

        [Theory]
        [InlineData(-1, 10, "sample", "Name", SortDirection.ASC)] // Negative page number
        [InlineData(1, 7, "sample", "Name", SortDirection.ASC)]  // Invalid page size
        [InlineData(1, 10, "sample", "invalidColumn", SortDirection.ASC)]  // Invalid sorting column
        [InlineData(0, -5, null, "title", SortDirection.DESC)]  // Combination of negative values
        [InlineData(1, -10, "sample", "date", SortDirection.ASC)] // Negative page size
        public async Task GetDiaries_ForInvalidQueryParams_ReturnsBadRequest(int pageNumber, int pageSize, string searchPhrase, string sortBy, SortDirection sortDirection)
        {
            //arrange

            var query = $"PageNumber={pageNumber}&PageSize={pageSize}&SearchPhrase={searchPhrase}&SortBy={sortBy}&SortDirection={sortDirection}";
            //act

            var response = await _client.GetAsync($"{_route}?{query}");
            //assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetDiaries_ForValidQueryParams_ReturnsOk()
        {
            //arrange

            var query = "PageNumber=1&PageSize=5&SearchPhrase=phrase&SortBy=Starts&SortDirection=0";
            //act

            var response = await _client.GetAsync($"{_route}?{query}");
            //assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetDiary_ForValidId_ReturnsOk()
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

            var diary = new Diary()
            {
                CreatedById = user.Id,
                Description = "Description",
                Name = "Name",
                Starts = new DateTime(2008, 5, 1, 8, 30, 0),
                Ends = new DateTime(2009, 5, 1, 8, 30, 0),
                Policy = PrivacyPolicy.Public,
            };
            await SeedDiary(diary);
            // act

            var response = await _client.GetAsync($"{_route}/{diary.Id}");
            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetDiaryById_ForInvalidId_ReturnsNotFound()
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

            var diary = new Diary()
            {
                CreatedById = user.Id,
                Description = "Description",
                Name = "Name",
                Starts = new DateTime(2008, 5, 1, 8, 30, 0),
                Ends = new DateTime(2009, 5, 1, 8, 30, 0),
                Policy = PrivacyPolicy.Public,
            };
            await SeedDiary(diary);
            // act

            var response = await _client.GetAsync($"{_route}/3456");
            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetDiaryById_ForNotDiaryOwner_ReturnsForbidden()
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

            var diary = new Diary()
            {
                CreatedById = user.Id,
                Description = "Description",
                Name = "Name",
                Starts = new DateTime(2008, 5, 1, 8, 30, 0),
                Ends = new DateTime(2009, 5, 1, 8, 30, 0),
                Policy = PrivacyPolicy.Private,
            };
            await SeedDiary(diary);
            var invaliduser = new User()
            {
                NickName = "FakeJDoe",
                UserDetails = new UserDetails()
                {
                    Email = "Faketest@email.com",
                    Country = "USA",
                    FirstName = "John",
                    LastName = "Doe"
                },

                PasswordHash = "validPassword",
                UserRoleId = role.Id,
            };
            await SeedUser(invaliduser);
            await PrepareUserClient(invaliduser, role);
            // act

            var response = await _userClient.GetAsync($"{_route}/{diary.Id}");
            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Update_ForNotDiaryOwner_ReturnsForbidden()
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
            var invaliduser = new User()
            {
                NickName = "FakeJDoe",
                UserDetails = new UserDetails()
                {
                    Email = "Faketest@email.com",
                    Country = "USA",
                    FirstName = "John",
                    LastName = "Doe"
                },

                PasswordHash = "validPassword",
                UserRoleId = role.Id,
            };
            await SeedUser(invaliduser);
            await PrepareUserClient(invaliduser, role);
            var diary = new Diary()
            {
                CreatedById = user.Id,
                Description = "Description",
                Name = "Name",
                Starts = new DateTime(2008, 5, 1, 8, 30, 0),
                Ends = new DateTime(2009, 5, 1, 8, 30, 0)
            };
            await SeedDiary(diary);
            var description = "NewDescription";

            var httpContent = description.ToJsonHttpContent();
            //act

            var response = await _userClient.PutAsync($"{_route}/{diary.Id}/Description", httpContent);
            //assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task UpdateDescription_ForInvalidIdAndDescription_ReturnsNotFound()
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

            var description = "NewDescription";

            var httpContent = description.ToJsonHttpContent();
            //act

            var response = await _userClient.PutAsync($"{_route}/4567/Description", httpContent);
            //assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdateDescription_ForValidIdAndDescription_ReturnsOK()
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

            var description = "NewDescription";

            var httpContent = description.ToJsonHttpContent();
            //act

            var response = await _userClient.PutAsync($"{_route}/{diary.Id}/Description", httpContent);
            //assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Theory]
        [InlineData("")] // Empty description
        [InlineData(null)] // Null description
        [InlineData("VeryLongDescriptionThatExceedsMaxLengthVeryLongDescriptionThatExceedsMaxLengthVeryLongDescriptionThatExceedsMaxLengthVeryLongDescriptionThatExceedsMaxLengthVeryLongDescriptionThatExceedsMaxLengthVeryLongDescriptionThatExceedsMaxLengthVeryLongDescriptionThatExceedsMaxLengthVeryLongDescriptionThatExceedsMaxLengthVeryLongDescriptionThatExceedsMaxLengthVeryLongDescriptionThatExceedsMaxLengthVeryLongDescriptionThatExceedsMaxLengthVeryLongDescriptionThatExceedsMaxLengthVeryLongDescriptionThatExceedsMaxLengthVeryLongDescriptionThatExceedsMaxLengthVeryLongDescriptionThatExceedsMaxLengthVeryLongDescriptionThatExceedsMaxLengthVeryLongDescriptionThatExceedsMaxLengthVeryLongDescriptionThatExceedsMaxLengthVeryLongDescriptionThatExceedsMaxLengthVeryLongDescriptionThatExceedsMaxLengthVeryLongDescriptionThatExceedsMaxLengthVeryLongDescriptionThatExceedsMaxLengthVeryLongDescriptionThatExceedsMaxLengthVeryLongDescriptionThatExceedsMaxLengthVeryLongDescriptionThatExceedsMaxLengthVeryLongDescriptionThatExceedsMaxLengthVeryLongDescriptionThatExceedsMaxLengthVeryLongDescriptionThatExceedsMaxLength")]// To Long description
        public async Task UpdateDescription_ForValidIdAndInvalidDescription_ReturnsBadRequest(string description)
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

            var Description = description;

            var httpContent = Description.ToJsonHttpContent();
            //act

            var response = await _userClient.PutAsync($"{_route}/{diary.Id}/Description", httpContent);
            //assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
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

        private async Task PrepareUserClient(User user, UserRole role)
        {
            var userToken = GenerateJwtToken(user, role);
            _userClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userToken);
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