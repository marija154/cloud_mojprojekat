using MediatR;
using Microsoft.Extensions.Logging;
using SmartGrid.Application.Common;
using SmartGrid.Domain.Enums;
using SmartGrid.Domain.Events;

namespace SmartGrid.Application.Features.Alerts.EventHandlers
{
    internal class LogAlertEventHandler(
        ILogger<LogAlertEventHandler> logger) : INotificationHandler<DomainEventNotification<AnomalyDetectedDomainEvent>>
    {
        public async Task Handle(DomainEventNotification<AnomalyDetectedDomainEvent> notification, CancellationToken ct)
        {
            var alert = notification.Event.Alert;

            string logMsg = $"[ALARM] {alert.AlertType} on {alert.DeviceId}: {alert.Message}";

            await Task.CompletedTask; // Simulate async work

            if (alert.AlertType == AlertType.Critical)
                logger.LogError(logMsg);
            else
                logger.LogWarning(logMsg);
        }
    }
}
