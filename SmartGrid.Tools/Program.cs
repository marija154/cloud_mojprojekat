using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

const string ConnectionString = "UseDevelopmentStorage=true";

Console.WriteLine("--- SmartGrid Infrastructure Initializer ---");
Console.WriteLine($"Target Storage: {ConnectionString}");
Console.WriteLine();

var tables = new[] { "Devices", "Telemetries", "DeviceStatuses", "Firmwares" };
var blobs = new[] { "firmware-updates" };

var tableServiceClient = new TableServiceClient(ConnectionString);
var blobServiceClient = new BlobServiceClient(ConnectionString);

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

foreach (var blobContainerName in blobs)
{
    try
    {
        var containerClient = blobServiceClient.GetBlobContainerClient(blobContainerName);

        Console.Write($"Deleting blob container (if exists): {blobContainerName}...");

        await containerClient.DeleteIfExistsAsync();

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(" [DELETED]");
        Console.ResetColor();

        bool created = false;
        while (!created)
        {
            try
            {
                Console.Write($"Creating fresh container: {blobContainerName}...");
                await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);
                created = true;
            }
            catch (Azure.RequestFailedException ex) when (ex.Status == 409)
            {
                Console.Write(".");
                await Task.Delay(1000);
            }
        }

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