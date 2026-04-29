using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;
using SmartGrid.Domain.ValueObjects;

namespace SmartGrid.Domain.Models
{
    public class Telemetry
    {
        public EntityId Id { get; private set; }
        public EntityId DeviceId { get; private set; }
        public string DeviceName { get; private set; } = string.Empty;
        public Power NominalPower { get; private set; }
        public Power CurrentPower { get; private set; }
        public DateTime Timestamp { get; private set; }
        public Percentage LoadPercentage { get; private set; }
        private Telemetry(
            EntityId id,
            EntityId deviceId,
            string deviceName,
            Power nominalPower,
            Power currentPower, 
            DateTime timestamp)
        {
            Id = id;
            DeviceId = deviceId;
            DeviceName = deviceName;
            NominalPower = nominalPower;
            CurrentPower = currentPower;
            Timestamp = timestamp;
            LoadPercentage = nominalPower.Value > 0
                ? Percentage.FromRaw(Math.Round((currentPower.Value / nominalPower.Value) * 100, 2))
                : Percentage.Zero();
        }

        #region Factory Method

        public static Result<Telemetry> Create(
            string deviceId,
            string deviceName,
            double nominalPower,
            double currentPower,
            DateTime timestamp)
        {
            if (string.IsNullOrWhiteSpace(deviceName))
                return Result<Telemetry>.Failure("DeviceName is required.",
                    ErrorType.Validation);

            if (timestamp > DateTime.UtcNow)
                return Result<Telemetry>.Failure("Timestamp cannot be in the future.",
                    ErrorType.Validation);

            var idResult = EntityId.Create(deviceId);

            if (idResult.IsFailure)
                return Result<Telemetry>.Failure(idResult.Error!.Message, ErrorType.Validation);

            var nominalPowerResult = Power.Create(nominalPower);
            if (nominalPowerResult.IsFailure)
                return Result<Telemetry>.Failure(nominalPowerResult.Error!.Message, ErrorType.Validation);

            var currentPowerResult = Power.Create(currentPower);
            if (currentPowerResult.IsFailure)
                return Result<Telemetry>.Failure(currentPowerResult.Error!.Message, ErrorType.Validation);

            return Result<Telemetry>.Success(new Telemetry(
                EntityId.New(),
                idResult.Value,
                deviceName,
                nominalPowerResult.Value,
                currentPowerResult.Value,
                timestamp
            ));
        }
        public static Result<Telemetry> Load(
            string id,
            string deviceId,
            string deviceName,
            double nominalPower,
            double currentPower,
            DateTime timestamp)
        {
            var idResult = EntityId.Create(id);
            if (idResult.IsFailure)
                return Result<Telemetry>.Failure(idResult.Error!.Message, ErrorType.Validation);

            var deviceIdResult = EntityId.Create(deviceId);
            if (deviceIdResult.IsFailure)
                return Result<Telemetry>.Failure(deviceIdResult.Error!.Message, ErrorType.Validation);

            var nominalPowerResult = Power.Create(nominalPower);
            if (nominalPowerResult.IsFailure)
                return Result<Telemetry>.Failure(nominalPowerResult.Error!.Message, ErrorType.Validation);

            var currentPowerResult = Power.Create(currentPower);
            if (currentPowerResult.IsFailure)
                return Result<Telemetry>.Failure(currentPowerResult.Error!.Message, ErrorType.Validation);

            var telemetry = new Telemetry(
               idResult.Value,
               deviceIdResult.Value,
               deviceName,
               nominalPowerResult.Value,
               currentPowerResult.Value,
               timestamp
            );

            return Result<Telemetry>.Success(telemetry);
        }

        #endregion
    }
}
