using JobApplicationApi.Dtos;
using System.Net;
using System.Text.Json;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            _logger.LogError(ex, "An unhandled exception occurred.");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = ex is BadHttpRequestException badRequest
                ? badRequest.StatusCode : (int)HttpStatusCode.InternalServerError;

            var errorResponse = new ErrorDto
            {
                StatusCode = context.Response.StatusCode,
                Message = ex is BadHttpRequestException ? ex.Message : "An unexpected error occurred. Please try again later.",
                Errors = null
            };

            var json = JsonSerializer.Serialize(errorResponse);
            await context.Response.WriteAsync(json);
        }
    }
}