using System.Net;
using System.Text.Json;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");

            context.Response.ContentType = "application/problem+json";

            var statusCode = HttpStatusCode.InternalServerError;
            var title = "Internal Server Error";
            var detail = ex.Message;

            // Handle basic HTTP responses
            if (ex is HttpRequestException)
            {
                statusCode = HttpStatusCode.BadGateway;
                title = "External Service Error";
            }
            else if (ex is ArgumentException)
            {
                statusCode = HttpStatusCode.BadRequest;
                title = "Bad Request";
            }

            context.Response.StatusCode = (int)statusCode;

            var problemDetails = new
            {
                type = $"https://httpstatuses.com/{(int)statusCode}",
                title,
                status = (int)statusCode,
                detail,
                instance = context.Request.Path
            };

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, options));
        }
    }
}
