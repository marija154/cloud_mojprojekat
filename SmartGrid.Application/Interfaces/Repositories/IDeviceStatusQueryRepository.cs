using SmartGrid.Domain.Enums;
using SmartGrid.Domain.Models;

namespace SmartGrid.Application.Interfaces.Repositories
{
    public interface IDeviceStatusQueryRepository
    {
        IAsyncEnumerable<DeviceStatus> GetByTypeStreamingAsync(DeviceType type, CancellationToken ct = default);
    }
}
