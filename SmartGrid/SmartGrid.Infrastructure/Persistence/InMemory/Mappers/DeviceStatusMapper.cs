using SmartGrid.Domain.Models;
using SmartGrid.Infrastructure.Persistence.InMemory.Entities;

namespace SmartGrid.Infrastructure.Persistence.InMemory.Mappers
{
    internal static class DeviceStatusMapper
    {
        public static DeviceStatusEntity ToEntity(DeviceStatus domain)
        {
            return new DeviceStatusEntity
            {
                DeviceId = domain.DeviceId,
                CurrentPower = domain.CurrentPower.Value,
                LoadPercentage = domain.LoadPercentage,
                LastHeartbeat = domain.LastHeartbeat,
            };
        }
        public static DeviceStatus? ToDomain(DeviceStatusEntity entity)
        {
            var deviceStatusResult = DeviceStatus.Load(
                entity.DeviceId,
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
