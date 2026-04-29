using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace FirstFunction.Functions;

public class NumberAddition
{
    private readonly ILogger<NumberAddition> _logger;

    public NumberAddition(ILogger<NumberAddition> logger)
    {
        _logger = logger;
    }

    [Function("NumberAddition")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
    {
        _logger.LogInformation("Processing addition request...");

        // 1. Reading the request body (JSON)
        string requestBody = 
            await new StreamReader(req.Body).ReadToEndAsync();

        try
        {
            // 2. Deserializing JSON into a C# object
            var data = 
                JsonSerializer.Deserialize<AdditionRequest>(requestBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (data == null)
            {
                _logger.LogWarning("Request body is empty.");
                return 
                    new BadRequestObjectResult("\"Please provide 'A' and 'B' in the request body.\"");
            }

            // 3. Logic: Performing addition
            int result = data.A + data.B;

            _logger.LogInformation($"Operation successful {data.A} + {data.B} = {result}");

            // 4. Returning the result to the client
            return new OkObjectResult(new
            {
                sum = result,
                message = $"The result of addition is {result}"
            });
        }
        catch (JsonException ex) {
            _logger.LogError($"JSON Error: {ex.Message}");
            return 
                new BadRequestObjectResult("Invalid JSON format. Expected: { \"A\": number, \"B\": number }");
        }

    }
}

public class AdditionRequest
{
    public int A { get; set; }
    public int B { get; set; }
}