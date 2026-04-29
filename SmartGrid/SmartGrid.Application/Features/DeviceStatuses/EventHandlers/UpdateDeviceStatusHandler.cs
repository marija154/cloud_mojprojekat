using MediatR;
using Microsoft.Extensions.Logging;
using SmartGrid.Application.Features.Telemetries.Events;
using SmartGrid.Application.Interfaces;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Models;

namespace SmartGrid.Application.Features.DeviceStatuses.EventHandlers;

public class UpdateDeviceStatusHandler(
        IDeviceStatusRepository deviceStatusRepository,
        IDateTimeProvider dateTimeProvider,
        ILogger<UpdateDeviceStatusHandler> logger) : INotificationHandler<TelemetryProcessedEvent>
{

    public async Task Handle(TelemetryProcessedEvent notification, CancellationToken cancellationToken)
    {
        var telemetry = notification.Telemetry;

        try
        {
            var deviceStatus = deviceStatusRepository.GetById(telemetry.DeviceId);

            if (deviceStatus is null)
            {
                deviceStatus = DeviceStatus.CreateDefault(telemetry.DeviceId, dateTimeProvider.UtcNow);
            }

            deviceStatus.UpdateTelemetry(telemetry);

            deviceStatusRepository.Save(deviceStatus);

            await Task.CompletedTask; // Simulate async work

            logger.LogInformation("Successfully updated status for device {DeviceId}.", telemetry.DeviceId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error asynchronously updating status for device {DeviceId}", telemetry.DeviceId);
        }
    }
}