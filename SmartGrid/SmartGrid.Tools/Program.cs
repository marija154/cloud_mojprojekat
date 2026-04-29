using Azure.Data.Tables;

const string ConnectionString = "UseDevelopmentStorage=true";

Console.WriteLine("--- SmartGrid Infrastructure Initializer ---");
Console.WriteLine($"Target Storage: {ConnectionString}");
Console.WriteLine();

var tables = new[] { "Devices", "Telemetries", "DeviceStatuses"};

var tableServiceClient = new TableServiceClient(ConnectionString);

foreach (var tableName in tables)
{
    try
    {
        var tableClient = tableServiceClient.GetTableClient(tableName);

        Console.Write($"Deleting table (if exists): {tableName}...");

        await tableClient.DeleteAsync();

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(" [DELETED]");
        Console.ResetColor();


        Console.Write($"Creating fresh table: {tableName}...");
        await tableClient.CreateIfNotExistsAsync();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(" [OK]");
        Console.ResetColor();
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($" [FAILED] - Error: {ex.Message}");
        Console.ResetColor();
    }
}

Console.WriteLine();
Console.WriteLine("--- Infrastructure setup completed ---");