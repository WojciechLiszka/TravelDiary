using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TravelDiary.Infrastructure.Persistence;

namespace TravelDiary.Infrastructure.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<TravelDiaryDBContext>(options => options.UseSqlServer(configuration.GetConnectionString("TravelDiary")));
        }
    }
}