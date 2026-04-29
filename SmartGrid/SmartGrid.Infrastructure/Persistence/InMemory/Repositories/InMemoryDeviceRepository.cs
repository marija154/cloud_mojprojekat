using System.Collections.Concurrent;
using SmartGrid.Domain.Models;
using SmartGrid.Infrastructure.Persistence.InMemory.Entities;
using SmartGrid.Domain.ValueObjects;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Infrastructure.Persistence.InMemory.Mappers;

namespace SmartGrid.Infrastructure.Persistence.InMemory.Repositories
{
    public class InMemoryDeviceRepository : IDeviceStatusRepository
    {
        private readonly ConcurrentDictionary<string, DeviceStatusEntity> _deviceStatusEntities = new();
        public InMemoryDeviceRepository() { }
        public IEnumerable<DeviceStatus?> GetAll()
        {
            return _deviceStatusEntities.Values.Select(DeviceStatusMapper.ToDomain);
        }
        public DeviceStatus? GetById(EntityId deviceId)
        {
            if (_deviceStatusEntities.TryGetValue(deviceId.Value, out var entity))
            {
                return DeviceStatusMapper.ToDomain(entity);
            }
            return null;
        }
        public void Save(DeviceStatus status)
        {
            var entity = DeviceStatusMapper.ToEntity(status);
            if (entity == null) return;

            _deviceStatusEntities.AddOrUpdate(
                status.DeviceId.Value,
                _ => entity,
                (_, _) => entity
            );
        }
    }
}