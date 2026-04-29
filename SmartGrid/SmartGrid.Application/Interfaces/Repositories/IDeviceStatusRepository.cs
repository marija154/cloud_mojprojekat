using SmartGrid.Domain.Models;
using SmartGrid.Domain.ValueObjects;

namespace SmartGrid.Application.Interfaces.Repositories
{
    public interface IDeviceStatusRepository
    {
        void Save(DeviceStatus status);
        IEnumerable<DeviceStatus?> GetAll();
        DeviceStatus? GetById(EntityId deviceId);
    }
}
