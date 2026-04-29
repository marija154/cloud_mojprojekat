using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SmartGrid.Application.Features.Telemetries.Events;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;
using SmartGrid.Domain.Models;

namespace SmartGrid.Application.Features.Telemetries.Commands
{
    // COMMAND
    public record ProcessTelemetryCommand : IRequest<Result>
    {
        public string DeviceId { get; set; } = string.Empty;
        public string DeviceName { get; init; } = string.Empty;
        public double NominalPower { get; init; }
        public double CurrentPower { get; init; }
        public DateTime Timestamp { get; init; }
    }

    // VALIDATOR
    public class ProcessTelemetryValidator : AbstractValidator<ProcessTelemetryCommand>
    {
        public ProcessTelemetryValidator()
        {
            RuleFor(t => t.DeviceId)
                .NotEmpty().WithMessage("DeviceId is required.");

            RuleFor(t => t.DeviceName)
                .NotEmpty().WithMessage("DeviceName is required.");

            RuleFor(t => t.NominalPower)
                .NotEmpty().WithMessage("NominalPower is missing from payload.")
                .GreaterThan(0).WithMessage("NominalPower must be greater than zero.");

            RuleFor(t => t.CurrentPower)
                .NotEmpty().WithMessage("CurrentPower is missing from payload.")
                .GreaterThanOrEqualTo(0);

            RuleFor(t => t.Timestamp)
                .NotEmpty().WithMessage("Timestamp is required.")
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Timestamp cannot be in the future.");
        }
    }

    // HANDLER
    public class ProcessTelemetryHandler(
        IMediator mediator,
        ILogger<ProcessTelemetryHandler> logger) : IRequestHandler<ProcessTelemetryCommand, Result>
    {
        public async Task<Result> Handle(ProcessTelemetryCommand request, CancellationToken ct)
        {
            var telemetryResult = Telemetry.Create(
                request.DeviceId,
                request.DeviceName,
                request.NominalPower,
                request.CurrentPower,
                request.Timestamp
            );

            if (telemetryResult.IsFailure)
                return Result.Failure(telemetryResult.Error!.Message, ErrorType.Validation);

            var telemetry = telemetryResult.Value;

            await mediator.Publish(new TelemetryProcessedEvent(telemetry), ct);

            logger.LogInformation("[TELEMETRY] Event published.");

            return Result.Success();
        }
    }
}
