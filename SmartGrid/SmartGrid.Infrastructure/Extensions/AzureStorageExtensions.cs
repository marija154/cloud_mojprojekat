using Azure.Data.Tables;
using Microsoft.Extensions.DependencyInjection;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Models;
using SmartGrid.Infrastructure.Persistence.AzureTable.Common;
using SmartGrid.Infrastructure.Persistence.AzureTable.Entities;
using SmartGrid.Infrastructure.Persistence.AzureTable.KeyProviders;
using SmartGrid.Infrastructure.Persistence.AzureTable.Mappers;
using SmartGrid.Infrastructure.Persistence.AzureTable.Repositories;

namespace SmartGrid.Infrastructure.Extensions
{
    internal static class AzureStorageExtensions
    {
        public static IServiceCollection AddAzureTables(
            this IServiceCollection services,
            string connectionString)
        {
            // Table Service Client
            services.AddSingleton(new TableServiceClient(connectionString));

            // Mappers
            services.AddSingleton<ITableMapper<Telemetry, TelemetryEntity>, TelemetryTableMapper>();
            services.AddSingleton<ITableMapper<Device, DeviceEntity>, DeviceTableMapper>();
            services.AddSingleton<ITableMapper<DeviceStatus, DeviceStatusEntity>, DeviceStatusTableMapper>();

            // Key Providers
            services.AddSingleton<ITableKeyProvider<Telemetry>, TelemetryTableKeyProvider>();
            services.AddSingleton<ITableKeyProvider<Device>, DeviceTableKeyProvider>();
            services.AddSingleton<ITableKeyProvider<DeviceStatus>, DeviceStatusTableKeyProvider>();

            // Repositories
            services.AddScoped<ITelemetryRepository, TelemetryRepository>();
            services.AddScoped<IDeviceRepository, DeviceRepository>();

            return services;
        }
    }
}
