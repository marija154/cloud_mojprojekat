using SmartGrid.Domain.Common;
using SmartGrid.Domain.Constants;
using SmartGrid.Domain.Enums;
using SmartGrid.Domain.ValueObjects;

namespace SmartGrid.Domain.Models
{
    public class DeviceStatus
    {
        public EntityId DeviceId { get; private set; }
        // SNAPSHOT
        public Power CurrentPower { get; private set; }
        public Percentage LoadPercentage { get; private set; }
        public DateTime LastHeartbeat { get; private set; }
        // STATUS
        public bool IsOnline(DateTime currentTime) => currentTime - LastHeartbeat < DeviceStatusLimits.OfflineThreshold;
        public bool IsUnderperforming => LoadPercentage < DeviceStatusLimits.UnderperformingLoad;
        public bool IsOverloaded => LoadPercentage > DeviceStatusLimits.OverloadedLoad;
        private DeviceStatus(
            EntityId deviceId,
            Power currentPower,
            Percentage loadPercentage,
            DateTime lastHeartbeat
        )
        {
            DeviceId = deviceId;
            CurrentPower = currentPower;
            LoadPercentage = loadPercentage;
            LastHeartbeat = lastHeartbeat;
        }

        #region Factory Method

        public static DeviceStatus CreateDefault(EntityId deviceId, DateTime now)
        {
            return new DeviceStatus(
                deviceId,
                Power.Create(0).Value,
                Percentage.Zero(),
                now
            );
        }
        public static Result<DeviceStatus> Load(
                  string deviceId,
                  double currentPower,
                  double loadPercentage,
                  DateTime lastHeartbeat)
        {
            var deviceIdResult = EntityId.Create(deviceId);
            if (deviceIdResult.IsFailure)
                return Result<DeviceStatus>.Failure(deviceIdResult.Error!.Message, ErrorType.Validation);

            var powerResult = Power.Create(currentPower);
            if (powerResult.IsFailure)
                return Result<DeviceStatus>.Failure(powerResult.Error!.Message, ErrorType.Validation);

            var loadResult = Percentage.Create(loadPercentage);
            if (loadResult.IsFailure)
                return Result<DeviceStatus>.Failure(loadResult.Error!.Message, ErrorType.Validation);

            return Result<DeviceStatus>.Success(new DeviceStatus(
                deviceIdResult.Value,
                powerResult.Value,
                Percentage.FromRaw(loadResult.Value),
                lastHeartbeat
            ));
        }

        #endregion

        #region Domain Logic

        public void UpdateTelemetry(Telemetry telemetry)
        {
            LastHeartbeat = telemetry.Timestamp;
            CurrentPower = telemetry.CurrentPower;
            LoadPercentage = telemetry.LoadPercentage;
        }
        public Alert? GetCurrentAnomaly(DateTime currentTime)
        {
            if (!IsOnline(currentTime))
            {
                return Alert.Create(
                    DeviceId,
                    AlertType.Critical,
                    $"Heartbeat missing. Device has been offline since {LastHeartbeat} UTC.").Value;
            }

            if (IsOverloaded)
            {
                return Alert.Create(
                    DeviceId,
                    AlertType.Critical,
                    $"Critical load detected: {LoadPercentage}. Overload threshold exceeded.").Value;
            }

            if (IsUnderperforming)
            {
                return Alert.Create(
                    DeviceId,
                    AlertType.Warning,
                    $"Device is underperforming. Current load is only {LoadPercentage}.").Value;
            }

            return null;
        }

        #endregion
    }
}
