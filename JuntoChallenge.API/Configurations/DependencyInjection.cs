using JuntoChallenge.Application.Interfaces;
using JuntoChallenge.Application.Services;

namespace JuntoChallenge.API.Configurations
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();

            services.AddScoped<ILogService, LogService>();

            return services;
        }
    }
}
