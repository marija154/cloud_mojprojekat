using Microsoft.Extensions.DependencyInjection;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Infrastructure.Extensions;
using SmartGrid.Infrastructure.Persistence.InMemory.Repositories;

namespace SmartGrid.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddServices();

            services.AddSingleton<IDeviceStatusRepository, InMemoryDeviceRepository>();

            return services;
        }

    }
}
