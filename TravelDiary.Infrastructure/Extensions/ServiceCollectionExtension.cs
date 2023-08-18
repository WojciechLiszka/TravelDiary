using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TravelDiary.Domain.Interfaces;
using TravelDiary.Infrastructure.Persistence;
using TravelDiary.Infrastructure.Repositories;
using TravelDiary.Infrastructure.Seeders;

using TravelDiary.Infrastructure.Services;

namespace TravelDiary.Infrastructure.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<TravelDiaryDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("TravelDiary")));

            services.AddScoped<IDiaryRepository, DiaryRepository>();

            services.AddScoped<IAccountRepository, AccountRepository>();

            services.AddScoped<IUserRoleRepository, UserRoleRepository>();

            services.AddScoped<IEntryRepository, EntryRepository>();

            services.AddScoped<RoleSeeder>();

            services.AddScoped<IUserContextService, UserContextService>();
        }
    }
}