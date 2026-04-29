using Microsoft.Extensions.Configuration;
using SmartGrid.ITSimulator.Services;
using SmartGrid.ITSimulator.UI;


// Load configuration
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

// Get settings
var baseApiUrl = configuration["SimulatorSettings:BaseApiUrl"]
    ?? throw new InvalidOperationException("Device URL is not configured");
var delayMs = int.Parse(configuration["SimulatorSettings:DelayMilliseconds"] ?? "10000");
var maxVariation = double.Parse(configuration["SimulatorSettings:MaxPowerVariation"] ?? "50");

// Initialize services
ConsoleUI.PrintHeader();

string deviceName = ConsoleUI.GetDeviceNameInput();
double nominalPower = ConsoleUI.GetNominalPowerInput();

using var httpClient = new HttpClient { BaseAddress = new Uri(baseApiUrl) };

var simulator = new SimulatorService(maxVariation);
var publisher = new TelemetryPublisher(httpClient);

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine($"\n[SYSTEM] Registering device '{deviceName}' on API...");
Console.ResetColor();

ConsoleUI.PrintStartMessage(deviceName, baseApiUrl + "/api/ReceiveTelemetry");

string deviceId = Guid.NewGuid().ToString();

try
{
    while (true)
    {
        var telemetry = simulator.GenerateTelemetry(deviceId,
                                                    deviceName,
                                                    nominalPower);
        var (success, errorMessage) = await publisher.PublishSafeAsync(telemetry);

        if (success)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUI.PrintSuccess(telemetry.CurrentPower, telemetry.NominalPower);
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            ConsoleUI.PrintError(errorMessage ?? "Unknown error");
            Console.ResetColor();
        }

        await Task.Delay(delayMs);
    }
}
catch (OperationCanceledException)
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("\n[SYSTEM] Simulation stopped by user.");
    Console.ResetColor();
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    ConsoleUI.PrintCritical($"Fatal error: {ex.Message}");
    Console.ResetColor();
    Environment.Exit(1);
}