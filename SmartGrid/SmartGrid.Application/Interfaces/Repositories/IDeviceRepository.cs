using SmartGrid.Domain.Enums;
using SmartGrid.Domain.Models;
using SmartGrid.Domain.ValueObjects;

namespace SmartGrid.Application.Interfaces.Repositories
{
    public interface IDeviceRepository
    {
        Task<Device?> GetWithStatusByIdAsync(DeviceType deviceType, EntityId deviceId, CancellationToken ct = default);
        IAsyncEnumerable<Device> GetAllWithStatusStreamingAsync(CancellationToken ct = default);
        Task SaveAsync(Device device, CancellationToken ct = default);
        Task SaveStatusAsync(DeviceStatus status, CancellationToken ct = default);
    }
}
