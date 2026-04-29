using Microsoft.Extensions.DependencyInjection;
using SmartGrid.Application.Interfaces;
using SmartGrid.Infrastructure.Services;

namespace SmartGrid.Infrastructure.Extensions
{
    internal static class ServiceExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

            return services;
        }
    }
}
