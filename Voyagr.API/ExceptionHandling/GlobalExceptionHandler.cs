using Microsoft.AspNetCore.Diagnostics;
using Voyagr.Application.DTOS.Common;

namespace Voyagr.API.ExceptionHandling
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }
        public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
        {
            _logger.LogError(
                exception,
                "Unhandled exception occurred.");

            httpContext.Response.StatusCode =
                StatusCodes.Status500InternalServerError;

            httpContext.Response.ContentType =
                "application/json";

            var response = new ApiErrorResponse
            {
                Message = "An unexpected error occurred.",
                Errors = null
            };

            await httpContext.Response.WriteAsJsonAsync(
                response,
                cancellationToken);

            return true;
        }
    }
}
