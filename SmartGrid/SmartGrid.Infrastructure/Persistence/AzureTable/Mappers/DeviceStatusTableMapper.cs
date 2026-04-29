using SmartGrid.Domain.Enums;
using SmartGrid.Domain.Models;
using SmartGrid.Infrastructure.Persistence.AzureTable.Common;
using SmartGrid.Infrastructure.Persistence.AzureTable.Entities;

namespace SmartGrid.Infrastructure.Persistence.AzureTable.Mappers
{
    internal sealed class DeviceStatusTableMapper : ITableMapper<DeviceStatus, DeviceStatusEntity>
    {
        public DeviceStatusEntity ToEntity(DeviceStatus domain)
        {
            return new DeviceStatusEntity
            {
                CurrentPower = domain.CurrentPower.Value,
                LoadPercentage = domain.LoadPercentage,
                LastHeartbeat = domain.LastHeartbeat,
            };
        }
        public DeviceStatus? ToDomain(DeviceStatusEntity entity)
        {
            var deviceType = Enum.TryParse<DeviceType>(entity.PartitionKey, out var parsedType)
                             ? parsedType : DeviceType.Unknown;

            var deviceStatusResult = DeviceStatus.Load(
                entity.RowKey,
                deviceType,
                entity.CurrentPower,
                entity.LoadPercentage,
                entity.LastHeartbeat
            );

            if (deviceStatusResult.IsFailure)
                return null;

            return deviceStatusResult.Value;
        }
    }
}