using System.Net;
using System.Text.Json;

public class ErrorHandling
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandling> _logger;

    public ErrorHandling(RequestDelegate next, ILogger<ErrorHandling> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");

            var response = context.Response;
            response.ContentType = "application/json";
            response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var error = new
            {
                message = ex.Message,
                inner = ex.InnerException?.Message,
                stackTrace = ex.StackTrace,           
                timestamp = DateTime.UtcNow
            };

            var options = new JsonSerializerOptions { WriteIndented = true };
            await response.WriteAsync(JsonSerializer.Serialize(error, options));
        }
    }
}
