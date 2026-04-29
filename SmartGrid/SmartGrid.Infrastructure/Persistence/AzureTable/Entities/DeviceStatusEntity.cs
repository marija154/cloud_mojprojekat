namespace SmartGrid.Infrastructure.Persistence.AzureTable.Entities
{
    internal class DeviceStatusEntity : BaseTableEntity
    {
        public double CurrentPower { get; set; }
        public double LoadPercentage { get; set; }
        public DateTime LastHeartbeat { get; set; }
    }
}