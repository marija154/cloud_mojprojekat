using MediatR;
using Microsoft.Extensions.Logging;
using SmartGrid.Application.Interfaces;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;

namespace SmartGrid.Application.Features.DeviceStatuses.Commands;

// COMMAND - Empty
public record ExecuteMonitoringCommand : IRequest<Result>;

// HANDLER
public class ExecuteMonitoringHandler(
    IDeviceStatusRepository deviceStatusRepository,
    IDateTimeProvider dateTimeProvider,
    ILogger<ExecuteMonitoringHandler> logger
    ) : IRequestHandler<ExecuteMonitoringCommand, Result>
{
    public async Task<Result> Handle(ExecuteMonitoringCommand request, CancellationToken ct)
    {
        try
        {
            var deviceStatuses = deviceStatusRepository.GetAll();

            foreach (var statuses in deviceStatuses)
            {
                if (statuses is not null)
                {
                    var alert = statuses.GetCurrentAnomaly(dateTimeProvider.UtcNow);

                    if (alert is not null)
                    {
                        string logMsg = $"[ALARM] {alert.AlertType} on {alert.DeviceId}: {alert.Message}";

                        if (alert.AlertType == AlertType.Critical)
                             logger.LogError(logMsg);
                         else
                             logger.LogWarning(logMsg);
                    }
                }
            }

            await Task.CompletedTask; // Simulate async work

            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Monitoring cycle failed due to an unexpected error.");
            return Result.Failure("Critical monitoring failure.", ErrorType.Failure);
        }
    }
}