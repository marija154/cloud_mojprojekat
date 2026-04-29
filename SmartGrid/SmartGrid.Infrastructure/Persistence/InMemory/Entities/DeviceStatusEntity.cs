namespace SmartGrid.Infrastructure.Persistence.InMemory.Entities
{
    public class DeviceStatusEntity
    {
        public string DeviceId { get; set; } = string.Empty;
        public double CurrentPower { get; set; }
        public double LoadPercentage { get; set; }
        public DateTime LastHeartbeat { get; set; }
    }
}
