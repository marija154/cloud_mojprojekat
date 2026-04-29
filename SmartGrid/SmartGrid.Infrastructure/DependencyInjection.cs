using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartGrid.Application.Common.Options;
using SmartGrid.Infrastructure.Common.Options;
using SmartGrid.Infrastructure.Extensions;

namespace SmartGrid.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<AzureTableOptions>(configuration.GetSection("AzureTableOptions"));
            services.Configure<ParallelSettings>(configuration.GetSection("ParallelSettings"));

            var tableConn = configuration.GetValue<string>("AzureTableOptions:ConnectionString")
                ?? throw new InvalidOperationException("AzureTableOptions:ConnectionString is not configured.");

            services
                .AddServices()
                .AddAzureTables(tableConn);

            return services;
        }

    }
}
